using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehavior_Sniper : AIController
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
                // If we have just spawned and are on the right side of the screen, keep moving left until we reach x = 7
                if (pawn.transform.position.x > 7.0f)
                {
                    pawn.Move(-1.0f, 0.0f);
                }
                else
                {
                    // Once we get there, transition to attacking
                    ChangeState(AIState.Attacking);
                }

                break;

            case AIState.Stationary:
                break;

            case AIState.Attacking:
                // Check if we have a target
                if (!IsHasTarget())
                {
                    TargetPlayerOne();
                }

                // Just Shoot MFs
                DoAttackState();

                break;

            case AIState.Panic:
                break;
        }
    }
}
