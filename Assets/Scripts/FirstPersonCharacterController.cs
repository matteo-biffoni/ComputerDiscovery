using UnityEngine;

public class FirstPersonCharacterController : MonoBehaviour
{
    public Transform CameraT;
    public float Speed = 5f;
    private float _defaultSpeed;
    public float MouseSensitivity = 100f;

    public float Gravity = -9.81f;
    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    private CharacterController _characterController;
    private float _cameraXRotation;
    private Vector3 _velocity;
    private bool _isGrounded;

    private bool _ignoreInput;
    private bool _ignoreMovement;

    public bool IsMagnet0Free()
    {
        return !_ignoreInput;
    }

    public void IgnoreMovement()
    {
        _ignoreMovement = true;
    }

    public void IgnoreInput()
    {
        _ignoreInput = true;
    }

    public void ReactivateInput()
    {
        _ignoreMovement = false;
        _ignoreInput = false;
    }
    
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        _defaultSpeed = Speed;
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        if (_isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
        }

        if (!_ignoreInput)
        {
            var mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up, mouseX);
            _cameraXRotation -= mouseY;
            _cameraXRotation = Mathf.Clamp(_cameraXRotation, -90f, 90f);
            CameraT.localRotation = Quaternion.Euler(_cameraXRotation, 0f, 0f);

            if (!_ignoreMovement)
            {
                var h = Input.GetAxis("Horizontal");
                var v = Input.GetAxis("Vertical");
                var tr = transform;
                var move = (tr.right * h + tr.forward * v).normalized;
                _characterController.Move(move * (Speed * Time.deltaTime));
            }
        }

        _velocity.y += Gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    public void EnableSlowMovementAndShakeCamera()
    {
        Speed = _defaultSpeed * 0.75f;
        CameraT.GetComponent<CameraShaker>().Shake(0.25f);
    }

    public void BetterSlowMovementAndBetterShakeCamera()
    {
        Speed = _defaultSpeed * 0.9f;
        var cameraShaker = CameraT.GetComponent<CameraShaker>();
        cameraShaker.Stabilize();
        cameraShaker.Shake(0.1f);
    }

    public void MovementBackToNormal()
    {
        Speed = _defaultSpeed;
        CameraT.GetComponent<CameraShaker>().Stabilize();
    }
}
