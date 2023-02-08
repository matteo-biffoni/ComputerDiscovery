using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    public Image NotificationImage;
    public TMP_Text message;
    public RectTransform backgroundBox;
    public Sprite SuccessSprite;
    public Sprite NotAllowedSprite;
    public Sprite DeleteSprite;
    public Sprite PermDeleteSprite;
    public Sprite AD5LSprite;
    public Sprite LampSprite;
    public Sprite IOSprite;
    public Sprite ZipSprite;
    
    

    private void Awake()
    {
        Instance = this;
    }

    //Notifiche operazioni su file e cartelle
    public static void Notify(Operation operation)
    {
        AudioManager.Play(Instance.transform, AudioManager.Instance.Notification);
        Instance.backgroundBox.gameObject.SetActive(true);
        
        switch (operation)
        {
            //File spostato nel cestino
            case Operation.FileDeleted:
                Instance.message.text = "File spostato nel cestino";
                Instance.NotificationImage.sprite = Instance.DeleteSprite;
                break;
            //File eliminato definitivamente
            case Operation.FilePermDeleted:
                Instance.message.text = "File eliminato definitivamente";
                Instance.NotificationImage.sprite = Instance.PermDeleteSprite;
                break;
            //Cartella spostata nel cestino
            case Operation.FolderDeleted:
                Instance.message.text = "Cartella spostata nel cestino";
                Instance.NotificationImage.sprite = Instance.DeleteSprite;
                break;
            //Cartella eliminata definitivamente
            case Operation.FolderPermDeleted:
                Instance.message.text = "Cartella eliminata definitivamente";
                Instance.NotificationImage.sprite = Instance.PermDeleteSprite;
                break;
            //Creazione cartella
            case Operation.FolderCreated:
                Instance.message.text = "Cartella creata correttamente";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            
            //Cartella mossa correttamente
            case Operation.FolderMoved:
                Instance.message.text = "Cartella spostata in una nuova posizione";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            
            //Cartella mossa correttamente
            case Operation.FileMoved:
                Instance.message.text = "File spostato in una nuova posizione";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            
            //Svuotamento cestino
            case Operation.TrashBinEmpty:
                Instance.message.text = "Cestino svuotato correttamente";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            
            //Rinomina file
            case Operation.FileRenamed:
                Instance.message.text = "File rinominato correttamente";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;

            //Rinomina Cartella
            case Operation.FolderRenamed:
                Instance.message.text = "Cartella rinominata correttamente";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            //Cartella copiata
            case Operation.FolderCopied:
                Instance.message.text = "Cartella copiata correttamente";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            //File copiato
            case Operation.FileCopied:
                Instance.message.text = "File copiato correttamente";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            //Operazione di release non consentita
            case Operation.ReleaseNotAllowed:
                Instance.message.text = "Non puoi lasciare il file o la cartella in questa posizione";
                Instance.NotificationImage.sprite = Instance.NotAllowedSprite;
                break;
            //File ripristinato
            case Operation.FileRestored:
                Instance.message.text = "File ripristinato correttamente";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            //Cartella ripristinata
            case Operation.FolderRestored:
                Instance.message.text = "Cartella ripristinata correttamente";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            //Cartella piena di sottocartelle
            case Operation.FolderFullOfSubfolders:
                Instance.message.text = "Questa cartella ha già il massimo numero di sottocartelle";
                Instance.NotificationImage.sprite = Instance.NotAllowedSprite;
                break;
            //Cartella piena di file
            case Operation.FolderFullOfFiles:
                Instance.message.text = "Questa cartella ha già il massimo numero di file";
                Instance.NotificationImage.sprite = Instance.NotAllowedSprite;
                break;
            case Operation.LockedFunctionality:
                Instance.message.text = "Completa l'obiettivo per sbloccare questa funzionalità";
                Instance.NotificationImage.sprite = Instance.NotAllowedSprite;
                break;
            case Operation.RestoreRedirectedToDesktop:
                Instance.message.text = "Ripristino effettuato nel Desktop";
                Instance.NotificationImage.sprite = Instance.SuccessSprite;
                break;
            case Operation.ReleaseIONotCopy:
                Instance.message.text = "Effettua prima una copia del file";
                Instance.NotificationImage.sprite = Instance.NotAllowedSprite;
                break;
        }
        Instance.StartCoroutine(CloseNotification(2f));
    }

    public static IEnumerator QuestNotify(string message)
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => !Instance.backgroundBox.gameObject.activeSelf);
        Instance.backgroundBox.gameObject.SetActive(true);
        Instance.message.text = message;
        Instance.NotificationImage.sprite = Instance.LampSprite;
    }

    public static void HardCloseNotification()
    {
        Instance.backgroundBox.gameObject.SetActive(false);
    }


    private static IEnumerator CloseNotification(float duration)
    {
        yield return new WaitForSeconds(duration);
        Instance.backgroundBox.gameObject.SetActive(false);
    }

    
}
