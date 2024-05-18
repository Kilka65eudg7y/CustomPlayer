using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour, IDamageable
{
    [HideInInspector] public BaseEntity Parent;
    [SerializeField] private float DamageMultiplier;
    private Collider HitboxCollider;

    public string HitboxName
    {
        get
        {
            return HitboxName;
        }
        set
        {
            HitboxName = value;
        }
    }
    public void TakeDamage(int Damage)
    {
        Parent.TakeDamage((int)((float)Damage * DamageMultiplier));
    }
    private void Start()
    {
        HitboxCollider = GetComponent<Collider>();
        HitboxCollider.isTrigger = true;
    }
}
