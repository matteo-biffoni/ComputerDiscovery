using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Magnet0Raycaster : MonoBehaviour
{
    public float RaycastDistance;
    
    public PlayerNavigatorManager Player;

    private Grabber _grabbedFile;

    private Grabber _previousPointedFile;

    public GameObject Explosion;

    private bool _showingObjMenu, _showingRenameMenu;
    private GameObject _objMenu, _renameMenu;
    private NetworkManager _previousNetworkManager;
    private NetworkBox _previousNetworkBox;
    private CarDownloader _previousCarDownloader;
    private ZipperHandler _previousZipperHandler;
    private Transform _boxObjHolderT;
    public GameObject CursorCanvas;

    public static bool Operating = true;

    public PauseManager PauseManager;

    public bool ShowingMenus()
    {
        return _showingObjMenu || _showingRenameMenu;
    }

    private void Awake()
    {
        Operating = true;
    }


    // Update is called once per frame
    private void Update()
    {
        if (!Operating || PauseManager.Paused) return;
        if (_showingObjMenu)
        {
            if (_showingRenameMenu)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Destroy(_renameMenu);
                    _showingRenameMenu = false;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                    {
                        Destroy(_objMenu);
                        _showingObjMenu = false;
                        _showingRenameMenu = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                        CursorCanvas.SetActive(true);
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Destroy(_objMenu);
                    _showingObjMenu = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                    CursorCanvas.SetActive(true);
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                    {
                        Destroy(_objMenu);
                        _showingObjMenu = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                        CursorCanvas.SetActive(true);
                    }
                }
            }
            return;
        }
        var t = transform;
        var ray = new Ray(t.position, t.forward);

        // Rilascio del file


        if (_grabbedFile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DropFile();
                return;
            }
        }
        if (Physics.Raycast(ray, out var hit, RaycastDistance))
        {
            if (!_grabbedFile)
            {
                var fileGrabber = hit.transform.GetComponent<Grabber>();
                if (_previousPointedFile != null && _previousPointedFile != fileGrabber)
                {
                    _previousPointedFile.Outlined.OutlineWidth = 0f;
                    _previousPointedFile.TriggerLabelRaycast(false);
                }

                if (fileGrabber)
                {
                    _previousPointedFile = fileGrabber;
                    if (fileGrabber.Outlined)
                        fileGrabber.Outlined.OutlineWidth = 7f;
                    fileGrabber.TriggerLabelRaycast(true);
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (fileGrabber.GetReferred().GetParent() == Folder.TrashBin) return;
                        fileGrabber.Outlined.OutlineWidth = 0f;
                        _grabbedFile = fileGrabber;
                        _grabbedFile.GrabReferred(transform.Find("ObjHolder"));
                    }
                    else if (Input.GetMouseButtonDown(1) && HouseManager.ActualQuest >= 2)
                    {
                        _showingObjMenu = true;
                        if (fileGrabber.GetReferred().GetParent() == Folder.TrashBin)
                        {
                            CursorCanvas.SetActive(false);
                            _objMenu = fileGrabber.ShowTrashItemMenu(Player.transform);
                            var recover = _objMenu.transform.GetChild(0).Find("RecoverButton").GetComponent<Button>();
                            recover.onClick.AddListener(delegate
                            {
                                AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
                                Recover(fileGrabber);
                                Destroy(_objMenu);
                                _showingObjMenu = false;
                                Cursor.lockState = CursorLockMode.Locked;
                                Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                            });
                            var permDelete = _objMenu.transform.GetChild(0).Find("PermDeleteButton").GetComponent<Button>();
                            permDelete.onClick.AddListener(delegate
                            {
                                AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
                                PermDelete(fileGrabber);
                                Destroy(_objMenu);
                                _showingObjMenu = false;
                                Cursor.lockState = CursorLockMode.Locked;
                                Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                            });
                            _objMenu.SetActive(true);
                        }
                        else
                        {
                            CursorCanvas.SetActive(false);
                            _objMenu = fileGrabber.ShowObjectMenu(Player.transform);
                            var copyButton = _objMenu.transform.GetChild(0).Find("CopyButton").GetComponent<Button>();
                            if (HouseManager.ActualQuest < 5)
                            {
                                ColorBlock cb = copyButton.colors;
                                cb.normalColor = Color.grey;
                                cb.highlightedColor = Color.grey;
                                cb.selectedColor = Color.grey;
                                cb.disabledColor = Color.gray;
                                copyButton.colors = cb;
                                copyButton.onClick.AddListener(delegate
                                {
                                    NotificationManager.Notify(Operation.LockedFunctionality);
                                });
                            }
                            else
                            {
                                copyButton.onClick.AddListener(delegate
                                {
                                    AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
                                    Copy(fileGrabber);
                                    Destroy(_objMenu);
                                    _showingObjMenu = false;
                                    Cursor.lockState = CursorLockMode.Locked;
                                    Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                                });
                            }
                            var renameButton = _objMenu.transform.GetChild(0).Find("RenameButton").GetComponent<Button>();
                            renameButton.onClick.AddListener(delegate
                            {
                                AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
                                _showingRenameMenu = true;
                                _renameMenu = fileGrabber.ShowRenameMenu(renameButton.transform.parent);
                                var cancelButton = _renameMenu.transform.Find("CancelButton").GetComponent<Button>();
                                cancelButton.onClick.AddListener(delegate
                                {
                                    AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
                                    Destroy(_renameMenu);
                                    _showingRenameMenu = false;
                                });
                                var confirmButton = _renameMenu.transform.Find("ConfirmButton").GetComponent<Button>();
                                confirmButton.onClick.AddListener(delegate
                                {
                                    AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
                                    var error = false;
                                    var fileNameInputField = _renameMenu.transform.Find("RenameFileNameInputField")
                                        .GetComponent<TMP_InputField>();
                                    var fileNameError = _renameMenu.transform.Find("FileNameError").gameObject;
                                    if (fileNameInputField.text.Trim().Equals("") ||
                                        fileNameInputField.text.Trim().Contains("."))
                                    {
                                        fileNameError.SetActive(true);
                                        error = true;
                                    }
                                    else
                                    {
                                        fileNameError.SetActive(false);
                                    }

                                    if (!error)
                                    {
                                        var newName = fileNameInputField.text.Trim();
                                        fileGrabber.Rename(newName);
                                        Destroy(_objMenu);
                                        _showingObjMenu = false;
                                        _showingRenameMenu = false;
                                        Cursor.lockState = CursorLockMode.Locked;
                                        Player.transform.GetComponent<FirstPersonCharacterController>()
                                            .ReactivateInput();
                                    }
                                });
                            });
                            var deleteButton = _objMenu.transform.GetChild(0).Find("DeleteButton").GetComponent<Button>();
                            if (HouseManager.ActualQuest < 5)
                            {
                                ColorBlock cb = deleteButton.colors;
                                cb.normalColor = Color.grey;
                                cb.highlightedColor = Color.grey;
                                cb.selectedColor = Color.grey;
                                cb.disabledColor = Color.gray;
                                deleteButton.colors = cb;
                                deleteButton.onClick.AddListener(delegate
                                {
                                    NotificationManager.Notify(Operation.LockedFunctionality);
                                });
                            }
                            else
                            {
                                deleteButton.onClick.AddListener(delegate
                                {
                                    AudioManager.Play(transform, AudioManager.Instance.OperationSound, false);
                                    Delete(fileGrabber);
                                    Destroy(_objMenu);
                                    _showingObjMenu = false;
                                    Cursor.lockState = CursorLockMode.Locked;
                                    Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                                });
                            }
                            _objMenu.SetActive(true);
                        }
                    }
                }
                if (hit.transform.CompareTag("CarDownloader") && !CarDownloader.FilesDownloaded)
                {
                    var carDownloader = hit.transform.GetComponent<CarDownloader>();
                    if (_previousCarDownloader != null && _previousCarDownloader != carDownloader)
                    {
                        if (_previousCarDownloader.GetActualRaycast())
                        {
                            _previousCarDownloader.SetActualRaycast(false);
                            _previousCarDownloader = null;
                            return;
                        }
                    }

                    if (carDownloader)
                    {
                        _previousCarDownloader = carDownloader;
                        if (!_previousCarDownloader.GetActualRaycast())
                        {
                            _previousCarDownloader.SetActualRaycast(true);
                        }
                    }
                }
            }
            else
            {
                if (hit.transform.CompareTag("Box"))
                {
                    if (_grabbedFile != null)
                    {
                        var networkBox = hit.transform.GetComponent<NetworkBox>();
                        if (_previousNetworkBox != null && _previousNetworkBox != networkBox)
                        {
                            if (_previousNetworkBox.GetActualRaycast())
                            {
                                _previousNetworkBox.SetActualRaycast(false);
                                _previousNetworkBox = null;
                                _boxObjHolderT = null;
                                return;
                            }
                        }
                        if (networkBox)
                        {
                            _previousNetworkBox = networkBox;
                            _boxObjHolderT = _previousNetworkBox.transform.Find("BoxObjHolder");
                            if (!_previousNetworkBox.GetActualRaycast())
                            {
                                _previousNetworkBox.SetActualRaycast(true);
                            }
                        }
                    }
                }
            }
            if (hit.transform.CompareTag("AD5L"))
            {
                var networkManager = hit.transform.GetComponent<NetworkManager>();
                if (_previousNetworkManager != null && _previousNetworkManager != networkManager)
                {
                    if (_previousNetworkManager.GetActualRaycast())
                    {
                        _previousNetworkManager.SetActualRaycast(false);
                        _previousNetworkManager = null;
                        return;
                    }
                }
                if (networkManager)
                {
                    _previousNetworkManager = networkManager;
                    if (!_previousNetworkManager.GetActualRaycast())
                    {
                        _previousNetworkManager.SetActualRaycast(true);
                    }
                }
            }
            else if (hit.transform.CompareTag("Zipper"))
            {
                var zipperHandler = hit.transform.GetComponent<ZipperHandler>();
                if (_previousZipperHandler != null && _previousZipperHandler != zipperHandler)
                {
                    if (_previousZipperHandler.GetActualRaycast())
                    {
                        _previousZipperHandler.SetActualRaycast(false, null);
                        _previousZipperHandler = null;
                        return;
                    }
                }

                if (zipperHandler)
                {
                    _previousZipperHandler = zipperHandler;
                    if (!_previousZipperHandler.GetActualRaycast())
                    {
                        _previousZipperHandler.SetActualRaycast(true, _grabbedFile);
                    }
                }
            }
        }
        else
        {
            if (_previousPointedFile != null)
            {
                _previousPointedFile.Outlined.OutlineWidth = 0f;
                if (!_grabbedFile)
                    _previousPointedFile.TriggerLabelRaycast(false);
                _previousPointedFile = null;
            }

            if (_previousNetworkManager != null)
            {
                if (_previousNetworkManager.GetActualRaycast())
                {
                    _previousNetworkManager.SetActualRaycast(false);
                    _previousNetworkManager = null;
                }
            }

            if (_previousNetworkBox != null)
            {
                if (_previousNetworkBox.GetActualRaycast())
                {
                    _previousNetworkBox.SetActualRaycast(false);
                    _previousNetworkBox = null;
                    _boxObjHolderT = null;
                }
            }

            if (_previousCarDownloader != null)
            {
                if (_previousCarDownloader.GetActualRaycast())
                {
                    _previousCarDownloader.SetActualRaycast(false);
                    _previousCarDownloader = null;
                }
            }

            if (_previousZipperHandler != null)
            {
                if (_previousZipperHandler.GetActualRaycast())
                {
                    _previousZipperHandler.SetActualRaycast(false, null);
                    _previousZipperHandler = null;
                }
            }
        }
    }

    public void SetGrabbedFile(Grabber grabber)
    {
        _grabbedFile = grabber;
    }

    private void Recover(Grabber grabber)
    {
        grabber.Recover();
    }

    private void PermDelete(Grabber grabber)
    {
        grabber.PermDelete();
    }

    private void Copy(Grabber fileGrabber)
    {
        fileGrabber.Outlined.OutlineWidth = 0f;
        _grabbedFile = fileGrabber.Copy(transform.Find("ObjHolder"));
        _grabbedFile.SetReferred(fileGrabber.GetReferred().GetACopy());
    }

    private void Delete(Grabber grabber)
    {
        if (Folder.TrashBin.GetFiles().Count >= 5)
        {
            NotificationManager.Notify(Operation.NoSpaceInTrashBin);
            return;
        }
        grabber.Delete();
    }

    private void DropFile()
    {
        if (!_grabbedFile) return;
        // Trovare la stanza in cui si trova il giocatore (ricordarsi che qui siamo nella camera, non in magnet0
        var roomIn = Player.GetRoomIn();
        // Una volta trovata la stanza prendere la bacheca di quella stanza e aggiungere correttamente il File
        // Se ci troviamo in MainRoom fare in modo che venga messo sulla bacheca del desktop
        if (Folder.MainRoom == roomIn)
        {
            roomIn = Folder.Root;
        }
        else if (Folder.Garage == roomIn)
        {
            if (_previousNetworkBox == null)
            {
                NotificationManager.Notify(Operation.ReleaseNotAllowed);
                return;
            }
            if (_grabbedFile.GetReferred().IsACopy())
            {
                if (_grabbedFile.GetReferred() is RoomFile roomFile)
                {
                    if (HouseManager.ActualQuest == 5 && DialogueTrigger.FifthQuestInstantiation && roomFile.IsACopyOf(RoomFile.ScoperteFile))
                    {
                        NetworkManager.SendingScoperte = true;
                        _grabbedFile.DropInBox(Player.transform, _boxObjHolderT);
                        _previousNetworkBox.FileInserted(_grabbedFile);
                        _grabbedFile = null;
                    }
                    else if (HouseManager.ActualQuest == 5 && DialogueTrigger.FifthQuestInstantiation)
                    {
                        NotificationManager.Notify(Operation.ShouldBringScoperte);
                    }
                    else if (HouseManager.ActualQuest < 5 || !DialogueTrigger.FifthQuestInstantiation)
                    {
                        NotificationManager.Notify(Operation.LockedFunctionality);
                    }
                    else if (HouseManager.ActualQuest == 6 && DialogueTrigger.SixthQuestInstantiation &&
                             roomFile.IsZipOf(Folder.ImmaginiEVideoFolder))
                    {
                        NetworkManager.SendingImmaginiEVideoFolder = true;
                        _grabbedFile.DropInBox(Player.transform, _boxObjHolderT);
                        _previousNetworkBox.FileInserted(_grabbedFile);
                        _grabbedFile = null;
                    }
                    else if (HouseManager.ActualQuest == 6 && DialogueTrigger.SixthQuestInstantiation)
                    {
                        NotificationManager.Notify(Operation.ShouldBringImmaginiEVideoFolder);
                    }
                    else if (HouseManager.ActualQuest > 6)
                    {
                        _grabbedFile.DropInBox(Player.transform, _boxObjHolderT);
                        _previousNetworkBox.FileInserted(_grabbedFile);
                        _grabbedFile = null;
                    }
                    return;
                }
                if (_grabbedFile.GetReferred() is Folder)
                {
                    if (HouseManager.ActualQuest == 6 && DialogueTrigger.SixthQuestInstantiation)
                    {
                        _grabbedFile.DropInBox(Player.transform, _boxObjHolderT);
                        _previousNetworkBox.FileInserted(_grabbedFile);
                        //qui
                        _grabbedFile = null;
                    }
                    else if (HouseManager.ActualQuest < 6 || !DialogueTrigger.SixthQuestInstantiation)
                    {
                        NotificationManager.Notify(Operation.LockedFunctionality);
                    }
                    return;
                }
                return;
            }
            
            NotificationManager.Notify(Operation.ReleaseIONotCopy);
            return;
        }
        if (roomIn != null)
        {
            switch (_grabbedFile.GetReferred())
            {
                case Folder:
                    if (roomIn.GetChildren().Count == Folder.MaxNumberOfSubfolders)
                    {
                        NotificationManager.Notify(Operation.FolderFullOfSubfolders);
                        return;
                    }
                    break;
                case RoomFile:
                    if (roomIn.GetFiles().Count == Folder.MaxNumberOfFilesPerFolder)
                    {
                        NotificationManager.Notify(Operation.FolderFullOfFiles);
                        return;
                    }
                    break;
            }
        }
        _grabbedFile.DropReferred(Player.transform, roomIn, Explosion);
        _grabbedFile = null;
    }
}
