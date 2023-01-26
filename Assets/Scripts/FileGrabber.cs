using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileGrabber : MonoBehaviour
{
    private RoomFile _file;
    private Transform _player;
    private Folder _room;
    private Vector3 _bachecaTarget;
    private bool _turnPlayer;
    private bool _moveTowardsBacheca;
    private GameObject _explosion;

    private void Update()
    {
        if (_turnPlayer)
        {
            var direction = (_room.GetBacheca().transform.position - _player.position).normalized;
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

    public void DropFile(Transform player, Folder room, GameObject explosion)
    {
        _player = player;
        _player.GetComponent<FirstPersonCharacterController>().IgnoreInput();
        _room = room;
        var bachecaPosition = room.GetBacheca().transform.position;
        _bachecaTarget = new Vector3(bachecaPosition.x, bachecaPosition.y + 1f, bachecaPosition.z);
        _explosion = explosion;
        _turnPlayer = true;
    }

    private IEnumerator Finish()
    {
        _room.InsertFile(this);
        yield return new WaitForSeconds(1f);
        _player.GetComponent<FirstPersonCharacterController>().ReactivateInput();
        Destroy(gameObject);
    }
}
