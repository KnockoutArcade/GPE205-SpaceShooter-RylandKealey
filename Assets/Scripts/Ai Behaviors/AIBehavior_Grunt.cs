using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehavior_Grunt : AIController
{
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
                // If we have just spawned and are on the right side of the screen, keep moving left until we reach x = 0
                if (pawn.transform.position.x > 0)
                {
                    pawn.Move(-1.0f, 0.0f);
                }
                else
                {
                    // Once we get there, stop for a bit
                    ChangeState(AIState.Stationary);
                }

                break;

            case AIState.Stationary:

                // Stand still for a bit, then continue moving forward
                if (Time.time >= lastStateChangeTime + 5.0f)
                {
                    ChangeState(AIState.Attacking);
                }
                
                break;

            case AIState.Attacking:
                // Keep moving until we reach the edge of the screen
                pawn.Move(-1.0f, 0.0f);

                // If we reach the edge of the screen, die
                if (pawn.transform.position.x <= -12.0f)
                {
                    Destroy(pawn.gameObject);
                }

                break;

            case AIState.Panic:
                break;
        }
    }
}
