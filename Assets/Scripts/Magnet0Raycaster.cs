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
    private Transform _boxObjHolderT;


    // Update is called once per frame
    private void Update()
    {
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
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (EventSystem.current.currentSelectedGameObject == null)
                    {
                        Destroy(_objMenu);
                        _showingObjMenu = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
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
                    _previousPointedFile.TriggerLabel(false);
                }

                if (fileGrabber)
                {
                    _previousPointedFile = fileGrabber;
                    if (fileGrabber.Outlined)
                        fileGrabber.Outlined.OutlineWidth = 7f;
                    fileGrabber.TriggerLabel(true);
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (fileGrabber.GetReferred().GetParent() == Folder.TrashBin) return;
                        fileGrabber.Outlined.OutlineWidth = 0f;
                        _grabbedFile = fileGrabber;
                        _grabbedFile.GrabReferred(transform.Find("ObjHolder"));
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        _showingObjMenu = true;
                        if (fileGrabber.GetReferred().GetParent() == Folder.TrashBin)
                        {
                            _objMenu = fileGrabber.ShowTrashItemMenu(Player.transform);
                            var recover = _objMenu.transform.Find("RecoverButton").GetComponent<Button>();
                            recover.onClick.AddListener(delegate
                            {
                                Recover(fileGrabber);
                                Destroy(_objMenu);
                                _showingObjMenu = false;
                                Cursor.lockState = CursorLockMode.Locked;
                                Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                            });
                            var permDelete = _objMenu.transform.Find("PermDeleteButton").GetComponent<Button>();
                            permDelete.onClick.AddListener(delegate
                            {
                                PermDelete(fileGrabber);
                                Destroy(_objMenu);
                                _showingObjMenu = false;
                                Cursor.lockState = CursorLockMode.Locked;
                                Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                            });
                        }
                        else
                        {
                            _objMenu = fileGrabber.ShowObjectMenu(Player.transform);
                            var copyButton = _objMenu.transform.Find("CopyButton").GetComponent<Button>();
                            copyButton.onClick.AddListener(delegate
                            {
                                Copy(fileGrabber);
                                Destroy(_objMenu);
                                _showingObjMenu = false;
                                Cursor.lockState = CursorLockMode.Locked;
                                Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                            });
                            var renameButton = _objMenu.transform.Find("RenameButton").GetComponent<Button>();
                            renameButton.onClick.AddListener(delegate
                            {
                                _showingRenameMenu = true;
                                _renameMenu = fileGrabber.ShowRenameMenu(renameButton.transform.parent);
                                var cancelButton = _renameMenu.transform.Find("CancelButton").GetComponent<Button>();
                                cancelButton.onClick.AddListener(delegate
                                {
                                    Destroy(_renameMenu);
                                    _showingRenameMenu = false;
                                });
                                var confirmButton = _renameMenu.transform.Find("ConfirmButton").GetComponent<Button>();
                                confirmButton.onClick.AddListener(delegate
                                {
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
                            var deleteButton = _objMenu.transform.Find("DeleteButton").GetComponent<Button>();
                            deleteButton.onClick.AddListener(delegate
                            {
                                Delete(fileGrabber);
                                Destroy(_objMenu);
                                _showingObjMenu = false;
                                Cursor.lockState = CursorLockMode.Locked;
                                Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                            });
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
        }
        else
        {
            if (_previousPointedFile != null)
            {
                _previousPointedFile.Outlined.OutlineWidth = 0f;
                if (!_grabbedFile)
                    _previousPointedFile.TriggerLabel(false);
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
        }
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
        _grabbedFile.SetReferred(fileGrabber.GetReferred().GetACopy(true));
    }

    private void Delete(Grabber grabber)
    {
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
                Debug.Log("Non puoi lasciare qui l'elemento, non sei in una vera e propria cartella!");
                return;
            }
            _grabbedFile.DropInBox(Player.transform, _boxObjHolderT);
            _previousNetworkBox.FileInserted(_grabbedFile);
            //qui
            _grabbedFile = null;
            return;
        }
        _grabbedFile.DropReferred(Player.transform, roomIn, Explosion);
        _grabbedFile = null;
    }
}
