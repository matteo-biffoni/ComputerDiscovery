using System.Linq;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    private bool _open;
    private Folder _room;
    private Folder _roomTo;

    public GameObject Handle;
    public GameObject ObjMenuCanvasPrefab;
    public GameObject TrashItemCanvasPrefab;
    public GameObject RenameCanvasPrefab;
    public Outline Outline;

    private Animator _doorAnimator;
    private static readonly int OpenDoor = Animator.StringToHash("openDoor");
    private static readonly int CloseDoor = Animator.StringToHash("closeDoor");

    public TMP_Text DirectionFrontText;

    private void Awake()
    {
        _doorAnimator = GetComponent<Animator>();
        if (Folder.ShouldDoorsHaveGrabberAttached)
        {
            var grabber = Handle.AddComponent<Grabber>();
            grabber.Outlined = Outline;
            grabber.RenameCanvasPrefab = RenameCanvasPrefab;
            grabber.TrashItemCanvasPrefab = TrashItemCanvasPrefab;
            grabber.ObjMenuCanvasPrefab = ObjMenuCanvasPrefab;
        }
    }

    private void Start()
    {
        if (!(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position,
                transform.position) < 1.17197f)) return;
        _doorAnimator.SetBool(CloseDoor, false);
        _doorAnimator.SetBool(OpenDoor, true);
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
        AudioManager.Play(transform, AudioManager.Instance.OpenDoor);
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
        AudioManager.Play(transform, AudioManager.Instance.OpenDoor);
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
