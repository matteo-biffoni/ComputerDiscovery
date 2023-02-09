using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    public static List<string> ImageNamesAtQuest2Start;

    public static void Quest1FormatChecker(Folder actualFolderStructure)
    {
        var wrongAllocatedFilesList = new List<string>(); //lista di file non posizionati correttamente
        var filesActual = actualFolderStructure.GetAllFiles();
        foreach (var file in filesActual)
        {
            var parentName = file.GetParent().GetName();
            var fileName = file.GetName();
            switch (file.GetFormat())
            {
                case "png":
                    if (parentName != "Immagini")
                    {
                        wrongAllocatedFilesList.Add($"- Il file '{fileName}' deve essere posizionato nella cartella Immagini");
                    }
                    break;
                case "jpeg":
                    if (parentName != "Immagini")
                    {
                        wrongAllocatedFilesList.Add($"- Il file '{fileName}' deve essere posizionato nella cartella Immagini");
                    }
                    break;
                case "doc":
                    if (parentName != "Documenti")
                    {
                        wrongAllocatedFilesList.Add($"- Il file '{fileName}' deve essere posizionato nella cartella Documenti");
                    }
                    break;
                case "pdf":
                    if (parentName != "Documenti")
                    {
                        wrongAllocatedFilesList.Add($"- Il file '{fileName}' deve essere posizionato nella cartella Immagini");
                    }
                    break;
                case "txt":
                    if (parentName != "Documenti")
                    {
                        wrongAllocatedFilesList.Add($"- Il file '{fileName}' deve essere posizionato nella cartella Documenti");
                    }
                    break;
                case "mp3":
                    if (parentName != "Audio")
                    {
                        wrongAllocatedFilesList.Add($"- Il file '{fileName}' deve essere posizionato nella cartella Audio");
                    }
                    break;
                case "mov":
                    if (parentName != "Video")
                    {
                        wrongAllocatedFilesList.Add($"- Il file '{fileName}' deve essere posizionato nella cartella Video");
                    }
                    break;
            }
        }
        LavagnettaManager.WriteOnLavagnetta(wrongAllocatedFilesList, "INFORMAZIONI");
        //Check fine quest, tutti i file allocati correttamente
        if (filesActual.Count == 8 && wrongAllocatedFilesList.Count == 0) {
            HouseManager.ActualQuest = 2;
            LavagnettaManager.WriteOnLavagnetta(null, "BEN FATTO!"); //messaggio fine quest
            
            NotificationManager.Instance.StartCoroutine(ShowLastNotifyAndNotifyQuest1());
        }
    }

    private static IEnumerator ShowLastNotifyAndNotifyQuest1()
    {
        yield return new WaitForSeconds(2f);
        yield return NotificationManager.QuestNotify("Lamp ti sta aspettando! :)");
    }
    public static bool Quest1CountChecker(Folder quest1, Folder actualFolderStructure)
    {
        return quest1.GetAllFiles().Count == actualFolderStructure.GetAllFiles().Count;
    }
    
    public static void Quest2FormatChecker()
    {
        var notRenamedFiles = new List<string>(); //lista di file non ancora rinominati
        var filesActual = Folder.Root.GetAllFiles(); //prendo tutti i file nella cartella Immagini
        foreach (var file in filesActual)
        {
            var fileName = file.GetName();
            if (ImageNamesAtQuest2Start.Contains(fileName))
            {
                notRenamedFiles.Add($"- Il file '{fileName}' deve essere rinominato");
            }
        }
        LavagnettaManager.WriteOnLavagnetta(notRenamedFiles, "INFORMAZIONI");
        //Check fine quest, tutti i file allocati correttamente
        if (notRenamedFiles.Count == 0) {
            HouseManager.ActualQuest = 3;
            LavagnettaManager.WriteOnLavagnetta(null, "BEN FATTO!"); //messaggio fine quest
            NotificationManager.Instance.StartCoroutine(NotificationManager.QuestNotify("Lamp ti sta aspettando! :)"));
        }
    }

    public static void Quest4FormatChecker()
    {
        var messages = new List<string>();
        if (!(Folder.Root.GetChildren().Exists(folder => folder.GetName() == "Viaggi") || Folder.Root.GetChildren().Exists(folder => folder.GetName() == "viaggi")))
        {
            messages.Add("Crea una nuova cartella 'Viaggi' nel Desktop");
        }
        else
        {
            string[] possiblePath1 = { "Desktop", "Viaggi" };
            string[] possiblePath2 = { "Desktop", "viaggi" };
            var viaggi = Folder.GetFolderFromAbsolutePath(possiblePath1, Folder.Root);
            if (viaggi == null)
            {
                viaggi = Folder.GetFolderFromAbsolutePath(possiblePath2, Folder.Root);
            }

            if (viaggi.GetFiles().Count < 5)
            {
                messages.Add($"Devi ancora posizionare {5 - viaggi.GetFiles().Count} file nella cartella Viaggi");
            }
            else
            {
                HouseManager.ActualQuest = 5;
                LavagnettaManager.WriteOnLavagnetta(null, "BEN FATTO!"); //messaggio fine quest
                NotificationManager.Instance.StartCoroutine(NotificationManager.QuestNotify("Lamp ti sta aspettando! :)"));
                return;
            }
        }
        LavagnettaManager.WriteOnLavagnetta(messages, "INFORMAZIONI");
    }
    
    public static void Quest6FormatChecker()
        {
            var messages = new List<string>();
            var Viaggio = Folder.Root.GetChildren().Find(folder => folder.GetName() == "Viaggi");
            if (Viaggio == null)
            {
                Viaggio = Folder.Root.GetChildren().Find(folder => folder.GetName() == "viaggi");
            }
            if (!(Viaggio.GetChildren().Exists(folder => folder.GetName() == "immagini e video") || Viaggio.GetChildren().Exists(folder => folder.GetName() == "Immagini e video")))
            {
                messages.Add("Crea una nuova cartella 'immagini e video' nella cartella 'Viaggi'");
            }
            else
            {
                
                string[] possiblePath1 = { "Desktop", Viaggio.GetName(), "immagini e video"};
                string[] possiblePath2 = { "Desktop", Viaggio.GetName(), "immagini e video"};
                var ImmaginiVideo = Folder.GetFolderFromAbsolutePath(possiblePath1, Folder.Root);
                if (ImmaginiVideo == null)
                {
                    ImmaginiVideo = Folder.GetFolderFromAbsolutePath(possiblePath2, Folder.Root);
                }
                foreach (var file in ImmaginiVideo.GetFiles().FindAll(file =>
                             file.GetFormat() != "png" && file.GetFormat() != "jpeg" && file.GetFormat() != "mov"))
                {
                    messages.Add($"Il file {file.GetName()} deve essere portato fuori dalla cartella 'Immagini e video'");
                    
                }
                
                var FilesToMove = Viaggio.GetFiles().FindAll(file => file.GetFormat() == "png" || file.GetFormat() == "jpeg" || file.GetFormat() == "mov");
                if (FilesToMove.Count != 0)
                {
                    for (int i = 0; i < FilesToMove.Count; i++)
                    {
                        messages.Add(
                            $"Il file {FilesToMove[i].GetName()} deve essere spostato nella cartella 'Immagini e Video'");
                    }
                }

                if (messages.Count == 0)
                {
                    messages.Add("Crea una copia della cartella 'Foto e video' e consegnala ad AD5L");
                }
            }
            LavagnettaManager.WriteOnLavagnetta(messages, "INFORMAZIONI");
        }
}
