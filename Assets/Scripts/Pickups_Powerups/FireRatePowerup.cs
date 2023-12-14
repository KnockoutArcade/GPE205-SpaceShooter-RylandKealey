using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FireRatePowerup : Powerup
{
    public float fireRateIncrease;

    public override void Apply(PowerupManager target)
    {
        // Increase the firing rate of the player
        ShipShooter targetShooter = target.GetComponent<ShipShooter>();

        if (targetShooter != null)
        {
            targetShooter.shotsPerSecond += fireRateIncrease;
        }
    }

    public override void Remove(PowerupManager target)
    {
        ShipShooter targetShooter = target.GetComponent<ShipShooter>();

        targetShooter.shotsPerSecond -= fireRateIncrease;
    }
}
