using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
public class Hitbox : MonoBehaviour, IDamageable
{
    [HideInInspector] public BaseEntity Parent;
    [SerializeField] private float DamageMultiplier;
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
}
