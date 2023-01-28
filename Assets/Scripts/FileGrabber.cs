using System.Collections;
using TMPro;
using UnityEngine;

public class FileGrabber : MonoBehaviour
{
    private RoomFile _file;
    private Transform _player;
    private Folder _destinationRoom;
    private Vector3 _bachecaTarget;
    private bool _turnPlayer;
    private bool _moveTowardsBacheca;
    private GameObject _explosion;
    private bool _labelVisibility;
    public GameObject FileTextLabel;
    private GameObject _instantiatedFileTextLabel;

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
            transform.position = Vector3.MoveTowards(transform.position, _bachecaTarget,Time.deltaTime * 8f);
            if (Vector3.Distance(transform.position, _bachecaTarget) < 0.001f)
            {
                _moveTowardsBacheca = false;
                Instantiate(_explosion, transform);
                StartCoroutine(Finish());
            }
        }
    }

    public void SetFile(RoomFile file)
    {
        _file = file;
    }

    public RoomFile GetFile()
    {
        return _file;
    }

    public void GrabFile(Transform cameraT, Transform objHolder)
    {
        _instantiatedFileTextLabel.transform.SetParent(cameraT);
        _instantiatedFileTextLabel.transform.localPosition = new Vector3(0, -3, 6);
        _instantiatedFileTextLabel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        _instantiatedFileTextLabel.transform.localScale *= 0.3f;
        Transform t;
        (t = transform).SetParent(objHolder);
        t.localPosition = new Vector3(0f, 0f, 0f);
        t.localRotation = Quaternion.Euler(0f, 0f, 0f);
        t.localScale *= 0.75f;
    }

    public void DropFile(Transform player, Folder room, GameObject explosion)
    {
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        _destinationRoom = room;
        var bachecaPosition = room.GetBacheca().transform.position;
        _bachecaTarget = new Vector3(bachecaPosition.x, bachecaPosition.y + 1f, bachecaPosition.z);
        _explosion = explosion;
        _turnPlayer = true;
    }

    private IEnumerator Finish()
    {
        _destinationRoom.InsertFile(this);
        yield return new WaitForSeconds(1f);
        _player.GetComponent<FirstPersonCharacterController>().ReactivateInput();
        Destroy(gameObject);
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
