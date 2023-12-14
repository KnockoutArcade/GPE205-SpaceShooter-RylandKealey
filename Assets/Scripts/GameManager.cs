using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The static instance of this class - there can only be one.
    /// </summary>
    public static GameManager instance;

    /// <summary>
    /// List of Player controllers (not the actual pawns)
    /// </summary>
    public List<PlayerController> players;

    /// <summary>
    /// List of all enemies in the scene
    /// </summary>
    public List<AIController> enemies;

    // Prefabs
    public GameObject playerControllerPrefab;
    public GameObject player2ControllerPrefab;
    public GameObject[] enemyControllerPrefab;
    public int enemyControllerPrefabIndex;
    public GameObject playerPawnPrefab;
    public GameObject[] enemyPawnPrefab;
    public int enemyPawnPrefabIndex;
    // Array of Player Spawns
    public PlayerSpawnPoint[] playerSpawnTransforms;
    public int randomPlayerSpawn;
    // List of enemy spawns
    public EnemySpawnPoint[] enemySpawnTransforms;
    public int randomEnemySpawn;
    // Enable Multiplayer
    public bool enableMultiplayer = false;
    // Enable Endless Mode
    public bool enableEndless = false;

    // Spawning Behavior
    public int enemiesToSpawn = 1; // amount of enemies to spawn
    public int maxEnemyCount = 10; // The maximum amount of enemies that are allowed to be spawned at once
    public int wavesCompleted = 0; // number of waves completed
    public bool isCurrentlySpawningEnemies = false; // whether the gameManager is currently spawning enemies
    public float spawningTime = 0.0f; // The moment in time when the GameManager spawned the previous clump of enemies
    public float spawningWaitTime = 2.0f; // the amount of time to wait (in seconds) between spawning each clump of enemies

    // UI
    public TextMeshProUGUI scoreDisplay; // The score that the player has while playing the game
    public TextMeshProUGUI wavesCompletedDisplay; // Shows how many waves the player has currently completed
    public TextMeshProUGUI finalScoreGameOverDisplay; // The final score stat that shows how many points the player has on Game Over
    public TextMeshProUGUI finalScoreVictoryDisplay; // The final score stat that shows how many points the player has on Victory
    public TextMeshProUGUI finalWavesCompletedDisplay; // The final stat that shows how many waves the player completed (only displayed on Game Over)

    public Image extraLifeImage_1; // The first extra life image
    public Image extraLifeImage_2; // The second extra life image

    // Music
    public AudioSource MenuMusicAudioSource; // The Audio Source that plays the music in menus
    public AudioSource GameplayMusicAudioSource; // The Audio Source that plays the music during gameplay

    // Game States
    public GameObject TitleScreenStateObject;
    public GameObject MainMenuStateObject;
    public GameObject OptionsScreenStateObject;
    public GameObject CreditsScreenStateObject;
    public GameObject GameplayStateObject;
    public GameObject GameOverScreenStateObject;
    public GameObject VictoryScreenStateObject;
    #endregion

    private void Awake()
    {
        // Ensure that this is the only gameManager in the scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Allocate Memory for player list
        players = new List<PlayerController>();
        // Allocate Memory for enemy list
        enemies = new List<AIController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ActivateTitleScreen();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for any player deaths
        if (players.Count > 0)
        {
            foreach (PlayerController pc in players)
            {
                // If a player loses all of their lives...
                if (!pc.pawnIsAlive && pc.lives <= 0)
                {
                    UpdateScore();
                    ActivateGameOverScreen();
                }

                // If a player loses 1 life...
                if (!pc.pawnIsAlive && pc.lives > 0)
                {
                    RespawnPlayer(pc);
                    UpdateScore();
                }
            }
        }

        // If we are in the gameplay mode, handle spawning enemies
        if (GameplayStateObject.activeSelf)
        {
            // If all enemies have been defeated AND we are not in the middle of spawning enemies, advance to the next wave
            if (enemies.Count <= 0 && !isCurrentlySpawningEnemies)
            {
                AdvanceWaveStage();
                UpdateScore();
            }

            // Handle Spawning enemies
            // Since there are only 5 enemy spawners, if the game wants to spawn more than 5 enemies in a wave, then the GM will split the wave into clumps of 5, and spawn those at
            // regular intervals
            if (isCurrentlySpawningEnemies && Time.time >= spawningTime + spawningWaitTime)
            {
                // Spawn a clump of enemies
                SpawnEnemyClump(enemiesToSpawn);

                // If we have run out of enemies to spawn, then stop spawning enemies
                if (enemiesToSpawn <= 0)
                {
                    isCurrentlySpawningEnemies = false;
                }
            }
        }
    }

    public void SpawnPlayer(GameObject controllerType)
    {
        // Spawn the Player Controller at (0, 0, 0) with no rotation
        GameObject newPlayerObj = Instantiate(controllerType, Vector3.zero, Quaternion.identity);

        // Find all the spawnPoints in the level
        playerSpawnTransforms = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        randomPlayerSpawn = 0;

        // Spawn the Pawn and connect it to that controller
        GameObject newPawnObj = Instantiate(playerPawnPrefab, playerSpawnTransforms[randomPlayerSpawn].transform.position, playerSpawnTransforms[randomPlayerSpawn].transform.rotation);

        // Get the PlayerController component and Pawn component
        PlayerController newController = newPlayerObj.GetComponent<PlayerController>();
        Pawn newPawn = newPawnObj.GetComponent<Pawn>();

        // Hook them up
        newController.pawn = newPawn;
        newPawn.playerController = newController;

        // Set the player score to 0
        newController.score = 0;
    }

    public void RespawnPlayer(PlayerController pc)
    {
        // Find all the spawnPoints in the level
        playerSpawnTransforms = FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        randomPlayerSpawn = UnityEngine.Random.Range(0, playerSpawnTransforms.Length - 1);

        // Spawn the Pawn and connect it to that controller
        GameObject newPawnObj = Instantiate(playerPawnPrefab, playerSpawnTransforms[randomPlayerSpawn].transform.position, playerSpawnTransforms[randomPlayerSpawn].transform.rotation);

        // Get the Pawn component
        Pawn newPawn = newPawnObj.GetComponent<Pawn>();

        // Hook them up
        pc.pawn = newPawn;
        newPawn.playerController = pc;

        // Give the new pawn invincibilty
        newPawn.isInvincible = true;
        newPawn.invincibilityTime += newPawn.invincibilityGain;

        // Reduce Lives by 1
        pc.lives--;

        // Reset pawn alive check
        pc.pawnIsAlive = true;

        // Update Score
        //UpdateScore();
    }

    public void SpawnEnemyClump(int amount)
    {
        // Store when we have spawned this clump of enemies
        spawningTime = Time.time;
        
        // Find all of the enemy spawn Points in the level
        enemySpawnTransforms = FindObjectsByType<EnemySpawnPoint>(FindObjectsSortMode.None);

        // Put the array into a list to make it easier to access
        List<EnemySpawnPoint> validSpawnPoints = new List<EnemySpawnPoint>();

        foreach (EnemySpawnPoint spawnPoint in enemySpawnTransforms)
        {
            validSpawnPoints.Add(spawnPoint);
        }

        // Choose a random spawn point index
        randomEnemySpawn = UnityEngine.Random.Range(0, validSpawnPoints.Count - 1);

        int i = 0;
        // For the number of enemies we want to spawn
        while (i < amount)
        {
            // If we have run out of valid spawn points OR if there are 10 or more enemies on screen, break
            if (validSpawnPoints.Count == 0 || enemies.Count >= 10)
            {
                break;
            }

            // Choose a random pawn type
            enemyControllerPrefabIndex = UnityEngine.Random.Range(0, enemyControllerPrefab.Length);

            // Spawn an enemy controller
            GameObject newEnemyController = Instantiate(enemyControllerPrefab[enemyControllerPrefabIndex], Vector3.zero, Quaternion.identity);

            // Spawn the Pawn
            GameObject newEnemyObj = Instantiate(enemyPawnPrefab[enemyControllerPrefabIndex], validSpawnPoints[randomEnemySpawn].transform.position, validSpawnPoints[randomEnemySpawn].transform.rotation);

            // remove the transform we just used
            validSpawnPoints.Remove(validSpawnPoints[randomEnemySpawn]);
            randomEnemySpawn = UnityEngine.Random.Range(0, validSpawnPoints.Count - 1);

            // Get the PlayerController component and Pawn component
            Controller newController = newEnemyController.GetComponent<Controller>();
            Pawn newPawn = newEnemyObj.GetComponent<Pawn>();

            // Hook them up
            newController.pawn = newPawn;

            // Increment the while loop
            i++;

            // Decrease the amount of enemies to spawn
            enemiesToSpawn -= 1;
        }
    }

    public void AdvanceWaveStage() 
    {
        // Increase the amount of waves the player has completed
        wavesCompleted += 1;

        // Once we have completed enough waves, end the game
        if (wavesCompleted >= 20 && !enableEndless)
        {
            ActivateVictoryScreen();
        }
        else
        {
            // Increase the number of enemies to spawn
            enemiesToSpawn = wavesCompleted + 1;


            // Tell the game we are currently spawning enemies
            isCurrentlySpawningEnemies = true;
        }
        
    }

    #region Gameplay State Scripts

    private void DeactivateAllGameStates()
    {
        // Deactivate all game states
        TitleScreenStateObject.SetActive(false);
        MainMenuStateObject.SetActive(false);
        OptionsScreenStateObject.SetActive(false);
        CreditsScreenStateObject.SetActive(false);
        GameplayStateObject.SetActive(false);
        GameOverScreenStateObject.SetActive(false);
        VictoryScreenStateObject.SetActive(false);

        // Destroy the game world
        DestroyExistingMap();

        // Stop the gameplay music
        GameplayMusicAudioSource.Stop();

    }

    public void ActivateTitleScreen()
    {
        // First, make sure all other game states are deactivated
        DeactivateAllGameStates();

        // Then, make the title screen object activate
        TitleScreenStateObject.SetActive(true);

        // TODO : Whatever needs to happen when activating the title screen
    }
    public void ActivateMainMenuScreen()
    {
        // First, make sure all other game states are deactivated
        DeactivateAllGameStates();

        // Then, make the screen object activate
        MainMenuStateObject.SetActive(true);

        // Reset the score UI
        scoreDisplay.text = "Score: 0";
        finalScoreGameOverDisplay.text = "Score: 0";
        finalScoreVictoryDisplay.text = "Score: 0";

        // Reset Waves Completed
        wavesCompletedDisplay.text = "Waves: 1 / 20";
        finalWavesCompletedDisplay.text = "Waves Completed: 1 / 20";

        // Re-show all the lives
        extraLifeImage_1.enabled = true;
        extraLifeImage_2.enabled = true;
    }
    public void ActivateOptionsScreen()
    {
        // First, make sure all other game states are deactivated
        DeactivateAllGameStates();

        // Then, make the screen object activate
        OptionsScreenStateObject.SetActive(true);

        // TODO : Whatever needs to happen when activating the screen
    }
    public void ActivateCreditsScreen()
    {
        // First, make sure all other game states are deactivated
        DeactivateAllGameStates();

        // Then, make the screen object activate
        CreditsScreenStateObject.SetActive(true);

        // TODO : Whatever needs to happen when activating the screen
    }
    public void ActivateGameplayScreen()
    {
        // First, make sure all other game states are deactivated
        DeactivateAllGameStates();

        // Then, make the screen object activate
        GameplayStateObject.SetActive(true);

        // Stop the menu music
        MenuMusicAudioSource.Stop();

        // Play the Gameplay Music
        GameplayMusicAudioSource.Play();

        // Destroy anyting in the game world that still exists
        DestroyExistingMap();

        // Allocate Memory for player list
        players = new List<PlayerController>();
        // Allocate Memory for enemy list
        enemies = new List<AIController>();

        // Reset enemy spawning variables
        enemiesToSpawn = 1; // amount of enemies to spawn
        wavesCompleted = 0; // number of waves completed
        isCurrentlySpawningEnemies = false; // whether the gameManager is currently spawning enemies

        // Spawn the player
        SpawnPlayer(playerControllerPrefab);

        // Update Score
        //UpdateScore();
}
    public void ActivateGameOverScreen()
    {
        // First, make sure all other game states are deactivated
        DeactivateAllGameStates();

        // Then, make the screen object activate
        GameOverScreenStateObject.SetActive(true);

        // Reset Player List
        players = new List<PlayerController>();
        // Reset Enemies List
        enemies = new List<AIController>();

        // Start playing menu music
        MenuMusicAudioSource.Play();

    }
    public void ActivateVictoryScreen()
    {
        // First, make sure all other game states are deactivated
        DeactivateAllGameStates();

        // Then, make the screen object activate
        VictoryScreenStateObject.SetActive(true);

        // Update the final score
        //finalScore.text = "" + players[0].score;

        // Reset Player List
        players = new List<PlayerController>();
        // Reset Enemies List
        enemies = new List<AIController>();

        // Start playing menu music
        MenuMusicAudioSource.Play();

    }

    #endregion

    public void DestroyExistingMap()
    {
        // Destroy anything that is set to be Destroyed upon loading a new map
        DestroyOnNewmap[] objectsToDestroy = FindObjectsByType<DestroyOnNewmap>(FindObjectsSortMode.None);
        if (objectsToDestroy.Length > 0)
        {
            foreach (DestroyOnNewmap a in objectsToDestroy)
            {
                Destroy(a.gameObject);
            }
        }
    }

    public void UpdateScore()
    {
        // Update player 1's ui
        scoreDisplay.text = "Score: " + players[0].score;
        finalScoreGameOverDisplay.text = "Score: " + players[0].score;
        finalScoreVictoryDisplay.text = "Score: " + players[0].score;

        // Slightly change how the waves are displayed if the player is playing in endless or not
        if (!enableEndless)
        {
            wavesCompletedDisplay.text = "Waves: " + wavesCompleted + " / 20";
            finalWavesCompletedDisplay.text = "Waves Completed: " + wavesCompleted + " / 20";
        }
        else
        {
            wavesCompletedDisplay.text = "Waves: " + wavesCompleted;
            finalWavesCompletedDisplay.text = "Waves Completed: " + wavesCompleted;
        }
        
        // Update Lives Display
        if (players[0].lives >= 2)
        {
            extraLifeImage_1.enabled = true;
            extraLifeImage_2.enabled = true;
        }
        else if (players[0].lives == 1)
        {
            extraLifeImage_1.enabled = true;
            extraLifeImage_2.enabled = false;
        }
        else if (players[0].lives <= 0)
        {
            extraLifeImage_1.enabled = false;
            extraLifeImage_2.enabled = false;
        }
    }
}
