using System.Collections;
using UnityEngine;

public class Magnet0Movement : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed;

    //[Header("Key binds")]
    //public KeyCode InsertFolderKey = KeyCode.I;
    
    public GameObject Body;

    private Transform _bodyTransform;
    private Animator _bodyAnimator;
    private static readonly int Walking = Animator.StringToHash("walking");

    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    private Rigidbody _rb;

    private Folder _roomIn; 
    private Collider _previousCollider;

    //public GameObject NewFolderMenu;
    //public TMP_InputField NewFolderInputField;
    //public TMP_Text ErrorText;
    //public TMP_Text CantAddMoreFoldersText;
    //public int MaxChildFolders;

    private bool _getPlayerMovementInput = true;

    private bool _changingHouseLayout;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rigidbody = GetComponent(typeof(Rigidbody)) as Rigidbody;
        _bodyTransform = Body.GetComponent<Transform>();
        _bodyAnimator = Body.GetComponent<Animator>();
    }

    public void HouseLayoutChangingCompleted(Folder newRoomIn)
    {
        _roomIn = newRoomIn;
        StartCoroutine(RestartPhysicsAndInputs());
    }

    private IEnumerator RestartPhysicsAndInputs()
    {
        yield return new WaitForFixedUpdate();
        _changingHouseLayout = false;
        _rigidbody.isKinematic = false;
    }

    private void Update()
    {
        
        if (_changingHouseLayout) return;
        GetInput();
        
        /*Physics.OverlapSphereNonAlloc(transform.position, 0f, _roomColliders, RoomLayerMask);

        if (_roomColliders[0] == _previousCollider) return;
        _previousCollider = _roomColliders[0];
        var newRoom = Folder.GetFolderFromCollider(Folder.Root, _previousCollider);
        var father = _roomIn?.GetFather();
        if (father != null && newRoom == father)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            _roomIn.ActivateGreatGrandFather(true);
        }
        else if (father != null && newRoom != father)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            newRoom?.ActivateGreatGrandFather(false);
        }
        _roomIn = newRoom;
        */
    }

    public void RoomInChanged(Folder newRoom)
    {
        var father = _roomIn?.GetFather();
        if (father != null && newRoom == father)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            _roomIn.ActivateGreatGrandFather(true);
        }
        else if (father != null && newRoom != father)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            newRoom?.ActivateGreatGrandFather(false);
        }
        _roomIn = newRoom;
    }

    public Folder GetRoomIn()
    {
        return _roomIn;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void GetInput()
    {
        if (!_getPlayerMovementInput) return;
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (_horizontalInput != 0 || _verticalInput != 0)
        {
            _bodyAnimator.SetBool(Walking, true);
        }
        else
        {
            _bodyAnimator.SetBool(Walking, false);
        }
        /*if (!Input.GetKeyDown(InsertFolderKey)) return;
        if (_roomIn.GetChildrenCount() == MaxChildFolders)
        {
            StartCoroutine(ShowMaxSubfoldersMessage());
        }
        else
        {
            ShowFolderCreationMenuAndSetRetrievePlayerInput(true);
        }*/
    }

    /*private IEnumerator ShowMaxSubfoldersMessage()
    {
        CantAddMoreFoldersText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        CantAddMoreFoldersText.gameObject.SetActive(false);
    }

    private void SetAndEnableErrorText(string error)
    {
        ErrorText.text = error;
        ErrorText.gameObject.SetActive(error != "");
    }

    private void ShowFolderCreationMenuAndSetRetrievePlayerInput(bool show)
    {
        if (show)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            _getPlayerMovementInput = false;
            NewFolderMenu.gameObject.SetActive(true);
        }
        else
        {
            SetAndEnableErrorText("");
            NewFolderInputField.text = "";
            NewFolderMenu.gameObject.SetActive(false);
            _getPlayerMovementInput = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void GoBackToNavigation()
    {
        ShowFolderCreationMenuAndSetRetrievePlayerInput(false);
    }

    public void InsertNewFolderButtonPressed()
    {
        if (NewFolderInputField.text == "")
        {
            SetAndEnableErrorText("Folder name can't be empty");
            return;
        }

        if (!_roomIn.IsChildNameAvailable(NewFolderInputField.text))
        {
            SetAndEnableErrorText($"This folder already has a subfolder named \"{NewFolderInputField.text.Trim()}\"");
            return;
        }
        _rigidbody.isKinematic = true;
        _changingHouseLayout = true;
        Folder.InsertNewFolder(NewFolderInputField.text.Trim(), _roomIn);
        ShowFolderCreationMenuAndSetRetrievePlayerInput(false);
    }*/

    private void MovePlayer()
    {
        _moveDirection = _bodyTransform.forward * _verticalInput + _bodyTransform.right * _horizontalInput;
        _rb.AddForce(_moveDirection.normalized * MoveSpeed, ForceMode.Force);
    }
}