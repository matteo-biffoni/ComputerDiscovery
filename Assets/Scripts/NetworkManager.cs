using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

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

    public string[] CameBackFromNetworkDialog;

    private Grabber _currentInserted;
    private bool _shouldLookAtPlayer = true;
    private bool _playerShouldLookAtMe;
    private Transform _lookAtMe;

    private AD5LNavController _ad5LNavController;
    private Quaternion _previousPlayerRotation;
    private Quaternion _previousCameraRotation;

    private bool _shouldListenForRaycastChanges = true;

    // Start is called before the first frame update
    private void Start()
    {
        _outline = GetComponent<Outline>();
        _lookAtMe = transform.Find("AD5L_LookAt").transform;
        _ad5LNavController = GetComponent<AD5LNavController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_shouldListenForRaycastChanges)
        {
            if (_changeRaycast)
            {
                _changeRaycast = false;
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

                AD5LOutline(_actualRaycast);
            }
        }

        if (_shouldLookAtPlayer)
        {
            var lookAtPlayer = Player.transform.position;
            lookAtPlayer.y = transform.position.y;
            transform.LookAt(lookAtPlayer);
        }

        if (_playerShouldLookAtMe)
        {
            var lookAtMeCorr = _lookAtMe.position;
            lookAtMeCorr.y = Player.transform.position.y;
            Player.transform.LookAt(lookAtMeCorr);
        }

        if (_startDialog)
        {
            if (Input.GetKeyDown(InteractWithAD5LKeyCode))
            {
                _startDialog = false;
                Player.IgnoreInput();
                AD5LOutline(false);
                InteractCanvas.SetActive(false);
                StartCoroutine(SimpleInteract());
            }
        }
    }

    private IEnumerator SimpleInteract()
    {
        _playerShouldLookAtMe = false;
        yield return SmoothTurnToAD5L();
        DialogueCanvas.SetActive(true);
        DialogueManager.OpenDialogue(EndDialogue, InfoDialog, ActorName, ActorSprite);
    }

    private IEnumerator SmoothTurnToAD5L()
    {
        _previousPlayerRotation = Player.transform.rotation;
        _previousCameraRotation = Player.transform.GetComponentInChildren<Camera>().transform.localRotation;
        var ad5LookAt = transform.Find("AD5L_LookAt").transform.position;
        var direction = (ad5LookAt - Player.transform.position).normalized;
        var cameraT = Player.transform.GetComponentInChildren<Camera>().transform;
        var lookRotationCamera = Quaternion.LookRotation(direction);
        direction.y = 0;
        var lookRotation = Quaternion.LookRotation(direction);
        var cameraTo = Quaternion.Euler(new Vector3(lookRotationCamera.eulerAngles.x, cameraT.localRotation.eulerAngles.y,
            cameraT.localRotation.eulerAngles.z));
        while (Quaternion.Angle(Player.transform.rotation, lookRotation) > 0.1f)
        {
            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, lookRotation, Time.deltaTime * 8f);
            cameraT.localRotation = Quaternion.Slerp(cameraT.localRotation, cameraTo, Time.deltaTime * 8f);
            yield return null;
        }
    }

    private IEnumerator SmoothReturnToPreviousOrientation()
    {
        var cameraT = Player.transform.GetComponentInChildren<Camera>().transform;
        while (Quaternion.Angle(Player.transform.rotation, _previousPlayerRotation) > 0.1f)
        {
            Player.transform.rotation =
                Quaternion.Slerp(Player.transform.rotation, _previousPlayerRotation, Time.deltaTime * 12f);
            cameraT.localRotation =
                Quaternion.Slerp(cameraT.localRotation, _previousCameraRotation, Time.deltaTime * 12f);
            yield return null;
        }
        InteractCanvas.SetActive(_actualRaycast);
        AD5LOutline(_actualRaycast);
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
        _shouldListenForRaycastChanges = true;
    }

    public IEnumerator FileInsertedInBox(Grabber grabber)
    {
        _shouldListenForRaycastChanges = false;
        _currentInserted = grabber;
        InteractCanvas.SetActive(false);
        yield return SmoothTurnToAD5L();
        DialogueCanvas.SetActive(true);
        switch (_currentInserted.GetReferred())
        {
            case Folder:
                DialogueManager.OpenDialogue(EndDialogueErr, ErrInsertionInBox, ActorName, ActorSprite);
                break;
            case RoomFile:
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

    public void CameBackFromNetwork()
    {
        Destroy(_currentInserted.gameObject);
        _currentInserted = null;
        _shouldLookAtPlayer = true;
        DialogueCanvas.SetActive(true);
        DialogueManager.OpenDialogue(EndDialogue, CameBackFromNetworkDialog, ActorName, ActorSprite);
    }
    

    private void AD5LOutline(bool show)
    {
        _outline.OutlineWidth = show ? 5f : 0f;
        _outline.enabled = show;
    }

    private void EndDialogue()
    {
        DialogueCanvas.SetActive(false);
        _playerShouldLookAtMe = false;
        StartCoroutine(SmoothReturnToPreviousOrientation());
        NetworkBox.ReOpenBox();
    }

    private void EndDialogueOk()
    {
        DialogueCanvas.SetActive(false);
        _shouldLookAtPlayer = false;
        _playerShouldLookAtMe = true;
        _ad5LNavController.enabled = true;
        _ad5LNavController.StartCoroutine(_ad5LNavController.SendBoxInNetwork());
    }

    private void EndDialogueErr()
    {
        DialogueCanvas.SetActive(false);
        if (_currentInserted.GetReferred().GetParent() != null)
        {
            _currentInserted.GetReferred().SetParentOnDeletionAbsolutePath(_currentInserted.GetReferred().GetParent().GetAbsolutePath());
            _currentInserted.Recover();
        }
        Destroy(_currentInserted.transform.parent.parent.parent.parent.gameObject);
        NetworkBox.ReOpenBox();
        _currentInserted = null;
        StartCoroutine(SmoothReturnToPreviousOrientation());
    }
}
