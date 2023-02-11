using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShaker : MonoBehaviour
{
    private bool _shake;

    private Vector3 _defaultPosition;

    private void Start()
    {
        _defaultPosition = transform.localPosition;
    }

    private void Update()
    {
        if (!_shake)
        {
            if (transform.localPosition != _defaultPosition)
            {
                transform.localPosition = _defaultPosition;
            }
            return;
        }
        var shakeI = Random.Range(-0.5f, 0.5f);
        transform.localPosition = new Vector3(shakeI, transform.localPosition.y, transform.localPosition.z);
    }

    public void Shake()
    {
        _shake = true;
    }

    public void Stabilize()
    {
        _shake = false;
    }
}
