using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateFolderManager : MonoBehaviour
{
    public Transform Player;
    private FirstPersonCharacterController _fps;
    private PlayerNavigatorManager _pnm;
    public KeyCode NewFolderKey;
    public GameObject CreateFolderCanvas;
    private bool _showingMenu;
    private GameObject _menu;
    public static bool EnabledByQuest;
    
    private void Start()
    {
        _fps = Player.transform.GetComponent<FirstPersonCharacterController>();
        _pnm = Player.transform.GetComponent<PlayerNavigatorManager>();
    }

    private void GetLockAndShowMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        _fps.IgnoreInput();
        _menu = Instantiate(CreateFolderCanvas);
        _showingMenu = true;
        var cancelButton = _menu.transform.GetChild(0).Find("CancelButton").GetComponent<Button>();
        cancelButton.onClick.AddListener(GoBackToGame);
        var confirmButton = _menu.transform.GetChild(0).Find("CreateButton").GetComponent<Button>();
        confirmButton.onClick.AddListener(delegate
        {
            bool error;
            var folderNameInputField =
                _menu.transform.GetChild(0).Find("NewFolderInputField").GetComponent<TMP_InputField>();
            var folderNameError = _menu.transform.GetChild(0).Find("NewFolderError").gameObject;
            if (folderNameInputField.text.Trim().Equals("") || folderNameInputField.text.Trim().Contains("."))
            {
                folderNameError.SetActive(true);
                error = true;
            }
            else
            {
                folderNameError.SetActive(false);
                error = false;
            }

            if (!error)
            {
                var newFolderName = folderNameInputField.text.Trim();
                _pnm.GetRoomIn().InsertFileOrFolder(new Folder(newFolderName, null), false);
                NotificationManager.Notify(Operation.FolderCreated);
                GoBackToGame();
            }
        });
    }

    private void GoBackToGame()
    {
        Destroy(_menu);
        _showingMenu = false;
        Cursor.lockState = CursorLockMode.Locked;
        _fps.ReactivateInput();
    }

    private void Update()
    {
        if (!_showingMenu)
        {
            if (Input.GetKeyDown(NewFolderKey))
            {
                if (_fps.IsMagnet0Free() && _pnm.CanCreateFolder() && EnabledByQuest)
                {
                    GetLockAndShowMenu();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBackToGame();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    GoBackToGame();
                }
            }
        }
    }
}
