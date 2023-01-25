using UnityEngine;

public class FirstPersonCharacterController : MonoBehaviour
{
    public Transform CameraT;
    public float Speed = 5f;
    public float MouseSensitivity = 100f;

    public float Gravity = -9.81f;
    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    private CharacterController _characterController;
    private float _cameraXRotation;
    private Vector3 _velocity;
    private bool _isGrounded;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        if (_isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
        }

        var mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, mouseX);
        _cameraXRotation -= mouseY;
        _cameraXRotation = Mathf.Clamp(_cameraXRotation, -90f, 90f);
        CameraT.localRotation = Quaternion.Euler(_cameraXRotation, 0f, 0f);

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        var tr = transform;
        var move = (tr.right * h + tr.forward * v).normalized;
        _characterController.Move(move * (Speed * Time.deltaTime));
        
        _velocity.y += Gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
}
