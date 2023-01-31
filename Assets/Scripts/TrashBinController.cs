using System.Collections;
using System.Collections.Generic;
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

    private void EmptyTrashBin()
    {
        foreach (Transform child in transform)
        {
            for (var i = child.childCount - 1; i >= 0; i--)
            {
                Destroy(child.GetChild(i).gameObject);
            }
        }
    }
    public void PopulateTrashBin()
    {
        EmptyTrashBin();
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
            Debug.Log("File instantiated inside trash bin");
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
}
