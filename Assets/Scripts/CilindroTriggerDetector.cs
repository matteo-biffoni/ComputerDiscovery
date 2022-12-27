using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CilindroTriggerDetector : MonoBehaviour
{
    // public qualcosa che permetta al giocatore di lasciare l'oggetto e relativa interfaccia
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // abilitare questo qualcosa
            Debug.Log("Player inside release area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // disabilitare questo qualcosa
            Debug.Log("Player outside release area");
        }
    }
}
