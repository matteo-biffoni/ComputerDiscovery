using System.Linq;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    private bool _open;
    private Folder _room;
    private Folder _roomTo;

    private Animator _doorAnimator;
    private static readonly int OpenDoor = Animator.StringToHash("openDoor");
    private static readonly int CloseDoor = Animator.StringToHash("closeDoor");

    public TMP_Text DirectionFrontText;
    
    /*private void Start()
    {
        _doorAnimator = GetComponent<Animator>();
    }*/

    private void Awake()
    {
        _doorAnimator = GetComponent<Animator>();
    }

    public void SetRoom(Folder folder)
    {
        _room = folder;
    }

    public void SetRoomTo(Folder folder)
    {
        _roomTo = folder;
        DirectionFrontText.text = _roomTo.GetName();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        var playerRoom = (other.gameObject.GetComponent(typeof(PlayerNavigatorManager)) as PlayerNavigatorManager)?.GetRoomIn();
        if (playerRoom != _room && playerRoom != _roomTo) return;
        _roomTo?.ActivateRoomComponents(true);
        if (playerRoom == _roomTo && _room == Folder.Root && !Folder.IsMainRoomVisible())
        {
            Folder.ShowMainRoom(true);
        }
        else if (_room != Folder.Root && Folder.IsMainRoomVisible())
        {
            Folder.ShowMainRoom(false);
        }

        if (playerRoom == _room)
        {
            DeactivateOtherChildren();
        }
        else if (playerRoom == _roomTo)
        {
            DeactivateChildren();
        }
        _doorAnimator.SetBool(CloseDoor, false);
        _doorAnimator.SetBool(OpenDoor, true);
    }

    private void DeactivateChildren()
    {
        foreach (var child in _roomTo.GetChildren())
        {
            child.ActivateRoomComponents(false);
        }
    }

    private void DeactivateOtherChildren()
    {
        foreach (var child in _room.GetChildren().Where(child => child != _roomTo))
        {
            child.ActivateRoomComponents(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        //var playerRoom = (other.gameObject.GetComponent(typeof(PlayerNavigatorManager)) as PlayerNavigatorManager)?.GetRoomIn();
        //if (playerRoom != _room && playerRoom != _roomTo) return;
        _doorAnimator.SetBool(OpenDoor, false);
        _doorAnimator.SetBool(CloseDoor, true);
        //if (playerRoom == _room)
        //{
        //    StartCoroutine(WaitForDoorToBeClosedAndDeactivateChildComponents());
        //}
    }

    /*private IEnumerator WaitForDoorToBeClosedAndDeactivateChildComponents()
    {
        yield return new WaitUntil(() => _doorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Door_Closed"));
        _roomTo?.ActivateRoomComponents(false);
    }*/
}
