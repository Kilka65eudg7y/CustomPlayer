using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum MovementState
    {
        None,
        Run,
        Walk,
        Air
    }

    [Header("Movement")]
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float RunSpeed;
    [SerializeField] private float AccelerationSpeed;
    private MovementState m_MovementState;
    private Vector3 MovementDirection;
    private float CurrentSpeed;

    [Header("Grounded")]
    [SerializeField] private Vector3 GroundCheckPosition;
    [SerializeField] private float GroundCheckSize;
    private bool IsGrounded;

    [Header("Camera")]
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float Sensivity;
    [SerializeField] private float CameraSmoothness;

    [Header("Dynamic fov")]
    [SerializeField] private float WalkFov;
    [SerializeField] private float RunFov;
    [SerializeField] private float DefaultFov;
    [SerializeField] private float FovSmoothness;

    [Header("Head bob")]
    [SerializeField] private float WalkBobbingSpeed = 14f;
    [SerializeField] private float RunBobbingSpeed = 14f;
    [SerializeField] private float BobbingAmount = 0.05f;
    [SerializeField] private float HeadBobSmoothness = 0.01f;
    float DefaultCameraY = 0;
    float HeadBobTimer = 0;

    [Header("Other")]
    public bool CanMove;
    public bool CanLook;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        DefaultCameraY = CameraTransform.localPosition.y;
    }
    private void Update()
    {
        IsGrounded = Physics.Raycast(GroundCheckPosition + transform.position, Vector3.down, GroundCheckSize);

        if (CanMove)
        {
            UpdateState();
            Movement();
            HeadBob();
        }
        if (CanLook)
        {
            Look();
        }
    }
    private void HeadBob()
    {
        Vector3 Velocity = Vector3.zero;
        if (m_MovementState != MovementState.None)
        {
            if (m_MovementState == MovementState.Walk)
            {
                HeadBobTimer += Time.deltaTime * WalkBobbingSpeed;
            }
            else if (m_MovementState == MovementState.Run)
            {
                HeadBobTimer += Time.deltaTime * RunBobbingSpeed;
            }
            CameraTransform.localPosition = new Vector3(CameraTransform.localPosition.x, DefaultCameraY + Mathf.Sin(HeadBobTimer) * BobbingAmount, CameraTransform.localPosition.z);
        }
        else
        {
            HeadBobTimer = 0;
            CameraTransform.localPosition = Vector3.SmoothDamp(CameraTransform.localPosition, new Vector3(CameraTransform.localPosition.x, DefaultCameraY, CameraTransform.localPosition.z), ref Velocity, HeadBobSmoothness);
        }
    }
    private void Look()
    {
        Vector3 TransformVelocity = Vector3.zero;
        float CameraVelocity = 0f; 

        Vector3 CameraAngles = CameraTransform.eulerAngles;
        CameraAngles.x = Mathf.SmoothDamp(CameraAngles.x, CameraAngles.x + -Input.GetAxis("Mouse Y") * Sensivity, ref CameraVelocity, CameraSmoothness * Time.deltaTime);
        CameraTransform.eulerAngles = CameraAngles;
        transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, new Vector3(0f, Input.GetAxis("Mouse X") * Sensivity, 0f) + transform.eulerAngles, ref TransformVelocity, CameraSmoothness * Time.deltaTime);
    }
    private void Movement()
    {
        MovementDirection = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal") * CurrentSpeed * Time.deltaTime, 0f, Input.GetAxis("Vertical") * CurrentSpeed * Time.deltaTime));
        transform.position += MovementDirection;
    }
    private void UpdateState()
    {
        if (IsGrounded && Input.GetKey(KeyCode.W))
        {
            m_MovementState = Input.GetKey(KeyCode.LeftShift) ? MovementState.Run : MovementState.Walk;
        }
        else if (IsGrounded && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            m_MovementState = MovementState.Walk;
        }
        else
        {
            m_MovementState = MovementState.None;
        }

        float Velocity = 0f;
        float FovVelocity = 0f;
        switch (m_MovementState)
        {
            case MovementState.None:
                CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, 0f, ref Velocity, AccelerationSpeed * Time.deltaTime);
                m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, DefaultFov, ref FovVelocity, FovSmoothness * Time.deltaTime);
                break;
            case MovementState.Walk:
                CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, WalkSpeed, ref Velocity, AccelerationSpeed * Time.deltaTime);
                m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, WalkFov, ref FovVelocity, FovSmoothness * Time.deltaTime);
                break;
            case MovementState.Run:
                CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, RunSpeed, ref Velocity, AccelerationSpeed * Time.deltaTime);
                m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, RunFov, ref FovVelocity, FovSmoothness * Time.deltaTime);
                break;
        }
    }
}
