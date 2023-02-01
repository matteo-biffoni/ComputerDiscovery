using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


public class Grabber : MonoBehaviour
{
    private Grabbable _file;
    private Transform _player;
    private Folder _destinationRoom;
    private Vector3 _bachecaTarget;
    private bool _turnPlayer;
    private bool _moveTowardsBacheca;
    private GameObject _explosion;
    private bool _labelVisibility;
    public GameObject FileTextLabel;
    private GameObject _instantiatedFileTextLabel;
    public Outline Outlined;
    [FormerlySerializedAs("ObjMenuCanvas")] public GameObject ObjMenuCanvasPrefab;
    public GameObject TrashItemCanvasPrefab;

    private void Start()
    {
        if (Outlined == null) Outlined = GetComponent<Outline>();
    }

    private void Update()
    {
        if (_turnPlayer)
        {
            var direction = (_destinationRoom.GetBacheca().transform.position - _player.position).normalized;
            var lookRotation = Quaternion.LookRotation(direction);
            _player.rotation = Quaternion.Slerp(_player.rotation, lookRotation, Time.deltaTime * 8f);
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
        Debug.Log("Grabber.Delete");
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
    }

    public void Rename()
    {
        
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

    public void DropReferred(Transform player, Folder room, GameObject explosion)
    {
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        _destinationRoom = room;
        var bachecaPosition = room.GetBacheca().transform.position;
        _bachecaTarget = new Vector3(bachecaPosition.x, bachecaPosition.y + 1f, bachecaPosition.z);
        _explosion = explosion;
        _turnPlayer = true;
    }

    private IEnumerator Finish(bool isRecovering)
    {
        _destinationRoom.InsertFileOrFolder(_file, isRecovering);
        yield return new WaitForSeconds(1f);
        _player.GetComponent<FirstPersonCharacterController>().ReactivateInput();
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
        else if (_labelVisibility)
        {
            Destroy(_instantiatedFileTextLabel);
        }
        _labelVisibility = value;
    }
}
