using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject PauseCanvas;
    public FirstPersonCharacterController Player;
    public Magnet0Raycaster Magnet0Raycaster;
    public KeyCode PauseKey;
    private bool _paused;
    private GameObject _instantiatedPauseCanvas;
    private void Update()
    {
        if (Magnet0Raycaster.ShowingMenus() || (!Input.GetKeyDown(PauseKey) && !Input.GetKeyDown(KeyCode.Escape))) return;
        if (!_paused)
            Pause();
        else
            UnPause();
    }

    private void Pause()
    {
        _paused = true;
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
        Debug.Log("Show commands");
    }

    private void BackToMainMenu()
    {
        Debug.Log("Go to main menu");
    }

    private void UnPause()
    {
        _paused = false;
        if (_instantiatedPauseCanvas != null)
            Destroy(_instantiatedPauseCanvas);
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
        Debug.Log("UnPause");
    }
}
