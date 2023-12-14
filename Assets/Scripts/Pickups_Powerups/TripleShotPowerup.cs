using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TripleShotPowerup : Powerup
{
    public ShipShooter.ShotTypes shotType;

    public override void Apply(PowerupManager target)
    {
        // Find the shooter component
        ShipShooter targetShooter = target.GetComponent<ShipShooter>();

        if (targetShooter != null)
        {
            // Change the player's shot type
            targetShooter.ShotType = shotType;
        }
    }

    public override void Remove(PowerupManager target)
    {
        ShipShooter targetShooter = target.GetComponent<ShipShooter>();

        // If the player is still using this powerup (they haven't overwritten it), then reset their shot type to standard
        if (targetShooter.ShotType == shotType)
        {
            targetShooter.ShotType = ShipShooter.ShotTypes.Standard;
        }
    }
}
