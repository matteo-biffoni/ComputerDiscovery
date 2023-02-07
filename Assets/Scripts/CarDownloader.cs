using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarDownloader : MonoBehaviour
{
    private bool _actualRaycast;
    public Outline Outline;
    public GameObject InteractCanvas;
    public TMP_Text InteractCanvasText;
    public FirstPersonCharacterController Player;
    private bool _operationOngoing;
    public TMP_Text DownloadText;

    public static bool FilesDownloaded;

    public bool GetActualRaycast()
    {
        return _actualRaycast;
    }

    public void SetActualRaycast(bool value)
    {
        _actualRaycast = value;
        Outline.enabled = value;
        if (value)
        {
            InteractCanvasText.text = "Fai click per scaricare i file nel Desktop";
        }
        else
        {
            InteractCanvasText.text = "Premi E per interagire";
        }
        InteractCanvas.SetActive(value);
    }

    private void Update()
    {
        if (_operationOngoing) return;
        if (_actualRaycast && Input.GetMouseButtonDown(0))
        {
            _operationOngoing = true;
            Player.IgnoreInput();
            InteractCanvasText.text = "Premi E per interagire";
            InteractCanvas.SetActive(false);
            StartCoroutine(Operate(5f));
        }
    }

    private IEnumerator Operate(float duration)
    {
        var i = 0f;
        var activeTime = 0f;
        while (i < 100)
        {
            DownloadText.text = i + " %";
            activeTime += Time.deltaTime;
            var percent = activeTime / duration;
            i = (int) Mathf.Lerp(0, 100, percent);
            yield return null;
        }
        DownloadText.text = "100 %";
        FilesDownloaded = true;
        yield return new WaitForSeconds(0.5f);
        FinishAndDestroy();
    }

    private void FinishAndDestroy()
    {
        DownloadText.text = "File scaricati nel Desktop";
        InteractCanvasText.text = "Premi E per interagire";
        InteractCanvas.SetActive(false);
        Player.ReactivateInput();
        SetActualRaycast(false);
        Destroy(gameObject);
    }
}
