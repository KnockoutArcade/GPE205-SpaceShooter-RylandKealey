using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPickup : MonoBehaviour
{
    public GameObject pickupToDrop; // The pickup that this object can drop
    public int dropChance; // How likely this pickup is to drop

    public void TryDropPickup()
    {
        // Pick a random number between 0 and the drop chance
        int roll = Random.Range(0, dropChance);

        // If we rolled a zero...
        if (roll == 0)
        {
            // Spawn the pickup
            Instantiate(pickupToDrop, transform.position, Quaternion.identity);
        }
    }
}
