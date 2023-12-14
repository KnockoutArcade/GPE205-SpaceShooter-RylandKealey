using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRatePickup : Pickup
{
    public FireRatePowerup powerup;

    public void OnTriggerEnter(Collider other)
    {
        // Get the PowerupManager object
        PowerupManager powerupManager = other.GetComponent<PowerupManager>();

        // If it has a powerup Manager
        if (powerupManager != null)
        {
            // Add the powerup
            powerupManager.Add(powerup);

            // Destroy this pickup
            Destroy(gameObject);
        }
    }
}
