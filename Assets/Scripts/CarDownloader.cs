using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
            InteractCanvasText.text = "Fai click per scaricare";
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
            while (Folder.Root.GetFiles().Count > 5)
            {
                Folder.Root.GetFiles()[0].PermDelete();
            }
            _operationOngoing = true;
            Player.IgnoreInput();
            InteractCanvas.SetActive(false);
            InteractCanvasText.text = "Premi E per interagire";
            StartCoroutine(Operate(4f));
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
        DownloadFileInDesktop();
        DownloadText.text = "File scaricati nel Desktop";
        InteractCanvasText.text = "Premi E per interagire";
        InteractCanvas.SetActive(false);
        Player.ReactivateInput();
        SetActualRaycast(false);
        StartCoroutine(ChangeDownloadText(10f));
    }

    private static void DownloadFileInDesktop()
    {
        var file1 = new RoomFile("Centaurus IX.jpg", "jpeg", 13, 70, Folder.Root, null, Guid.NewGuid().ToString());
        var file2 = new RoomFile("Metallo X79.jpg", "jpeg", 14, 70, Folder.Root, null, Guid.NewGuid().ToString());
        var file3 = new RoomFile("Blaster a infrarossi.png", "png", 15, 70, Folder.Root, null, Guid.NewGuid().ToString());
        var file4 = new RoomFile("Scoperte.docx", "doc", -2, 70, Folder.Root, null, Guid.NewGuid().ToString());
        var file5 = new RoomFile("Tramonto 3 soli.mov", "mov", -1, 70, Folder.Root, null, Guid.NewGuid().ToString());
        var files = new List<RoomFile> { file1, file2, file3, file4, file5 };
        Folder.Root.GetFiles().AddRange(files);
        RoomFile.ScoperteFile = file4;
        Folder.TriggerReloading(Operation.Nop);
        HouseManager.ActualQuest = 4;
        //messaggi su lavagnetta
        List<string> messagesToSend = new List<string>(new string[]
        {
            "Tutti i file scaricati dalla macchina USB sono disponibili nel Desktop"
        });
        LavagnettaManager.SpecialWriteOnLavagnetta( "BEN FATTO!", "Osserva bene...", messagesToSend); 
        NotificationManager.Instance.StartCoroutine(NotificationManager.QuestNotify("Lamp ti sta aspettando! :)"));
    }

    private IEnumerator ChangeDownloadText(float afterSec)
    {
        yield return new WaitForSeconds(afterSec);
        DownloadText.text = "Nessun file presente nella macchina USB";
        Destroy(gameObject);
    }
}
