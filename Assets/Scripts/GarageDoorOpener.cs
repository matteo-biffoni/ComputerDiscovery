using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoorOpener : MonoBehaviour
{
    public Transform ClosedPosition;
    public Transform OpenedPosition;
    public Transform Door;

    private IEnumerator MoveDoorTo(Vector3 target)
    {
        while (Vector3.Distance(Door.position, target) > 0.01f)
        {
            Door.position = Vector3.MoveTowards(Door.position, target, Time.deltaTime * 2f);
            yield return null;
        }
    }

    private IEnumerator RotateDoorTo(Quaternion target)
    {
        while (Quaternion.Angle(Door.localRotation, target) > 0.01f)
        {
            Door.localRotation = Quaternion.Slerp(Door.localRotation, target, Time.deltaTime * 4f);
            yield return null;
        }    
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            AudioManager.Play(transform, AudioManager.Instance.Serranda, false);
            StartCoroutine(MoveDoorTo(OpenedPosition.position));
            if (other.transform.GetComponent<PlayerNavigatorManager>().GetRoomIn() == Folder.Garage)
            {
                StartCoroutine(RotateDoorTo(Quaternion.Euler(90f, Door.localRotation.eulerAngles.y,
                    Door.localRotation.eulerAngles.z)));
            }
            else
            {
                StartCoroutine(RotateDoorTo(Quaternion.Euler(-90f, Door.localRotation.eulerAngles.y,
                    Door.localRotation.eulerAngles.z)));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            AudioManager.Play(transform, AudioManager.Instance.Serranda, false);
            StartCoroutine(MoveDoorTo(ClosedPosition.position));
            StartCoroutine(RotateDoorTo(Quaternion.Euler(0f, Door.localRotation.eulerAngles.y,
                Door.localRotation.eulerAngles.z)));
        }
    }
}
