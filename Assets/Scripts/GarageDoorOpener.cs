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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(MoveDoorTo(OpenedPosition.position));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(MoveDoorTo(ClosedPosition.position));
        }
    }
}
