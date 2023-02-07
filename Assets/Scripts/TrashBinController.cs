using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TrashBinController : MonoBehaviour
{

    public float MediumSizeMin = 10.0f;
    public float LargeSizeMin = 50.0f;

    public GameObject[] Mp3S;
    public GameObject[] ZiPs;
    public GameObject[] MoVs;
    public GameObject[] PdFs;
    public GameObject[] JpeGs;
    public GameObject[] PnGs;
    public GameObject[] DoCs;
    public GameObject[] TxTs;
    public GameObject FolderPrefab;
    public GameObject InteractCanvas;
    public TMP_Text InteractCanvasText;
    public KeyCode KeyPressed;
    public FirstPersonCharacterController Player;
    private bool _isPlayerInside;
    private bool _playAlphaAnimation;
    private float _startTimeAnimation;
    private bool _endAlphaAnimation;

    public static bool EmptyOperation;
    private int TrashItemsCount()
    {
        return transform.Cast<Transform>().Sum(child => child.childCount);
    }

    private void EmptyTrashBin(bool delete)
    {
        EmptyOperation = true;
        foreach (Transform child in transform)
        {
            for (var i = child.childCount - 1; i >= 0; i--)
            {
                var toDestroy = child.GetChild(i).gameObject;
                if (delete)
                {
                    toDestroy.transform.GetComponent<Grabber>().GetReferred().PermDelete();
                }
                Destroy(toDestroy);
            }
        }
    }
    
    public void PopulateTrashBin()
    {
        EmptyTrashBin(false);
        EmptyOperation = false;
        var j = 0;
        for (var i = 0; i < Folder.TrashBin.GetChildren().Count; i++)
        {
            j++;
            var iFolder = Folder.TrashBin.GetChildren()[i];
            var folderInstantiated = Instantiate(PickPrefab(iFolder),transform.GetChild(i));
            var t = folderInstantiated.transform;
            t.GetComponent<DoorController>().DirectionFrontText.text = iFolder.GetName();
            t.GetComponent<DoorController>().enabled = false;
            t.GetComponent<Animator>().enabled = false;
            t.GetComponent<BoxCollider>().enabled = false;
            t.GetComponentInChildren<Grabber>().SetReferred(iFolder);
            t.localPosition = new Vector3(0f, 0f, 0f);
            t.localRotation = Quaternion.Euler(0f, -90f, -90f);
            t.localScale *= 0.25f;
        }
        for (var i = 0; i < Folder.TrashBin.GetFiles().Count; i++)
        {
            var iFile = Folder.TrashBin.GetFiles()[i];
            var fileInstantiated = Instantiate(PickPrefab(iFile), transform.GetChild(i + j));
            var t = fileInstantiated.transform;
            t.GetComponent<Grabber>().SetReferred(iFile);
            t.localPosition = new Vector3(0f, 0f, 0f);
            t.localRotation = Quaternion.Euler(0f, 0f, 0f);
            t.localScale *= 0.5f;
        }
    }
    private GameObject PickPrefab(Grabbable grabbable)
    {
        switch (grabbable)
        {
            case Folder:
                return FolderPrefab;
            case RoomFile roomFile:
                var sizeIndex = roomFile.GetSize() >= MediumSizeMin ? (roomFile.GetSize() >= LargeSizeMin ? 2 : 1) : 0;
                return roomFile.GetFormat() switch
                {
                    "mp3" => Mp3S[sizeIndex],
                    "pdf" => PdFs[sizeIndex],
                    "zip" => ZiPs[sizeIndex],
                    "mov" => MoVs[sizeIndex],
                    "jpeg" => JpeGs[sizeIndex],
                    "png" => PnGs[sizeIndex],
                    "doc" => DoCs[sizeIndex],
                    "txt" => TxTs[sizeIndex],
                    _ => null
                };
        }
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && TrashItemsCount() > 0)
        {
            InteractCanvasText.text = "Premi E per svuotare il cestino";
            InteractCanvas.SetActive(true);
            _isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isPlayerInside = false;
            InteractCanvas.SetActive(false);
            InteractCanvasText.text = "Premi E per interagire";
        }
    }

    private void Update()
    {
        if (!_isPlayerInside) return;
        if (Input.GetKeyDown(KeyPressed))
        {
            Player.IgnoreInput();
            _playAlphaAnimation = true;
            InteractCanvas.SetActive(false);
            InteractCanvasText.text = "Premi E per interagire";
        }
        if (_playAlphaAnimation)
        {
            for (var i = 0; i < TrashItemsCount(); i++)
            {
                var actualChild = transform.GetChild(i).GetChild(0);
                actualChild.localScale *= 0.8f;
                if (actualChild.localScale.x <= 0.0001f)
                {
                    _playAlphaAnimation = false;
                    _endAlphaAnimation = true;
                }
            }
        }
        if (_endAlphaAnimation)
        {
            _endAlphaAnimation = false;
            _isPlayerInside = false;
            EmptyTrashBin(true);
            Player.ReactivateInput();
            NotificationManager.Notify(Operation.TrashBinEmpty);
        }
    }
}
