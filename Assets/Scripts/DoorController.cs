using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    public LayerMask RoomLayerMask;
    
    //private Quaternion _openDoor, _closedDoor;
    //public GameObject DoorRotator;

    private bool _open;
    private Folder _room;
    private Folder _roomTo;

    private Animator _doorAnimator;
    private static readonly int OpenDoor = Animator.StringToHash("openDoor");
    private static readonly int CloseDoor = Animator.StringToHash("closeDoor");

    //public TMP_Text DirectionText;
    
    private void Start()
    {
        var roomColliders = new Collider[9];
        var size = Physics.OverlapSphereNonAlloc(transform.position, .1f, roomColliders, RoomLayerMask);
        if (size != 1)
        {
            throw new Exception($"Doors should collide with 1 room exactly. Found: {size}");
        }
        _room = Folder.GetFolderFromCollider(Folder.Root, roomColliders[0]);
        _roomTo = _room.GetChildrenFromDoorController(this);
        //DirectionText.text = _roomTo.GetName();
        _doorAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        var playerRoom = (other.gameObject.GetComponent(typeof(Magnet0Movement)) as Magnet0Movement)?.GetRoomIn();
        if (playerRoom != _room && playerRoom != _roomTo) return;
        _roomTo?.ActivateRoomComponents(true);
        //_room?.ActivateChildComponents(this, true);
        _doorAnimator.SetBool(CloseDoor, false);
        _doorAnimator.SetBool(OpenDoor, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        var playerRoom = (other.gameObject.GetComponent(typeof(Magnet0Movement)) as Magnet0Movement)?.GetRoomIn();
        if (playerRoom != _room && playerRoom != _roomTo) return;
        if (playerRoom == _room)
        {
            _room?.ActivateChildComponents(this, false);
        }
        _doorAnimator.SetBool(OpenDoor, false);
        _doorAnimator.SetBool(CloseDoor, true);
    }
}
