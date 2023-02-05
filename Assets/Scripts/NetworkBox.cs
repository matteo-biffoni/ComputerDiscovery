using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBox : MonoBehaviour
{
    public NetworkManager NetworkManager;
    public Outline Outline;

    private bool _actualRaycast;

    private bool _changeRaycast;

    private Grabber _insertedFile;

    private Animator _animator;
    private static readonly int Close = Animator.StringToHash("close");
    private static readonly int Open = Animator.StringToHash("open");

    private void Start()
    {
        Outline = GetComponent<Outline>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_changeRaycast)
        {
            _changeRaycast = false;
            BoxOutline(_actualRaycast);
        }
    }

    public void ReOpenBox()
    {
        _animator.SetBool(Open, true);
        _animator.SetBool(Close, false);
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

    private void BoxOutline(bool show)
    {
        Outline.OutlineWidth = show ? 5f : 0f;
        Outline.enabled = show;
    }

    public void FileInserted(Grabber grabber)
    {
        _insertedFile = grabber;
        switch (_insertedFile.GetReferred())
        {
            case Folder:
                StartCoroutine(FallInBox(grabber.transform.parent.parent.parent.parent, transform.Find("BoxObjHolder").position));
                break;
            case RoomFile:
                StartCoroutine(FallInBox(grabber.transform, transform.Find("BoxObjHolder").position));
                break;
        }
    }

    private IEnumerator FallInBox(Transform grabberT, Vector3 target)
    {
        _animator.SetBool(Open, false);
        _animator.SetBool(Close, true);
        while (grabberT.transform.position != target)
        {
            grabberT.transform.position = Vector3.MoveTowards(grabberT.transform.position, target, Time.deltaTime * 0.75f);
            yield return null;
        }
        NetworkManager.StartCoroutine(NetworkManager.FileInsertedInBox(_insertedFile));
    }
}
