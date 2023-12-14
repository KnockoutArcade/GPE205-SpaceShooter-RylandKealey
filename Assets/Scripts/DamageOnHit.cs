using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public float damageDone;
    public Pawn owner;
    public float team;

    public void OnTriggerEnter(Collider other)
    {
        // Get the pawn component of whatever we hit
        Pawn otherPawn = other.GetComponent<Pawn>();

        // if the thing we hit has a pawn object...
        if (otherPawn != null)
        {
            // and if it is not on our team AND if it is not invincible
            if (otherPawn.team != team && !otherPawn.isInvincible)
            {
                // If the object this projectile hit has the health component
                Health otherHealth = other.gameObject.GetComponent<Health>();

                // Damage it if it has a Health Component
                if (otherHealth != null)
                {
                    otherHealth.TakeDamage(damageDone, owner);
                }

                // Increase its invincibility
                otherPawn.isInvincible = true;
                otherPawn.invincibilityTime += otherPawn.invincibilityGain;

                // Destroy the projectile when it hits anything not on our team
                Destroy(gameObject);
            }
        }
        //else
        //{
        //    // Destroy the projectile when it hits anything not on our team
        //    Destroy(gameObject);
        //}
    }
}
