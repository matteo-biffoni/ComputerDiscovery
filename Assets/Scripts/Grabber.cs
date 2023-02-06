using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable Unity.InefficientPropertyAccess


public class Grabber : MonoBehaviour
{
    private Grabbable _file;
    private Transform _player;
    private Folder _destinationRoom;
    private GameObject _explosion;
    private bool _labelVisibility;
    private TMP_Text _fileNameTextRaycast;
    private TMP_Text _fileNameTextGrabbed;
    private Transform _bachecaLookAt;
    public Outline Outlined;
    [FormerlySerializedAs("ObjMenuCanvas")] public GameObject ObjMenuCanvasPrefab;
    public GameObject TrashItemCanvasPrefab;
    public GameObject RenameCanvasPrefab;

    private void Start()
    {
        if (Outlined == null) Outlined = GetComponent<Outline>();
        _fileNameTextRaycast = GameObject.FindGameObjectWithTag("FileNameRaycast").transform.GetComponent<TMP_Text>();
        _fileNameTextGrabbed = GameObject.FindGameObjectWithTag("GrabbedFileText").transform.GetComponent<TMP_Text>();
    }

    public void SetImage(Sprite sprite)
    {
        if (_file is RoomFile roomFile && (roomFile.GetFormat() == "png" || roomFile.GetFormat() == "jpeg"))
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    private IEnumerator AnimationAfterDrop()
    {
        var previousPlayerRotation = _player.rotation;
        Transform playerTransform;
        var previousCameraRotation = (playerTransform = _player).GetComponentInChildren<Camera>().transform.localRotation;
        var direction = (_bachecaLookAt.position - playerTransform.position).normalized;
        var cameraT = _player.GetComponentInChildren<Camera>().transform;
        var lookRotationCamera = Quaternion.LookRotation(direction);
        direction.y = 0;
        var lookRotation = Quaternion.LookRotation(direction);
        var localRotation = cameraT.localRotation;
        var cameraTo = Quaternion.Euler(new Vector3(lookRotationCamera.eulerAngles.x, localRotation.eulerAngles.y,
            localRotation.eulerAngles.z));
        while (Quaternion.Angle(_player.rotation, lookRotation) > 0.1f)
        {
            _player.rotation = Quaternion.Slerp(_player.rotation, lookRotation, Time.deltaTime * 8f);
            cameraT.localRotation = Quaternion.Slerp(cameraT.localRotation, cameraTo, Time.deltaTime * 8f);
            yield return null;
        }

        var t = _file switch
        {
            Folder => transform.parent.parent.parent.parent,
            RoomFile => transform,
            _ => null
        };
        if (t != null)
        {
            AudioManager.Instance.StartCoroutine(AudioManager.Play(transform, AudioManager.Instance.DropClip));
            while (Vector3.Distance(t.position, _bachecaLookAt.position) > 0.1f)
            {
                t.position = Vector3.MoveTowards(t.position, _bachecaLookAt.position, Time.deltaTime * 8f);
                yield return null;
            }
        }

        Instantiate(_explosion, transform);
        yield return new WaitForSeconds(0.3f);
        switch (_file)
        {
            case Folder:
                transform.parent.parent.parent.parent.SetParent(_destinationRoom.GetBacheca().transform);
                break;
            case RoomFile:
                transform.SetParent(_destinationRoom.GetBacheca().transform);
                break;
        }
        while (Quaternion.Angle(_player.rotation, previousPlayerRotation) > 0.1f)
        {
            _player.rotation = Quaternion.Slerp(_player.rotation, previousPlayerRotation, Time.deltaTime * 8f);
            cameraT.localRotation =
                Quaternion.Slerp(cameraT.localRotation, previousCameraRotation, Time.deltaTime * 8f);
            yield return null;
        }
        _destinationRoom.InsertFileOrFolder(_file, false);
        if (HouseManager.ActualQuest == 1)
        {
            QuestManager.Quest1FormatChecker(Folder.Root);
        }
        switch (_file)
        {
            case Folder:
                NotificationManager.Notify(Operation.FolderMoved);
                Destroy(transform.parent.parent.parent.parent.gameObject);
                break;
            case RoomFile:
                NotificationManager.Notify(Operation.FileMoved);
                Destroy(gameObject);
                break;
        }
        Magnet0Raycaster.Operating = true;
    }

    public void Recover()
    {
        Magnet0Raycaster.Operating = false;
        _file.Recover();
        switch (_file)
        {
            case Folder:
                Destroy(transform.parent.parent.parent.parent.gameObject);
                NotificationManager.Notify(Operation.FolderRestored);
                break;
            case RoomFile:
                Destroy(gameObject);
                NotificationManager.Notify(Operation.FileRestored);
                break;
        }
        if (_fileNameTextRaycast.text.Trim() != "")
        {
            _fileNameTextRaycast.text = "";
        }
        Magnet0Raycaster.Operating = true;
    }

    public void PermDelete()
    {
        Magnet0Raycaster.Operating = false;
        _file.PermDelete();
        switch (_file)
        {
            case Folder:
                Destroy(transform.parent.parent.parent.parent.gameObject);
                break;
            case RoomFile:
                Destroy(gameObject);
                break;
        }
        if (_fileNameTextRaycast.text.Trim() != "")
        {
            _fileNameTextRaycast.text = "";
        }
        Magnet0Raycaster.Operating = true;
    }

    public void SetReferred(Grabbable file)
    {
        _file = file;
    }

    public Grabbable GetReferred()
    {
        return _file;
    }

    public Grabber Copy(Transform objHolder)
    {
        Magnet0Raycaster.Operating = false;
        var text = _fileNameTextRaycast.text;
        if (text.Contains("."))
        {
            text = text.Split(".")[0] + "_copia." + text.Split(".")[1];
        }
        else
        {
            text += "_copia";
        }
        _fileNameTextRaycast.text = "";
        _fileNameTextGrabbed.text = text;
        Transform t;
        GameObject duplicate;
        switch (_file)
        {
            case Folder:
                t = transform.parent.parent.parent.parent;
                duplicate = Instantiate(t.gameObject, objHolder);
                duplicate.transform.GetComponent<BoxCollider>().enabled = false;
                // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
                duplicate.transform.GetComponent<Animator>().SetBool("openDoor", true);
                // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
                duplicate.transform.GetComponent<Animator>().SetBool("closeDoor", false);
                StartCoroutine(CloseDoorWhenCopying(duplicate.transform.GetComponent<Animator>()));
                duplicate.transform.GetComponent<DoorController>().enabled = false;
                duplicate.transform.localPosition = new Vector3(0f, 0f, 2f);
                duplicate.transform.localRotation = Quaternion.Euler(180f, -90f, 120f);
                duplicate.transform.localScale *= 3f;
                //notifica
                NotificationManager.Notify(Operation.FolderCopied);
                Magnet0Raycaster.Operating = true;
                return duplicate.transform.GetComponentInChildren<Grabber>();
            case RoomFile:
                t = transform;
                duplicate = Instantiate(t.gameObject, objHolder);
                duplicate.transform.localPosition = new Vector3(0f, 0f, 0f);
                duplicate.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                duplicate.transform.localScale *= 5f;
                //notifica
                NotificationManager.Notify(Operation.FileCopied);
                Magnet0Raycaster.Operating = true;
                return duplicate.transform.GetComponent<Grabber>();
        }
        Magnet0Raycaster.Operating = true;
        return null;
    }

    private IEnumerator CloseDoorWhenCopying(Animator animator)
    {
        yield return new WaitForFixedUpdate();
        // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
        animator.SetBool("openDoor", false);
        // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
        animator.SetBool("closeDoor", true);
    }

    public void Delete()
    {
        Magnet0Raycaster.Operating = false;
        _file.Delete();
        switch (_file)
        {
            case Folder:
                Destroy(transform.parent.parent.parent.parent.gameObject);
                break;
            case RoomFile:
                Destroy(gameObject);
                break;
        }
        if (_fileNameTextRaycast.text.Trim() != "")
        {
            _fileNameTextRaycast.text = "";
        }
        Magnet0Raycaster.Operating = true;
    }

    public void Rename(string newName)
    {
        Magnet0Raycaster.Operating = false;
        _file.Rename(newName);
        Magnet0Raycaster.Operating = true;
    }

    public GameObject ShowRenameMenu(Transform canvasT)
    {
        var renameMenu = Instantiate(RenameCanvasPrefab, canvasT);
        var fileNameText = renameMenu.transform.Find("RenameFileNameInputField").GetComponent<TMP_InputField>();
        fileNameText.text = _file.GetName().Split(".")[0];
        var formatText = renameMenu.transform.Find("RenameFormatInputField").GetComponentInChildren<TMP_Text>();
        switch (_file)
        {
            case Folder:
                formatText.transform.parent.gameObject.SetActive(false);
                renameMenu.transform.Find("DotText").gameObject.SetActive(false);
                fileNameText.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                break;
            case RoomFile file:
                formatText.text = file.GetName().Split(".")[1];
                break;
        }
        return renameMenu;
    }

    public GameObject ShowTrashItemMenu(Transform player)
    {
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        Cursor.lockState = CursorLockMode.None;
        return Instantiate(TrashItemCanvasPrefab);
    }

    public GameObject ShowObjectMenu(Transform player)
    {
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        Cursor.lockState = CursorLockMode.None;
        return Instantiate(ObjMenuCanvasPrefab);
    }

    public void GrabReferred(Transform objHolder)
    {
        Magnet0Raycaster.Operating = false;
        AudioManager.Instance.StartCoroutine(AudioManager.Play(transform, AudioManager.Instance.GrabClip));
        _fileNameTextGrabbed.text = _fileNameTextRaycast.text.Trim();
        _fileNameTextRaycast.text = "";
        Transform t;
        switch (_file)
        {
            case Folder folder:
                folder.GetParent().RemoveChild(folder, Operation.FolderMoving);
                t = transform.parent.parent.parent.parent;
                t.GetComponent<BoxCollider>().enabled = false;
                // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
                t.GetComponent<Animator>().SetBool("closeDoor", true);
                // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
                t.GetComponent<Animator>().SetBool("openDoor", false);
                t.GetComponent<DoorController>().enabled = false;
                t.SetParent(objHolder);
                t.localPosition = new Vector3(0f, 0f, 2f);
                t.localRotation = Quaternion.Euler(180f, -90f, 120f);
                t.localScale *= 0.15f;
                break;
            case RoomFile:
                t = transform;
                t.SetParent(objHolder);
                t.localPosition = new Vector3(0f, 0f, 0f);
                t.localRotation = Quaternion.Euler(0f, 0f, 0f);
                t.localScale *= 0.75f;
                break;
        }
        Magnet0Raycaster.Operating = true;
    }

    public void DropInBox(Transform player, Transform boxObjHolder)
    {
        Magnet0Raycaster.Operating = false;
        _fileNameTextGrabbed.text = "";
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        switch (_file)
        {
            case Folder:
                transform.parent.parent.parent.parent.SetParent(boxObjHolder);
                transform.parent.parent.parent.parent.localPosition = new Vector3(0f, 1.5f, 0f);
                transform.parent.parent.parent.parent.localRotation = Quaternion.Euler(0f, 0f, 0f);
                transform.parent.parent.parent.parent.localScale *= 0.75f;
                break;
            case RoomFile:
                transform.SetParent(boxObjHolder);
                transform.localPosition = new Vector3(0f, 1.5f, 0f);
                transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                break;
        }
        Magnet0Raycaster.Operating = true;
    }

    public void DropReferred(Transform player, Folder room, GameObject explosion)
    {
        Magnet0Raycaster.Operating = false;
        _fileNameTextGrabbed.text = "";
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        _destinationRoom = room;
        _bachecaLookAt = room.GetContainer().transform.GetChild(0).Find("BachecaLookAt");
        _explosion = explosion;
        StartCoroutine(AnimationAfterDrop());
    }

    public void TriggerLabel(bool value)
    {
        if (value)
        {
            if (!_labelVisibility && _fileNameTextRaycast != null && _file?.GetName() != null)
            {
                _fileNameTextRaycast.text = _file.GetName().Trim();
            }
        }
        else if (_labelVisibility && _fileNameTextRaycast != null && _fileNameTextRaycast.text.Trim() != "")
        {
            _fileNameTextRaycast.text = "";
        }
        _labelVisibility = value;
    }
}
