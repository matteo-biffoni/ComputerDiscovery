using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositioner : MonoBehaviour
{
    public Transform CameraPosition;
    public FirstPersonCharacterController FPS;
    public GameObject RealCamera;
    private bool _ignore;

    private void Update()
    {
        if (_ignore) return;
        var distance = Vector3.Distance(CameraPosition.localPosition, transform.localPosition);
        if (distance > 0.01f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, CameraPosition.localPosition, Time.deltaTime * 2f);
            if (distance < 4f)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, CameraPosition.localRotation,
                    Time.deltaTime * 1.2f);
            }
                
        }
        else if (!FPS.CameraInPosition)
        {
            RealCamera.SetActive(true);
            FPS.CameraInPosition = true;
            _ignore = true;
            gameObject.SetActive(false);
        }
    }
}
