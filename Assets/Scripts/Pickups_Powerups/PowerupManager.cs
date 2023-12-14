using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public List<Powerup> powerups;
    public AudioClip powerupAudio;
    private List<Powerup> removedPowerUpQueue;

    // Start is called before the first frame update
    void Start()
    {
        powerups = new List<Powerup>();
        removedPowerUpQueue = new List<Powerup>();
    }

    // Update is called once per frame
    void Update()
    {
        DecrementPowerupTimers();
    }

    private void LateUpdate()
    {
        ApplyRemovePowerupsQueue();
    }

    public void Add(Powerup powerupToAdd)
    {
        // Apply powerup
        powerupToAdd.Apply(this);

        // Save it to the list
        powerups.Add(powerupToAdd);

        // Find the audio source
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        // Play the sound effect
        audioSource.PlayOneShot(powerupAudio);
    }

    public void Remove(Powerup powerupToRemove)
    {
        // Remove the powerup
        powerupToRemove.Remove(this);

        // Remove it from the list
        removedPowerUpQueue.Add(powerupToRemove);
    }

    public void RemoveOfType(Type type)
    {
        // Iterate through all of our powerups
        foreach (Powerup powerup in powerups)
        {
            // If a powerup we have matches the type we give it, remove it
            if (powerup.GetType() == type)
            {
                Remove(powerup);
            }
        }
    }

    public void DecrementPowerupTimers()
    {
        // One at at ime, put each object in "powerups" into the variable "powerup" and do the loop body on it
        foreach (Powerup powerup in powerups)
        {
            if (powerup.isPermanent == false)
            {
                // Subtract the time that it took to draw the frame from the duration
                powerup.duration -= Time.deltaTime;

                // If time is up, remove this powerup
                if (powerup.duration <= 0)
                {
                    Remove(powerup);
                }
            }
        }
    }

    private void ApplyRemovePowerupsQueue()
    {
        // Remove the queued powerups from the applied list
        foreach (Powerup powerup in removedPowerUpQueue)
        {
            powerups.Remove(powerup);
        }

        // Reset our temporary list
        removedPowerUpQueue.Clear();
    }
}
