using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private Outline _outline;
    private bool _actualRaycast;
    private bool _changeRaycast;
    private bool _startDialog;

    public KeyCode InteractWithAD5LKeyCode;
    public FirstPersonCharacterController Player;
    public NetworkBox NetworkBox;
    public GameObject InteractCanvas;
    public GameObject DialogueCanvas;
    public DialogueManager DialogueManager;
    public string ActorName;
    public Sprite ActorSprite;

    public string[] InfoDialog;

    public string[] OkInsertionInBox;

    public string[] ErrInsertionInBox;

    private Grabber _currentInserted;
    
    // Start is called before the first frame update
    private void Start()
    {
        _outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_changeRaycast)
        {
            if (_actualRaycast)
            {
                InteractCanvas.SetActive(true);
                _startDialog = true;
            }
            else
            {
                _startDialog = false;
                InteractCanvas.SetActive(false);
            }
            _changeRaycast = false;
            AD5LOutline(_actualRaycast);
        }

        var lookAtPlayer = Player.transform.position;
        lookAtPlayer.y = transform.position.y;
        transform.LookAt(lookAtPlayer);
        if (_startDialog)
        {
            if (Input.GetKeyDown(InteractWithAD5LKeyCode))
            {
                _startDialog = false;
                InteractCanvas.SetActive(false);
                DialogueCanvas.SetActive(true);
                DialogueManager.OpenDialogue(EndDialogue, InfoDialog, ActorName, ActorSprite);
            }
        }
    }

    public IEnumerator FileInsertedInBox(Grabber grabber)
    {
        _currentInserted = grabber;
        var targetPosition = transform.Find("AD5L_LookAt").transform.position;
        var direction = (targetPosition - Player.transform.position).normalized;
        var cameraT = Player.transform.GetComponentInChildren<Camera>().transform;
        var lookRotationCamera = Quaternion.LookRotation(direction);
        direction.y = 0;
        var lookRotation = Quaternion.LookRotation(direction);
        var cameraTo = Quaternion.Euler(new Vector3(lookRotationCamera.eulerAngles.x, cameraT.localRotation.eulerAngles.y,
            cameraT.localRotation.eulerAngles.z));
        while (Quaternion.Angle(Player.transform.rotation, lookRotation) > 0.001f)
        {
            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, lookRotation, Time.deltaTime * 8f);
            cameraT.localRotation = Quaternion.Slerp(cameraT.localRotation, cameraTo, Time.deltaTime * 8f);
            yield return null;
        }
        DialogueCanvas.SetActive(true);
        switch (_currentInserted.GetReferred())
        {
            case Folder:
                DialogueManager.OpenDialogue(EndDialogueErr, ErrInsertionInBox, ActorName, ActorSprite);
                break;
            case RoomFile roomFile:
                DialogueManager.OpenDialogue(EndDialogueOk, OkInsertionInBox, ActorName, ActorSprite);
                break;
        }
    }

    public void SetActualRaycast(bool value)
    {
        _changeRaycast = true;
        _actualRaycast = value;
    }

    public bool GetActualRaycast()
    {
        return _actualRaycast;
    }
    

    private void AD5LOutline(bool show)
    {
        _outline.OutlineWidth = show ? 5f : 0f;
        _outline.enabled = show;
    }

    private void EndDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
        DialogueCanvas.SetActive(false);
        InteractCanvas.SetActive(_actualRaycast);
    }

    private void EndDialogueOk()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
        DialogueCanvas.SetActive(false);
        InteractCanvas.SetActive(_actualRaycast);
        _currentInserted = null;
    }

    private void EndDialogueErr()
    {
        DialogueCanvas.SetActive(false);
        InteractCanvas.SetActive(_actualRaycast);
        if (_currentInserted.GetReferred().GetParent() != null)
        {
            _currentInserted.GetReferred().SetParentOnDeletionAbsolutePath(_currentInserted.GetReferred().GetParent().GetAbsolutePath());
            _currentInserted.Recover();
        }
        Destroy(_currentInserted.transform.parent.parent.parent.parent.gameObject);
        NetworkBox.ReOpenBox();
        _currentInserted = null;
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
    }
}
