using UnityEngine;
using UnityEngine.UI;

public class Magnet0Raycaster : MonoBehaviour
{
    public float RaycastDistance;
    
    public PlayerNavigatorManager Player;

    private Grabber _grabbedFile;

    private Grabber _previousPointedFile;

    public GameObject Explosion;

    private bool _showingObjMenu;

    // Update is called once per frame
    private void Update()
    {
        if (_showingObjMenu) return;
        var t = transform;
        var ray = new Ray(t.position, t.forward);

        // Rilascio del file


        if (_grabbedFile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DropFile();
            }
            return;
        }
        if (Physics.Raycast(ray, out var hit, RaycastDistance))
        {
            var fileGrabber = hit.transform.GetComponent<Grabber>();
            if (_previousPointedFile != null && _previousPointedFile != fileGrabber)
            {
                _previousPointedFile.Outlined.OutlineWidth = 0f;
                _previousPointedFile.TriggerLabel(false, null);
            }
            if (fileGrabber)
            {
                _previousPointedFile = fileGrabber;
                if (fileGrabber.Outlined)
                    fileGrabber.Outlined.OutlineWidth = 7f;
                fileGrabber.TriggerLabel(true, Player.transform.GetComponentInChildren<Camera>().transform);
                if (Input.GetMouseButtonDown(0))
                {
                    if (fileGrabber.GetReferred().GetParent() == Folder.TrashBin) return;
                    fileGrabber.Outlined.OutlineWidth = 0f;
                    _grabbedFile = fileGrabber;
                    _grabbedFile.GrabReferred(Player.transform.GetComponentInChildren<Camera>().transform, transform.Find("ObjHolder"));
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    _showingObjMenu = true;
                    if (fileGrabber.GetReferred().GetParent() == Folder.TrashBin)
                    {
                        var menu = fileGrabber.ShowTrashItemMenu(Player.transform);
                        var recover = menu.transform.Find("RecoverButton").GetComponent<Button>();
                        recover.onClick.AddListener(delegate
                        {
                            Recover(fileGrabber);
                            Destroy(menu);
                            _showingObjMenu = false;
                            Cursor.lockState = CursorLockMode.Locked;
                            Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                        });
                        var permDelete = menu.transform.Find("PermDeleteButton").GetComponent<Button>();
                        permDelete.onClick.AddListener(delegate
                        {
                            PermDelete(fileGrabber);
                            Destroy(menu);
                            _showingObjMenu = false;
                            Cursor.lockState = CursorLockMode.Locked;
                            Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                        });
                    }
                    else
                    {
                        var menu = fileGrabber.ShowObjectMenu(Player.transform);
                        var copyButton = menu.transform.Find("CopyButton").GetComponent<Button>();
                        copyButton.onClick.AddListener(delegate
                        {
                            Copy(fileGrabber);
                            Destroy(menu);
                            _showingObjMenu = false;
                            Cursor.lockState = CursorLockMode.Locked;
                            Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                        });
                        var deleteButton = menu.transform.Find("DeleteButton").GetComponent<Button>();
                        deleteButton.onClick.AddListener(delegate
                        {
                            Delete(fileGrabber);
                            Destroy(menu);
                            _showingObjMenu = false;
                            Cursor.lockState = CursorLockMode.Locked;
                            Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
                        });
                    }
                }
            }
        }
        else if (_previousPointedFile != null)
        {
            _previousPointedFile.Outlined.OutlineWidth = 0f;
            _previousPointedFile.TriggerLabel(false, null);
            _previousPointedFile = null;
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
        _grabbedFile = fileGrabber.Copy(Player.transform.GetComponentInChildren<Camera>().transform, transform.Find("ObjHolder"));
        _grabbedFile.SetReferred(fileGrabber.GetReferred().GetACopy());
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
        _grabbedFile.DropReferred(Player.transform, roomIn, Explosion);
        _grabbedFile = null;
    }
}
