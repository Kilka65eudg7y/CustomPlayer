using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : BaseEntity
{
    public enum MovementState
    {
        None,
        Run,
        Walk,
        Air
    }

    [Header("Movement")]
    [SerializeField] private float WalkSpeed = 5f;
    [SerializeField] private float RunSpeed = 7f;
    [SerializeField] private float AirSpeed = 3f;
    [SerializeField] private float AccelerationSpeed = 15f;
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private float JumpForce = 4.5f;
    private MovementState m_MovementState;
    private Vector3 MovementDirection;
    private float CurrentSpeed;

    [Header("Grounded")]
    [SerializeField] private Vector3 GroundCheckPosition;
    [SerializeField] private float GroundCheckSize = 1.1f;
    private bool IsGrounded;

    [Header("Camera")]
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float Sensivity = 1f;
    [SerializeField] private bool EnableCameraXClamp = false;
    [SerializeField] private Vector2 CameraClampX;
    [SerializeField] private bool EnableCameraYClamp = true;
    [SerializeField] private Vector2 CameraClampY = new Vector2(-75, 75);
    private float CameraX;
    private float CameraY;

    [Header("Dynamic fov")]
    [SerializeField] private float WalkFov = 65f;
    [SerializeField] private float RunFov = 70f;
    [SerializeField] private float AirFov = 63f;
    [SerializeField] private float DefaultFov = 60f;
    [SerializeField] private float FovSmoothness = 15f;

    [Header("Head bob")]
    [SerializeField] private float WalkBobbingSpeed = 14f;
    [SerializeField] private float RunBobbingSpeed = 18f;
    [SerializeField] private float BobbingAmount = 0.05f;
    [SerializeField] private float HeadBobSmoothness = 0.01f;
    float DefaultCameraY = 0;
    float HeadBobTimer = 0;

    [Header("Other")]
    public bool CanMove;
    public bool CanLook;
    public bool CanJump;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        DefaultCameraY = CameraTransform.localPosition.y;
        Rigidbody = GetComponent<Rigidbody>();  
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
        if (CanJump && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
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
        CameraX += Input.GetAxis("Mouse X") * Sensivity;
        CameraY += Input.GetAxis("Mouse Y") * Sensivity;

        if (EnableCameraXClamp)
        {
            CameraX = Mathf.Clamp(CameraX, CameraClampX.x, CameraClampX.y);
        }
        if (EnableCameraYClamp)
        {
            CameraY = Mathf.Clamp(CameraY, CameraClampY.x, CameraClampY.y);
        }

        CameraTransform.eulerAngles = new Vector3(
            -CameraY,
            transform.eulerAngles.y);
        transform.eulerAngles = new Vector3(0f, CameraX, 0f);
    }
    private void Movement()
    {
        MovementDirection = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal") * CurrentSpeed, 0f, Input.GetAxis("Vertical") * CurrentSpeed));
        Rigidbody.velocity = new Vector3(MovementDirection.x, Rigidbody.velocity.y, MovementDirection.z);
    }
    private void Jump()
    {
        if(IsGrounded)
        {
            Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
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
        else if(!IsGrounded)
        {
            m_MovementState = MovementState.Air;
        }
        else
        {
            m_MovementState = MovementState.None;
        }

        float AccelerationVelocity = 0f;
        float FovVelocity = 0f;
        switch (m_MovementState)
        {
            case MovementState.None:
                CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, 0f, ref AccelerationVelocity, AccelerationSpeed * Time.deltaTime);
                m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, DefaultFov, ref FovVelocity, FovSmoothness * Time.deltaTime);
                break;
            case MovementState.Walk:
                CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, WalkSpeed, ref AccelerationVelocity, AccelerationSpeed * Time.deltaTime);
                m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, WalkFov, ref FovVelocity, FovSmoothness * Time.deltaTime);
                break;
            case MovementState.Run:
                CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, RunSpeed, ref AccelerationVelocity, AccelerationSpeed * Time.deltaTime);
                m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, RunFov, ref FovVelocity, FovSmoothness * Time.deltaTime);
                break;
            case MovementState.Air:
                CurrentSpeed = Mathf.SmoothDamp(CurrentSpeed, AirSpeed, ref AccelerationVelocity, AccelerationSpeed * Time.deltaTime);
                m_Camera.fieldOfView = Mathf.SmoothDamp(m_Camera.fieldOfView, AirFov, ref FovVelocity, FovSmoothness * Time.deltaTime);
                break;
        }
    }
}
