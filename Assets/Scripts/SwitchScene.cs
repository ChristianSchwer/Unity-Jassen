using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void NextScene(string mode)
    {
        SceneManager.LoadScene(mode);
        if (mode == "Multiplayer")
        {
            Debug.Log("Multi");
        }
    }

    public void HomeScene()
    {
        SceneManager.LoadScene("Login");
    }
}
