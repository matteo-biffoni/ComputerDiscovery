using UnityEngine;

public class Magnet0Cam : MonoBehaviour
{
    public float SensitivityX = 400f;
    public float SensitivityY = 400f;
    public Transform Body;
    public Transform CameraPos;
    private float _xRotation;
    private float _yRotation;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        transform.position = CameraPos.transform.position;
        var mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * SensitivityX;
        var mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * SensitivityY;
        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 55f);
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        Body.rotation = Quaternion.Euler(0, _yRotation, 0);
    }
}
