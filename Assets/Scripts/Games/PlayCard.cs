using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCard : MonoBehaviour
{
    public string unitName;
    public int unitValue;
    public int unitStrength;
    public string unitPlayer;

    public void ResetCard()
    {
        unitPlayer = null;
    }

    public void ResetSnell(string currentName)
    {
        unitName = currentName + 9;
        unitValue = 0;
        unitStrength = 3;
    }

    public void ResetBauer(string currentName)
    {
        unitName = currentName + "U";
        unitValue = 2;
        unitStrength = 5;
    }

    public void ChangeSnell(string trumpf)
    {
        unitName = trumpf + "Snell";
        unitValue = 14;
        unitStrength = 9;
    }

    public void ChangeBauer(string trumpf)
    {
        unitName = trumpf + "Bauer";
        unitValue = 20;
        unitStrength = 10;
    }

    public void SetPlayer(string player)
    {
        unitPlayer = player;
    }
}
