using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreenCheckboxes : MonoBehaviour
{
    public Toggle endlessCheckbox;

    public void SetEndlessStatus()
    {
        GameManager.instance.enableEndless = endlessCheckbox.isOn;
    }
}
