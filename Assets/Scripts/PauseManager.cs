using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject PauseCanvas;
    public FirstPersonCharacterController Player;
    public Magnet0Raycaster Magnet0Raycaster;
    public KeyCode PauseKey;
    public bool Paused;
    private bool _showingCommands;
    private GameObject _instantiatedPauseCanvas;
    public GameObject CommandsPrefab;
    private GameObject _instantiatedCommandsPanel;
    private void Update()
    {
        if (Magnet0Raycaster.ShowingMenus()) return;
        if (Input.GetKeyDown(PauseKey))
        {
            if (!Paused)
                Pause();
            else
                UnPause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Paused)
            {
                Pause();
            }
            else
            {
                if (_showingCommands)
                {
                    HideCommands();
                }
                else
                {
                    UnPause();
                }
            }
        }
        else if (Paused && Input.GetMouseButtonDown(0) && !_showingCommands)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                UnPause();
            }
        }
    }

    private void Pause()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        Paused = true;
        Cursor.lockState = CursorLockMode.None;
        Player.IgnoreInput();
        _instantiatedPauseCanvas = Instantiate(PauseCanvas);
        var resumeButton = _instantiatedPauseCanvas.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        resumeButton.onClick.AddListener(UnPause);
        var commandsButton = _instantiatedPauseCanvas.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        commandsButton.onClick.AddListener(ShowCommands);
        var backMainMenuButton = _instantiatedPauseCanvas.transform.GetChild(0).GetChild(3).GetComponent<Button>();
        backMainMenuButton.onClick.AddListener(BackToMainMenu);
    }

    private void ShowCommands()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        _instantiatedCommandsPanel = Instantiate(CommandsPrefab, _instantiatedPauseCanvas.transform);
        _instantiatedCommandsPanel.SetActive(true);
        _showingCommands = true;
        var backButton = _instantiatedCommandsPanel.transform.GetChild(0).GetComponent<Button>();
        backButton.onClick.AddListener(HideCommands);
    }

    private void HideCommands()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        if (_instantiatedCommandsPanel != null)
            Destroy(_instantiatedCommandsPanel);
        _instantiatedCommandsPanel = null;
        _showingCommands = false;
    }

    private void BackToMainMenu()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        HouseManager.BackToMainMenu();
    }

    private void UnPause()
    {
        AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
        Paused = false;
        if (_instantiatedCommandsPanel != null)
            Destroy(_instantiatedCommandsPanel);
        if (_instantiatedPauseCanvas != null)
            Destroy(_instantiatedPauseCanvas);
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
    }
}
