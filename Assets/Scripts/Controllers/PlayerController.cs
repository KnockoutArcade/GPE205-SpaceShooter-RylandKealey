using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    // Controls
    public KeyCode moveRightKey;
    public KeyCode moveLeftKey;
    public KeyCode moveUpKey;
    public KeyCode moveDownKey;
    public KeyCode shootKey;
    public KeyCode layMineKey;

    // Movement Directions
    private float moveDirX;
    private float moveDirZ;

    // Score
    public int score;

    // Lives
    public int lives = 2;

    // Whether or not the pawn this controls is currently alive
    public bool pawnIsAlive = true;


    // Start is called before the first frame update
    public override void Start()
    {
        // If manager exists
        {
            if (GameManager.instance != null)
            {
                // And if it can track players
                if (GameManager.instance.players != null)
                {
                    // Register it to the GameManager
                    GameManager.instance.players.Add(this);
                }
            }
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        // Check to see if the pawn we're controlling is still alive
        if (pawn == null)
        {
            pawnIsAlive = false;
        }
        else
        {
            // If it is, process inputs
            ProcessInputs();
        }
        
    }

    public void ProcessInputs()
    {
        if (Input.GetKey(moveRightKey))
        {
            moveDirX = 1.0f;
        }
        else if (Input.GetKey(moveLeftKey))
        {
            moveDirX = -1.0f;
        }
        else
        {
            moveDirX = 0.0f;
        }

        if (Input.GetKey(moveUpKey))
        {
            moveDirZ = 1.0f;
        }
        else if (Input.GetKey(moveDownKey))
        {
            moveDirZ = -1.0f;
        }
        else
        {
            moveDirZ = 0.0f;
        }

        if (Input.GetKeyDown(shootKey))
        {
            pawn.Shoot();
        }
        if (Input.GetKeyDown(layMineKey))
        {
            //pawn.LayMine();
        }

        pawn.Move(moveDirX, moveDirZ);
    }

    public void AddToScore(int amount)
    {
        score += amount;

        // If manager exists
        {
            if (GameManager.instance != null)
            {
                // Update the score in the UI
                GameManager.instance.UpdateScore();
            }
        }
    }
}
