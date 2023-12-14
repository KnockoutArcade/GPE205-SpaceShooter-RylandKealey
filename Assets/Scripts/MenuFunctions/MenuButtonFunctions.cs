using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonFunctions : MonoBehaviour
{
    // This class contains functions for all of the different UI buttons in the game

    public void ChangeToMainMenu()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ActivateMainMenuScreen();
        }
    }
    public void ChangeToCreditsMenu()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ActivateCreditsScreen();
        }
    }

    public void ChangeToOptionsMenu()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ActivateOptionsScreen();
        }
    }

    public void ChangeToGameplay()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ActivateGameplayScreen();
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
