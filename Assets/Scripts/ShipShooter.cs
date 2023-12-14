using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipShooter : Shooter
{
    public enum ShotTypes { Standard, Triple };
    public ShotTypes ShotType;
    public Transform firepointTransform;
    public AudioSource shotSoundSource;
    public AudioClip shotSoundClip;
    public float shotsPerSecond;
    private float secondsPerShot;
    private float shotTimer;
    public bool canShoot = false;

    // Start is called before the first frame update
    public override void Start()
    {
        // Get the reciprical of shotsPerSecond to make it easier to work with
        secondsPerShot = 1 / shotsPerSecond;

        shotTimer = secondsPerShot;
    }

    // Update is called once per frame
    public override void Update()
    {
        // Subtract how much time has passed from the timer
        shotTimer -= Time.deltaTime;

        // if the timer is below 0
        if (shotTimer <= 0)
        {
            canShoot = true;
        }

        secondsPerShot = 1 / shotsPerSecond;
    }

    public override void Shoot(GameObject shellPrefab, float fireForce, float damageDone, float lifespan, Vector3 angle)
    {
        if (canShoot)
        {
            // Instantiate the projectile
            GameObject newShell = Instantiate(shellPrefab, firepointTransform.position, firepointTransform.rotation);

            #region Modify DamageOnHit Component
            // Get the DamageOnHit component
            DamageOnHit doh = newShell.GetComponent<DamageOnHit>();

            // if it has one...
            if (doh != null)
            {
                // Set damageDone to the value passed in
                doh.damageDone = damageDone;

                // Set the owner to this pawn
                doh.owner = this.GetComponent<Pawn>();

                // Set the projectile's team
                doh.team = doh.owner.team;
            }
            #endregion

            #region Launch Rigidbody Forwards
            // Get the rigidbody
            Rigidbody rb = newShell.GetComponent<Rigidbody>();

            // if it has a rigidbody...
            if (rb != null)
            {
                rb.AddForce(firepointTransform.forward + angle.normalized * fireForce);
            }
            #endregion

            // Reset any sound that might be happening
            shotSoundSource.Stop();

            // Play sound effect
            shotSoundSource.PlayOneShot(shotSoundClip);

            // Destroy it after a set time
            Destroy(newShell, lifespan);

            // Reset the Timer
            shotTimer = secondsPerShot;

            // Reset CanShoot
            canShoot = false;
        }
    }

    public override void ShootTriple(GameObject shellPrefab, float fireForce, float damageDone, float lifespan, Vector3 angle)
    {
        if (canShoot)
        {
            // To fire a spread shot, we spawn multiple bullets with slightly different angle offsets from the original shot

            // Create a Quaternion with the offset of the 1st bullet
            Quaternion q = Quaternion.Euler(0.0f, 0.0f, 0.15f);
            // Convert the quaternion into a Vector3 and add it to the shot angle
            Vector3 shotangleOffset = angle + q.eulerAngles;

            // Spawn the first bullet with an offset
            Shoot(shellPrefab, fireForce, damageDone, lifespan, shotangleOffset);

            // Allow this script to shoot the next bullet
            canShoot = true;

            // Spawn the second bullet with no offet
            Shoot(shellPrefab, fireForce, damageDone, lifespan, angle);

            // Allow this script to shoot the next bullet
            canShoot = true;

            // Invert the offset
            shotangleOffset = angle - q.eulerAngles;
            // Spawn the third bullet with an offset going the other direction
            Shoot(shellPrefab, fireForce, damageDone, lifespan, shotangleOffset);

            // Reset the Timer
            shotTimer = secondsPerShot;

            // Reset CanShoot
            canShoot = false;
        }
    }
}
