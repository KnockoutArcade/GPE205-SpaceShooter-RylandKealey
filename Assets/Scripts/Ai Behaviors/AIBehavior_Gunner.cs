using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using static AIController;

public class AIBehavior_Gunner : AIController
{
    /// <summary>
    /// which direction this enemy is traveling on the z axis during its attack phase
    /// </summary>
    private float travelDirection = 1;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    protected override void MakeDecisions()
    {
        switch (currentState)
        {
            case AIState.Moving:
                // If we have just spawned and are on the right side of the screen, keep moving left until we reach x = 4
                if (pawn.transform.position.x > 4.0f)
                {
                    // We want the ship to wiggle up and down slightly as it moves in, so use a sine function to offset the z value
                    float zOffset = Mathf.Sin(Time.time * 7.0f);
                    //Debug.Log(zOffset);
                    
                    pawn.Move(-1.0f, zOffset);
                }
                else
                {
                    // Once we get there, transition to attacking
                    ChangeState(AIState.Attacking);

                    // Reduce movment speed during this time
                    pawn.moveSpeed = 50.0f;
                }

                break;

            case AIState.Stationary:
                break;

            case AIState.Attacking:
                // Move up and down on the z axis while shooting

                if (pawn.transform.position.z < -6.0f)
                {
                    // If the pawn has hit the bottom of the screen, make it move upwards
                    travelDirection = 1;
                }
                else if (pawn.transform.position.z > 4.0f)
                {
                    // If the pawn has hit the top of the screen, make it move downwards
                    travelDirection = -1;
                }

                // Update movement
                pawn.Move(0.0f, travelDirection);

                // Reset the shot angle so that this enemy always shoots in a straight line, regaurdless of where the player is
                pawn.shotAngle = new Vector3(-1.0f, 0.0f, 0.0f);

                // Just Shoot MFs
                Shoot();

                break;

            case AIState.Panic:
                break;
        }
    }
}
