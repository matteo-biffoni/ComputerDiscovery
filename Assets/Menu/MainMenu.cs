using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartAdventure()
    {
        SceneManager.LoadScene("ComputerDiscovery_Home"); //index 0
    }

    public void QuitAdventure()
    {
        Application.Quit();
    }
}
