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
    private Vector3 _bachecaTarget;
    private bool _turnPlayer;
    private bool _moveTowardsBacheca;
    private bool _takeBackCamera;
    private GameObject _explosion;
    private bool _labelVisibility;
    private Quaternion _previousPlayerRotation;
    private Quaternion _previousCameraRotation;
    public GameObject FileTextLabel;
    private GameObject _instantiatedFileTextLabel;
    public Outline Outlined;
    [FormerlySerializedAs("ObjMenuCanvas")] public GameObject ObjMenuCanvasPrefab;
    public GameObject TrashItemCanvasPrefab;
    public GameObject RenameCanvasPrefab;

    private void Start()
    {
        if (Outlined == null) Outlined = GetComponent<Outline>();
    }

    private void Update()
    {
        if (_turnPlayer)
        {
            var direction = (_destinationRoom.GetBacheca().transform.position - _player.position).normalized;
            var cameraT = _player.GetComponentInChildren<Camera>().transform;
            var lookRotationCamera = Quaternion.LookRotation(direction);
            direction.y = 0;
            var lookRotation = Quaternion.LookRotation(direction);
            var cameraTo = Quaternion.Euler(new Vector3(lookRotationCamera.eulerAngles.x, cameraT.localRotation.eulerAngles.y,
                cameraT.localRotation.eulerAngles.z));
            _player.rotation = Quaternion.Slerp(_player.rotation, lookRotation, Time.deltaTime * 8f);
            cameraT.localRotation = Quaternion.Slerp(cameraT.localRotation, cameraTo, Time.deltaTime * 8f);
            if (Quaternion.Angle(_player.rotation, lookRotation) <= 0.01f)
            {
                _turnPlayer = false;
                _moveTowardsBacheca = true;
            }
        }
        if (_moveTowardsBacheca)
        {
            Transform t = null;
            switch (_file)
            {
                case Folder:
                    t = transform.parent.parent.parent.parent;
                    break;
                case RoomFile:
                    t = transform;
                    break;
            }

            if (t != null)
            {
                t.position = Vector3.MoveTowards(t.position, _bachecaTarget, Time.deltaTime * 8f);
                if (Vector3.Distance(t.position, _bachecaTarget) < 0.001f)
                {
                    _moveTowardsBacheca = false;
                    Instantiate(_explosion, transform);
                    StartCoroutine(Finish(false));
                }
            }
        }

        if (_takeBackCamera)
        {
            var cameraT = _player.GetComponentInChildren<Camera>().transform;
            _player.rotation = Quaternion.Slerp(_player.rotation, _previousPlayerRotation, Time.deltaTime * 12f);
            cameraT.localRotation = Quaternion.Slerp(cameraT.localRotation, _previousCameraRotation, Time.deltaTime * 12f);
            if (Quaternion.Angle(_player.rotation, _previousPlayerRotation) <= 0.01f)
            {
                _takeBackCamera = false;
                _player.GetComponent<FirstPersonCharacterController>().ReactivateInput();
            }
        }
    }
    

    private IEnumerator Finish(bool isRecovering)
    {
        yield return new WaitForSeconds(0.3f);
        _takeBackCamera = true;
        switch (_file)
        {
            case Folder:
                transform.parent.parent.parent.parent.localScale *= 0f;
                break;
            case RoomFile:
                transform.GetComponent<MeshRenderer>().enabled = false;
                break;
        }
        yield return new WaitUntil(() => !_takeBackCamera);
        _destinationRoom.InsertFileOrFolder(_file, isRecovering);
        switch (_file)
        {
            case Folder:
                Destroy(transform.parent.parent.parent.parent.gameObject);
                break;
            case RoomFile:
                Destroy(gameObject);
                break;
        }
    }

    public void Recover()
    {
        _file.Recover();
        switch (_file)
        {
            case Folder:
                Destroy(transform.parent.parent.parent.parent.gameObject);
                break;
            case RoomFile:
                Destroy(gameObject);
                break;
        }
        if (_instantiatedFileTextLabel != null)
            Destroy(_instantiatedFileTextLabel);
    }

    public void PermDelete()
    {
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
        if (_instantiatedFileTextLabel != null)
            Destroy(_instantiatedFileTextLabel);
    }

    public void SetReferred(Grabbable file)
    {
        _file = file;
    }

    public Grabbable GetReferred()
    {
        return _file;
    }

    public Grabber Copy(Transform cameraT, Transform objHolder)
    {
        _instantiatedFileTextLabel.transform.SetParent(cameraT);
        var text = _instantiatedFileTextLabel.transform.GetComponent<TMP_Text>().text;
        if (text.Contains("."))
        {
            text = text.Split(".")[0] + "_copia." + text.Split(".")[1];
        }
        else
        {
            text += "_copia";
        }
        _instantiatedFileTextLabel.transform.GetComponent<TMP_Text>().text = text;
        _instantiatedFileTextLabel.transform.localPosition = new Vector3(0, -3, 6);
        _instantiatedFileTextLabel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        _instantiatedFileTextLabel.transform.localScale *= 0.3f;
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
                return duplicate.transform.GetComponentInChildren<Grabber>();
            case RoomFile:
                t = transform;
                duplicate = Instantiate(t.gameObject, objHolder);
                duplicate.transform.localPosition = new Vector3(0f, 0f, 0f);
                duplicate.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                duplicate.transform.localScale *= 5f;
                return duplicate.transform.GetComponent<Grabber>();
        }
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
        if (_instantiatedFileTextLabel != null)
            Destroy(_instantiatedFileTextLabel);
    }

    public void Rename(string newName)
    {
        _file.Rename(newName);
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

    public void GrabReferred(Transform cameraT, Transform objHolder)
    {
        _instantiatedFileTextLabel.transform.SetParent(cameraT);
        _instantiatedFileTextLabel.transform.localPosition = new Vector3(0, -3, 6);
        _instantiatedFileTextLabel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        _instantiatedFileTextLabel.transform.localScale *= 0.3f;
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
    }

    public void DropInBox(Transform player, Transform boxObjHolder)
    {
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        switch (_file)
        {
            case Folder:
                transform.parent.parent.parent.parent.SetParent(boxObjHolder);
                transform.parent.parent.parent.parent.localPosition = new Vector3(0f, 1.5f, 0f);
                transform.parent.parent.parent.parent.localRotation = Quaternion.Euler(0f, 0f, -90f);
                break;
            case RoomFile:
                transform.SetParent(boxObjHolder);
                transform.localPosition = new Vector3(0f, 1.5f, 0f);
                transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                break;
        }
    }

    public void DropReferred(Transform player, Folder room, GameObject explosion)
    {
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        _destinationRoom = room;
        var bachecaPosition = room.GetBacheca().transform.position;
        _bachecaTarget = new Vector3(bachecaPosition.x, bachecaPosition.y + 1f, bachecaPosition.z);
        _explosion = explosion;
        _previousPlayerRotation = _player.rotation;
        _previousCameraRotation = _player.GetComponentInChildren<Camera>().transform.localRotation;
        _turnPlayer = true;
    }

    public void TriggerLabel(bool value, Transform orientation)
    {
        if (value)
        {
            if (!_labelVisibility)
            {
                var t = transform;
                var tPosition = t.position;
                _instantiatedFileTextLabel = Instantiate(FileTextLabel, t.parent);
                if (_file is Folder) _instantiatedFileTextLabel.transform.localScale *= 0.002f;
                _instantiatedFileTextLabel.GetComponent<TMP_Text>().text = _file.GetName();
                var position = new Vector3(tPosition.x, tPosition.y + 0.2f, tPosition.z);
                position = Vector3.MoveTowards(position, orientation.position, 0.1f);
                _instantiatedFileTextLabel.transform.position = position;
            }
            _instantiatedFileTextLabel.transform.LookAt(orientation);
            _instantiatedFileTextLabel.transform.Rotate(0, 180, 0);
        }
        else if (_labelVisibility && _instantiatedFileTextLabel != null)
        {
            Destroy(_instantiatedFileTextLabel);
        }
        _labelVisibility = value;
    }
}
