using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject Commands;
    public GameObject Credits;
    public GameObject LoadingPanel;
    public void StartAdventure()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        LoadingPanel.SetActive(true);
        StartCoroutine(LoadScenePost());
    }

    private static IEnumerator LoadScenePost()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("ComputerDiscovery_Home");
    }

    public void QuitAdventure()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        Application.Quit();
    }

    public void ShowCommands()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        Commands.SetActive(true);
        var backButton = Commands.transform.GetChild(0).GetComponent<Button>();
        backButton.onClick.AddListener(HideCommands);
    }

    public void ShowCredits()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        Credits.SetActive(true);
        var backButton = Credits.transform.GetChild(0).GetComponent<Button>();
        backButton.onClick.AddListener(HideCredits);
    }

    private void HideCredits()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        Credits.SetActive(false);
        var backButton = Credits.transform.GetChild(0).GetComponent<Button>();
        backButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (Commands.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideCommands();
        }
        else if (Credits.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            HideCredits();
        }
    }

    private void HideCommands()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        Commands.SetActive(false);
        var backButton = Commands.transform.GetChild(0).GetComponent<Button>();
        backButton.onClick.RemoveAllListeners();
    }
}
