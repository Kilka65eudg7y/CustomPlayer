using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    [SerializeField] private List<Hitbox> Hitboxes;
    [SerializeField] private int MaxHealth;
    private int Health;

    
    public void TakeDamage(int Damage)
    {
        Health -= Damage;
        Debug.Log(Health);
        if (Health <= 0)
        {
            OnDead();
        }
    }
    public virtual void OnDead()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        Health = MaxHealth;
        foreach(Hitbox hitbox in Hitboxes)
        {
            hitbox.Parent = this;
        }
        OnStart();
    }
    public virtual void OnStart()
    {
        
    }
}
