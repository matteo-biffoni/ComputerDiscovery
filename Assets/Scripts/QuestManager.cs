using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{

    public List<string> Quest1FormatChecker(Folder actualFolderStructure)
    {
        var resList = new List<string>();
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
                        resList.Add($"Il file {fileName} è stato posizionato nella cartella sbagliata");
                    }
                    break;
                case "jpeg":
                    if (parentName != "Immagini")
                    {
                        resList.Add($"Il file {fileName} è stato posizionato nella cartella sbagliata");
                    }
                    break;
                case "doc":
                    if (parentName != "Documenti")
                    {
                        resList.Add($"Il file {fileName} è stato posizionato nella cartella sbagliata");
                    }
                    break;
                case "pdf":
                    if (parentName != "Documenti")
                    {
                        resList.Add($"Il file {fileName} è stato posizionato nella cartella sbagliata");
                    }
                    break;
                case "txt":
                    if (parentName != "Documenti")
                    {
                        resList.Add($"Il file {fileName} è stato posizionato nella cartella sbagliata");
                    }
                    break;
                case "mp3":
                    if (parentName != "Audio")
                    {
                        resList.Add($"Il file {fileName} è stato posizionato nella cartella sbagliata");
                    }
                    break;
                case "mov":
                    if (parentName != "Video")
                    {
                        resList.Add($"Il file {fileName} è stato posizionato nella cartella sbagliata");
                    }
                    break;
            }
        }
        return resList;
    }
    public bool Quest1CountChecker(Folder quest1, Folder actualFolderStructure)
    {
        return quest1.GetAllFiles().Count == actualFolderStructure.GetAllFiles().Count;
    }
}
