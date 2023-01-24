using UnityEngine;

public class BachecaFileController : MonoBehaviour
{

    public float MediumSizeMin = 10.0f;
    public float LargeSizeMin = 50.0f;

    public GameObject[] MP3s;
    public GameObject[] ZIPs;
    public GameObject[] MOVs;
    public GameObject[] PDFs;
    public GameObject[] JPEGs;
    public GameObject[] PNGs;
    public GameObject[] DOCs;
    public GameObject[] TXTs;

    // Setto la variabile Folder e popolo la bacheca di conseguenza
    public void SetFolder(Folder folder)
    {
        // Disattivo gli altri holder (serve nel caso in cui la colonna sia stata modificata, per esempio aggiungendo o rimuovendo un file)
        // che potevano essere eventualmente attivi
        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child)
            {
                foreach (Transform grandGrandChild in grandChild)
                {
                    Destroy(grandGrandChild.gameObject);
                }
            }
            child.gameObject.SetActive(false);
        }
        // Recupero i file della cartella
        var files = folder.GetFiles();
        // Se non ha file, non serve attivare nessun holder
        if (files.Count == 0) return;
        // Recupero l'holder adeguato e lo attivo
        var holderGo = transform.GetChild(files.Count - 1);
        holderGo.gameObject.SetActive(true);
        // Per ogni file della cartella instanzio il modello corrispondente
        for (var i = 0; i < files.Count; i++)
        {
            Instantiate(PickPrefabFromFile(files[i]), holderGo.transform.GetChild(i));
        }
    }

    // Ritorna il prefab corretto in termini di tipo e dimensione
    private GameObject PickPrefabFromFile(RoomFile file)
    {
        var sizeIndex = file.GetSize() >= MediumSizeMin ? (file.GetSize() >= LargeSizeMin ? 2 : 1) : 0;
        return file.GetFormat() switch
        {
            "mp3" => MP3s[sizeIndex],
            "pdf" => PDFs[sizeIndex],
            "zip" => ZIPs[sizeIndex],
            "mov" => MOVs[sizeIndex],
            "jpeg" => JPEGs[sizeIndex],
            "png" => PNGs[sizeIndex],
            "doc" => DOCs[sizeIndex],
            "txt" => TXTs[sizeIndex],
            _ => null
        };
    }
}
