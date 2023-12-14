using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateTransitionTester : MonoBehaviour
{
    public KeyCode TitleScreenKey;
    public KeyCode MainMenuKey;
    public KeyCode OptionsKey;
    public KeyCode CreditsKey;
    public KeyCode GameplayKey;
    public KeyCode GameOverKey;
    public KeyCode VictoryKey;
    public GameManager GameManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    public void ProcessInputs()
    {
        if (Input.GetKey(TitleScreenKey))
        {
            GameManager.ActivateTitleScreen();
        }
        if (Input.GetKey(MainMenuKey))
        {
            GameManager.ActivateMainMenuScreen();
        }
        if (Input.GetKey(OptionsKey))
        {
            GameManager.ActivateOptionsScreen();
        }
        if (Input.GetKey(CreditsKey))
        {
            GameManager.ActivateCreditsScreen();
        }
        if (Input.GetKeyDown(GameplayKey))
        {
            GameManager.ActivateGameplayScreen();
        }
        if (Input.GetKeyDown(GameOverKey))
        {
            GameManager.ActivateGameOverScreen();
        }
        if (Input.GetKeyDown(VictoryKey))
        {
            GameManager.ActivateVictoryScreen();
        }
    }
}
