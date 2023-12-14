using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPawn : Pawn
{
    public GameObject shellPrefab;
    public GameObject minePrefab;
    public float fireForce;
    public float damageDone;
    public float shellLifespan;
    protected ShipShooter shooter;
    private Rigidbody rb;
    

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        shooter = GetComponent<ShipShooter>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Decrease invincibility
        if (invincibilityTime > 0.0f)
        {
            invincibilityTime -= Time.deltaTime;
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
        }
    }

    #region Override Functions
    public override void Move(float xMove, float zMove)
    {
        // Create Vector3 direction to move in
        Vector3 direction = new Vector3(xMove, 0.0f, zMove);

        // Move the rigid body
        rb.MovePosition(transform.position + direction.normalized * moveSpeed * Time.deltaTime);
    }
    public override void Shoot()
    {
        // Shoot depending on the shot type
        if (shooter.ShotType == ShipShooter.ShotTypes.Standard)
        {
            shooter.Shoot(shellPrefab, fireForce, damageDone, shellLifespan, shotAngle);
        }
        else if (shooter.ShotType == ShipShooter.ShotTypes.Triple)
        {
            shooter.ShootTriple(shellPrefab, fireForce, damageDone, shellLifespan, shotAngle);
        }
    }
    #endregion
}
