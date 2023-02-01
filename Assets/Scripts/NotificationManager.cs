using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    private static NotificationManager Instance;
    public Image NotificationImage;
    public TMP_Text message;
    public RectTransform backgroundBox;
    public Sprite successSprite;
    

    private void Awake()
    {
        Instance = this;
    }

    public static void Notify(Operation operation)
    {
        Instance.backgroundBox.gameObject.SetActive(true);
        switch (operation)
        {
            //Eliminazione file
            case Operation.FileDeleted:
                Instance.message.text = "File eliminato correttamente";
                Instance.NotificationImage.sprite = Instance.successSprite;
                break;
            //Eliminazione definitiva file
            case Operation.FilePermDeleted:
                Instance.message.text = "File eliminato definitivamente";
                Instance.NotificationImage.sprite = Instance.successSprite;
                break;
            //Eliminazione Cartella
            case Operation.FolderDeleted:
                Instance.message.text = "Cartella eliminata correttamente";
                Instance.NotificationImage.sprite = Instance.successSprite;
                break;
            //Eliminazione definitiva Cartella 
            case Operation.FolderPermDeleted:
                Instance.message.text = "Cartella eliminata definitivamente";
                Instance.NotificationImage.sprite = Instance.successSprite;
                break;
            //Creazione cartella
            case Operation.FolderCreated:
                Instance.message.text = "Cartella creata correttamente";
                Instance.NotificationImage.sprite = Instance.successSprite;
                break;
            
            //Cartella mossa correttamente
            case Operation.FolderMoved:
                Instance.message.text = "Cartella spostata correttamente";
                Instance.NotificationImage.sprite = Instance.successSprite;
                break;
            
        }
        Instance.StartCoroutine(CloseNotification(2f));
    }

    private static IEnumerator CloseNotification(float duration)
    {
        yield return new WaitForSeconds(duration);
        Instance.backgroundBox.gameObject.SetActive(false);
    }

    
}
