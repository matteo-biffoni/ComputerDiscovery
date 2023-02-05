using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

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
                        wrongAllocatedFilesList.Add($"Il file {fileName} deve essere posizionato nella cartella Immagini");
                    }
                    break;
                case "jpeg":
                    if (parentName != "Immagini")
                    {
                        wrongAllocatedFilesList.Add($"Il file {fileName} deve essere posizionato nella cartella Immagini");
                    }
                    break;
                case "doc":
                    if (parentName != "Documenti")
                    {
                        wrongAllocatedFilesList.Add($"Il file {fileName} deve essere posizionato nella cartella Documenti");
                    }
                    break;
                case "pdf":
                    if (parentName != "Documenti")
                    {
                        wrongAllocatedFilesList.Add($"Il file {fileName} deve essere posizionato nella cartella Immagini");
                    }
                    break;
                case "txt":
                    if (parentName != "Documenti")
                    {
                        wrongAllocatedFilesList.Add($"Il file {fileName} deve essere posizionato nella cartella Documenti");
                    }
                    break;
                case "mp3":
                    if (parentName != "Audio")
                    {
                        wrongAllocatedFilesList.Add($"Il file {fileName} deve essere posizionato nella cartella Audio");
                    }
                    break;
                case "mov":
                    if (parentName != "Video")
                    {
                        wrongAllocatedFilesList.Add($"Il file {fileName} deve essere posizionato nella cartella Video");
                    }
                    break;
            }
        }
        LavagnettaManager.WriteOnLavagnetta(wrongAllocatedFilesList, "INFO");
        //Check fine quest, tutti i file allocati correttamente
        if (filesActual.Count == 8 && wrongAllocatedFilesList.Count == 0) {
            HouseManager.ActualQuest = 2;
            LavagnettaManager.WriteOnLavagnetta(null, "COMPLIMENTI!"); //messaggio fine quest
            NotificationManager.Instance.StartCoroutine(NotificationManager.QuestNotify("Lamp ti sta aspettando! :)"));
        }
    }
    public static bool Quest1CountChecker(Folder quest1, Folder actualFolderStructure)
    {
        return quest1.GetAllFiles().Count == actualFolderStructure.GetAllFiles().Count;
    }
    
    public static void Quest2FormatChecker(Folder ImagesFolder)
    {
        //Ipotizzo che il filesystem caricato per la quest 2 abbia alcuni file nella cartella immagini e 2 sottocartelle 
        //con altri file al loro interno, tutti da rinominare. 
        //Es. foto.jpeg - albero.png - ponte.jpeg - luna.png 
        var toBeRenamedFiles = new List<string>{"foto.jpg","albero.png", "ponte.jpg", "luna.png"};
        var notRenamedFiles = new List<string>(); //lista di file non ancora rinominati
        var filesActual = ImagesFolder.GetAllFiles(); //prendo tutti i file nella cartella Immagini
        foreach (var file in filesActual)
        {
            var fileName = file.GetName();
            if (toBeRenamedFiles.Contains(fileName))
            {
                notRenamedFiles.Add($"Il file '{fileName}' deve essere rinominato");
            }
        }
        LavagnettaManager.WriteOnLavagnetta(notRenamedFiles, "INFO");
        //Check fine quest, tutti i file allocati correttamente
        if (notRenamedFiles.Count == 0) {
            HouseManager.ActualQuest = 3;
            LavagnettaManager.WriteOnLavagnetta(null, "COMPLIMENTI!"); //messaggio fine quest
            NotificationManager.Instance.StartCoroutine(NotificationManager.QuestNotify("Lamp ti sta aspettando! :)"));
        }
    }
}
