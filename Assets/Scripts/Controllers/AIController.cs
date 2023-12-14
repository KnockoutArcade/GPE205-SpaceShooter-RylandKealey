using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{
    #region Variables
    public enum AIState { Moving, Stationary, Attacking, Panic};

    /// <summary>
    /// The current state of this FSM
    /// </summary>
    public AIState currentState;

    protected float lastStateChangeTime;

    public GameObject target;

    public float fleeDistance;

    /// <summary>
    /// How far away this can hear things
    /// </summary>
    public float hearingDistance;

    /// <summary>
    /// How far this can see things
    /// </summary>
    public float maxVisionDistance;
    public float fieldOfView;
    #endregion


    // Start is called before the first frame update
    public override void Start()
    {
        // If manager exists
        {
            if (GameManager.instance != null)
            {
                // And if it can track enemies
                if (GameManager.instance.enemies != null)
                {
                    // Register it to the GameManager
                    GameManager.instance.enemies.Add(this);
                }
            }
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        if (pawn == null)
        {
            // If manager exists
            {
                if (GameManager.instance != null)
                {
                    // And if it can track players
                    if (GameManager.instance.enemies != null)
                    {
                        // Remove it from the GameManager
                        GameManager.instance.enemies.Remove(this);
                    }
                }
            }

            Destroy(this.gameObject);
        }
        else
        {
            MakeDecisions();
        }
    }

    /// <summary>
    /// Autonomously make decisions about what to do in the current state
    /// </summary>
    protected virtual void MakeDecisions()
    {
        switch (currentState)
        {
            case AIState.Moving:
                break;

            case AIState.Stationary:
                
                // Check if we have a target
                if (!IsHasTarget())
                {
                    TargetPlayerOne();
                }

                // Do Work
                DoAttackState();

                //Check for transitions
                if (!IsDistanceLessThan(target, 10))
                {
                    //ChangeState(AIState.Patrol);
                }
                break;

            case AIState.Attacking:
                break;

            case AIState.Panic:
                break;
        }
    }

    public virtual void ChangeState(AIState newState)
    {
        // Change the current state
        currentState = newState;

        // Log the time that this change happened
        lastStateChangeTime = Time.time;
    }

    public virtual void DoStationaryState()
    {
        // Do Nothing
    }

    public virtual void DoAttackState()
    {
        // Shoot
        //Debug.Log(CanSee(target));
        if (CanSee(target))
        {
            Shoot();
        }

    }

    public virtual void DoPanicState()
    {
        Flee();
    }

    public virtual void DoMovingState()
    {
        //Patrol();
    }


    public void Seek(GameObject target)
    {
        // Rotate Towards target
        //pawn.RotateTowards(target.transform.position);

        // Move forward
        //pawn.MoveForward();
    }

    public void Seek(Transform targetTransform)
    {
        // Rotate Towards target
        //pawn.RotateTowards(targetTransform.position);

        //pawn.MoveForward();
    }

    public void Seek(Pawn targetPawn)
    {
        //Seek(targetPawn.gameObject);
    }

    public void Seek(Vector3 targetPosition)
    {
        // Rotate Towards target
        //pawn.RotateTowards(targetPosition);

        //pawn.MoveForward();
    }

    protected bool IsDistanceLessThan(GameObject target, float distance)
    {
        if (Vector3.Distance(pawn.transform.position, target.transform.position) < distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Shoot()
    {
        // Tell the pawn to shoot
        pawn.Shoot();
    }

    protected void Flee()
    {
        float targetDistance = Vector3.Distance(target.transform.position, pawn.transform.position);
        float percentOfFleeDistance = targetDistance / fleeDistance;
        percentOfFleeDistance = Mathf.Clamp01(percentOfFleeDistance);
        float flippedPercentOfFleeDistance = 1 - percentOfFleeDistance;

        // Find vector to the target position
        Vector3 vectorToTarget = target.transform.position - pawn.transform.position;
        // Point target vector in the opposite direction
        Vector3 vectorAwayFromTarget = -vectorToTarget;
        // Find the vector we would travel down in order to flee
        Vector3 fleeVector = vectorAwayFromTarget.normalized * (fleeDistance * flippedPercentOfFleeDistance);
        // Seek the point that is "fleeVector" away from our current position
        Seek(pawn.transform.position + fleeVector);
    }

    public void TargetPlayerOne()
    {
        // If the gameManager exists
        if (GameManager.instance != null)
        {
            //Debug.Log("Found Game manager");
            // and the array of players exists
            if (GameManager.instance.players != null)
            {
                //Debug.Log("Found Player list");
                // ... and there are players in it
                if (GameManager.instance.players.Count > 0)
                {
                    //Debug.Log("Players in list");
                    // Then target the gameObject of the pawn of the first player controller in the list
                    target = GameManager.instance.players[0].pawn.gameObject;
                }
            }
        }
    }

    protected bool IsHasTarget()
    {
        // If we have a target
        if (target != null)
        {
            // If the target exists, return true
            if (GameObject.Find(target.gameObject.name) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    protected void TargetNearestTank()
    {
        // Get a list of all the tanks (pawns)
        Pawn[] allTanks = FindObjectsByType<Pawn>(FindObjectsSortMode.None);

        // Assume that the first tank is closest
        Pawn closestTank = allTanks[0];
        // If this is the first tank, skip it
        if (closestTank == this.pawn)
        {
            closestTank = allTanks[1];
        }
        float closestTankDistance = Vector3.Distance(pawn.transform.position, closestTank.transform.position);

        // Iterate through them one at a time
        foreach (Pawn tank in allTanks)
        {
            // If this one is closer than the closest
            if ((Vector3.Distance(pawn.transform.position, tank.transform.position) <= closestTankDistance) && (tank != this.pawn))
            {
                // It is the closest
                closestTank = tank;
                closestTankDistance = Vector3.Distance(pawn.transform.position, closestTank.transform.position);
            }
        }
        //Debug.Log(closestTank);
        // Target the closest tank
        target = closestTank.gameObject;
    }

    #region Senses

    public bool CanHear(GameObject Target)
    {
        // Get the target's noisemaker
        NoiseMaker noiseMaker = target.GetComponent<NoiseMaker>();

        // If they don't have one, they can't make noise, so return false
        if (noiseMaker == null)
        {
            return false;
        }

        //If they are making 0 noise, they can't be heard
        if (noiseMaker.volumeDistance <= 0)
        {
            //Debug.Log("noisMaker not making noise");
            return false;
        }

        // If they are making noise, add the volumeDistance in the noisemaker to the hearingdistance of this ai
        float totalDistance = noiseMaker.volumeDistance + hearingDistance;
        //Debug.Log(totalDistance);

        // If the distance between our pawn and the target is closer than this...
        if (Vector3.Distance(pawn.transform.position, target.transform.position) <= totalDistance)
        {
            // ... then we can hear the target
            //Debug.Log("Can hear " + target);
            return true;
        }
        else
        {
            // ... otherwise they are too far away
            return false;
        }
    }

    public bool CanSee(GameObject target)
    {
        //Debug.Log("Finding Target");
        // Find the vector from the agent to the target
        Vector3 agentToTargetVector = target.transform.position - pawn.transform.position;
        // Find the angle between the direction our agent is facing (forward in local space) and the vector to the target.
        float angleToTarget = Vector3.Angle(agentToTargetVector, pawn.transform.up);

        // Send that angle to the pawn so it can use it for shooting
        pawn.shotAngle = agentToTargetVector;

        // if that angle is less than our field of view
        if (angleToTarget < fieldOfView)
        {
            //Debug.Log("Target in FOV");

            // Raise the origin slightly for vision
            Vector3 rayOrigin = pawn.transform.position;
            rayOrigin.y = pawn.transform.lossyScale.y / 2;

            // Define our raycast hit
            RaycastHit hit;
            // Do a raycast then output the results to hit
            Physics.Raycast(rayOrigin, agentToTargetVector, out hit, maxVisionDistance);
            Debug.DrawRay(rayOrigin, agentToTargetVector, Color.white, 5.0f);

            // If we can see it
            if (hit.collider != null)
            {
                //Debug.Log("Hit " + hit.collider.gameObject);
                if (hit.collider.gameObject == target)
                {
                    //Debug.Log("Hit Player");
                    return true;
                }
                else
                {
                    //Debug.Log("Hit not the player");
                    return false;
                }
            }
            else
            {
                //Debug.Log("Hit Nothing");
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    #endregion
}
