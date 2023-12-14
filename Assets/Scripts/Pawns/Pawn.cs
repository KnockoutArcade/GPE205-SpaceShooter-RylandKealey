using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    /// <summary>
    /// How fast this pawn moves
    /// </summary>
    public float moveSpeed;
    /// <summary>
    /// Variable for the rotation speed of the pawn
    /// </summary>
    public float turnSpeed;
    /// <summary>
    /// The controller object this pawn is attached to
    /// </summary>
    public PlayerController playerController;
    /// <summary>
    /// Which team is this pawn on?
    /// </summary>
    public float team;
    public Vector3 shotAngle = new Vector3(1.0f, 0.0f, 0.0f);

    /// <summary>
    /// How much invincibility time this pawn has left
    /// </summary>
    public float invincibilityTime;

    /// <summary>
    /// Whether this object should be invincible or not
    /// </summary>
    public bool isInvincible = false;

    /// <summary>
    /// How much invincibility time this pawn gains when taking damage
    /// </summary>
    public float invincibilityGain;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Move(float xMove, float zMove);
    public abstract void Shoot();
}
