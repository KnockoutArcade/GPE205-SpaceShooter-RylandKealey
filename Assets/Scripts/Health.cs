using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float _currentHealth;
    public float maxHealth;
    //public Image healthBar;
    public AudioClip deathSoundFX;
    public AudioClip hurtSoundFX;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = maxHealth;
    }

    /// <summary>
    /// Subract incoming damage from health
    /// </summary>
    /// <param name="amount">The amount of damage recieved</param>
    /// <param name="source">The player that caused this damage</param>
    public void TakeDamage(float amount, Pawn source)
    {
        _currentHealth = _currentHealth - amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);

        if (_currentHealth <= 0)
        {
            Die(source);
        }

        // Find the audio source on this object
        AudioSource hitaudio = gameObject.GetComponent<AudioSource>();
        // Play the sound efect
        hitaudio.PlayOneShot(hurtSoundFX);
    }

    public void Heal(float amount, Pawn source)
    {
        _currentHealth = _currentHealth + amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
    }

    public void Die(Pawn source)
    {
        // Find a dropPickup component
        DropPickup dropPickup = GetComponent<DropPickup>();

        // If it has one, drop the pickup
        if (dropPickup != null) 
        {
            dropPickup.TryDropPickup();
        }

        // Get the player controller of the object that killed this
        PlayerController controller = source.playerController;
        // If the controller is found, add 500 points to the score
        if (controller != null)
        {
            controller.AddToScore(500);
        }

        // Play the destroy sound
        AudioSource.PlayClipAtPoint(deathSoundFX, new Vector3(0.0f, 0.0f, 0.0f));

        Destroy(gameObject);

        Debug.Log(source.name + "killed " + gameObject.name);
    }
}
