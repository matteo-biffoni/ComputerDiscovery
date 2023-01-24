using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Formatting = Newtonsoft.Json.Formatting;
using Object = UnityEngine.Object;
using Random = System.Random;

public class HouseManager : MonoBehaviour
{

    public float RoomLenght;
    public float RoomHeight;

    public LayerMask RoomLayer;

    public GameObject[] RoomsPrefabs;

    public GameObject[] EntrancesPrefabs;

    public string FileName = "FileSystem";

    public Magnet0Movement Player;

    public GameObject MainRoomGo;

    private Folder _quest1;

    private static List<string> imageFileNames = new() { "Gatto", "Cane", "Viaggio", "Prato", "Ape", "New York", "Roma", "Oculus" };
    private static List<string> docFileNames = new () { "Passaporto", "Carta d'identità", "Patente", "Tessera sanitaria", "Biglietto del treno", "Tesi", "Assicurazione auto", "Ricetta" };
    private static List<string> multimediaFileNames = new () { "Recita", "Concerto", "Audizione", "Spettacolo", "Provino", "Shakira", "Beethoven", "John Lennon" };
    
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

    private void Start()
    {
        Folder.MainRoomGo = MainRoomGo;
        var mainRoomPlayerDetector = transform.GetChild(0).AddComponent<PlayerDetector>();
        mainRoomPlayerDetector.SetFolderReferred(Folder.MainRoom);
        InstantiateScene(true);
        RetrieveQuest1();
        SpawnObjectsForQuest1();
    }

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
    private void SpawnObjectsForQuest1()
    {
        Transform quest1ObjectsSpawner =
            transform.Find("MainRoom").Find("MainRoom_Outside").Find("Quest1_ObjSpawners");
        List<GameObject> objectSpawners = new List<GameObject>();
        for (int i = 0; i < 8; i++)
        {
            objectSpawners.Add(quest1ObjectsSpawner.GetChild(i).gameObject);
        }

        Random random = new Random();
        int indiceNome = random.Next(0, imageFileNames.Count);
        string nomeImmagine1 = imageFileNames[indiceNome];
        imageFileNames.Remove(nomeImmagine1);
        indiceNome = random.Next(0, imageFileNames.Count);
        string nomeImmagine2 = imageFileNames[indiceNome];
        imageFileNames.Remove(nomeImmagine2);
        nomeImmagine1 += ".png";
        nomeImmagine2 += ".jpg";
        var immagine1 = new RoomFile(nomeImmagine1, "png", true, random.Next(1, 150), null);
        var immagine2 = new RoomFile(nomeImmagine2, "jpeg", true, random.Next(1, 150), null);
        indiceNome = random.Next(0, docFileNames.Count);
        string nomeDocumento1 = docFileNames[indiceNome];
        docFileNames.Remove(nomeDocumento1);
        indiceNome = random.Next(0, docFileNames.Count);
        string nomeDocumento2 = docFileNames[indiceNome];
        docFileNames.Remove(nomeDocumento2);
        indiceNome = random.Next(0, docFileNames.Count);
        string nomeDocumento3 = docFileNames[indiceNome];
        docFileNames.Remove(nomeDocumento3);
        nomeDocumento1 += ".docx";
        nomeDocumento2 += ".pdf";
        nomeDocumento3 += ".txt";
        var documento1 = new RoomFile(nomeDocumento1, "doc", true, random.Next(1, 150), null);
        var documento2 = new RoomFile(nomeDocumento2, "pdf", true, random.Next(1, 150), null);
        var documento3 = new RoomFile(nomeDocumento3, "txt", true, random.Next(1, 150), null);
        indiceNome = random.Next(0, multimediaFileNames.Count);
        string nomeMultFile1 = multimediaFileNames[indiceNome];
        multimediaFileNames.Remove(nomeMultFile1);
        indiceNome = random.Next(0, multimediaFileNames.Count);
        string nomeMultFile2 = multimediaFileNames[indiceNome];
        multimediaFileNames.Remove(nomeMultFile2);
        nomeMultFile1 += ".mp3";
        nomeMultFile2 += ".mov";
        var multFile1 = new RoomFile(nomeMultFile1, "mp3", true, random.Next(1, 150), null);
        var multFile2 = new RoomFile(nomeMultFile2, "mov", true, random.Next(1, 150), null);
        var fileBonus = new RoomFile("", "", true, 0f, null);
        var nomeFileBonus = "";
        var formatoBonusIndice = random.Next(0, 2);
        switch (formatoBonusIndice)
        {
            case 0:
                formatoBonusIndice = random.Next(0, 1);
                indiceNome = random.Next(0, imageFileNames.Count);
                nomeFileBonus = imageFileNames[indiceNome];
                imageFileNames.Remove(nomeFileBonus);
                switch (formatoBonusIndice)
                {
                    case 0:
                        nomeFileBonus += ".png";
                        fileBonus = new RoomFile(nomeFileBonus, "png", true, random.Next(1, 150), null);
                        break;
                    case 1:
                        nomeFileBonus += ".jpg";
                        fileBonus = new RoomFile(nomeFileBonus, "jpeg", true, random.Next(1, 150), null);
                        break;
                }
                break;
            case 1:
                formatoBonusIndice = random.Next(0, 2);
                indiceNome = random.Next(0, docFileNames.Count);
                nomeFileBonus = docFileNames[indiceNome];
                docFileNames.Remove(nomeFileBonus);
                switch (formatoBonusIndice)
                {
                    case 0:
                        nomeFileBonus += ".docx";
                        fileBonus = new RoomFile(nomeFileBonus, "doc", true, random.Next(1, 150), null);
                        break;
                    case 1:
                        nomeFileBonus += ".pdf";
                        fileBonus = new RoomFile(nomeFileBonus, "pdf", true, random.Next(1, 150), null);
                        break;
                    case 2:
                        nomeFileBonus += ".txt";
                        fileBonus = new RoomFile(nomeFileBonus, "txt", true, random.Next(1, 150), null);
                        break;
                }
                break;
            case 2:
                formatoBonusIndice = random.Next(0, 1);
                indiceNome = random.Next(0, multimediaFileNames.Count);
                nomeFileBonus = multimediaFileNames[indiceNome];
                multimediaFileNames.Remove(nomeFileBonus);
                switch (formatoBonusIndice)
                {
                    case 0:
                        nomeFileBonus += ".mp3";
                        fileBonus = new RoomFile(nomeFileBonus, "mp3", true, random.Next(1, 150), null);
                        break;
                    case 1:
                        nomeFileBonus += ".mov";
                        fileBonus = new RoomFile(nomeFileBonus, "mov", true, random.Next(1, 150), null);
                        break;
                }
                break;
        }
        // Ora gli 8 file sono: { immagine1, immagine2, documento1, documento2, documento3, multFile1, multFile2, fileBonus }
        var extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        var objToSpawn = PickPrefabFromFile(immagine1);
        var instantiated = Instantiate(objToSpawn, extracted.transform);
        var fileGrabber = instantiated.transform.GetComponent<FileGrabber>();
        fileGrabber.SetFile(immagine1);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(immagine2);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<FileGrabber>();
        fileGrabber.SetFile(immagine2);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(documento1);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<FileGrabber>();
        fileGrabber.SetFile(documento1);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(documento2);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<FileGrabber>();
        fileGrabber.SetFile(documento2);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(documento3);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<FileGrabber>();
        fileGrabber.SetFile(documento3);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(multFile1);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<FileGrabber>();
        fileGrabber.SetFile(multFile1);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(multFile2);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<FileGrabber>();
        fileGrabber.SetFile(multFile2);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(fileBonus);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<FileGrabber>();
        fileGrabber.SetFile(fileBonus);
    }

    private void RetrieveQuest1()
    {
        _quest1 = Folder.RetrieveQuest1();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            var questManager = GetComponent<QuestManager>();
            var formatErrors = questManager.Quest1FormatChecker(Folder.Root);
            if (formatErrors.Count == 0)
            {
                Debug.Log("Nessun file fuori posto");
                if (questManager.Quest1CountChecker(_quest1, Folder.Root))
                {
                    Debug.Log("Hai finito");
                }
                else
                {
                    Debug.Log("Ti mancano ancora dei file da posizionare");
                }
            }
            else
            {
                foreach (var error in formatErrors)
                {
                    Debug.Log(error);
                }
            }
        }
        if (!Folder.DirtyAfterInsertion) return;
        Folder.DirtyAfterInsertion = false;
        Destroy(Folder.Root.GetContainer());
        StartCoroutine(DelayInstantiation());
    }

    private IEnumerator DelayInstantiation()
    {
        yield return new WaitForFixedUpdate();
        InstantiateScene(false);
        Player.gameObject.transform.position = Folder.GetFolderFromAbsolutePath(Player.GetRoomIn().GetAbsolutePath().Split("/"), Folder.Root)
            .GetContainer().transform.position;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void InstantiateScene(bool firstTime)
    {
        try
        {
            Folder.CurrentFileName = FileName;
            var desktop = Folder.GenerateFolderStructureFromFile();
            if (desktop == null)
            {
                throw new Exception("Could not generate folder structure");
            }
            Folder.InstantiateFolder(desktop, transform, Direction.North, RoomsPrefabs, Entrance.None, EntrancesPrefabs,
                RoomLenght, RoomHeight, (int)Mathf.Log(RoomLayer.value, 2));
            Folder.Root = desktop;
            if (firstTime)
            {
                Folder.Root.ActivateRoomComponents(true);
            }
            else
            {
                var newPlayerFolder = Folder.GetFolderFromAbsolutePath(Player.GetRoomIn().GetAbsolutePath().Split("/"), Folder.Root);
                newPlayerFolder.GetFather()?.GetFather()?.ActivateRoomComponents(true);
                newPlayerFolder.GetFather()?.ActivateRoomComponents(true);
                newPlayerFolder.ActivateRoomComponents(true);
                Player.HouseLayoutChangingCompleted(newPlayerFolder);
            }
        }
        catch (UnsupportedFileFormatException unsupportedFileFormatException)
        {
            Debug.Log(unsupportedFileFormatException.Message);
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            Debug.Log(fileNotFoundException.Message);
        }
    }
}

public enum Direction
{
    North = 0,
    East = 90,
    South = 180,
    West = 270
}

public enum Entrance
{
    None,
    L2,
    L1,
    M,
    R1,
    R2
}

public class RoomFile
{
    private readonly string _name;
    private readonly string _format;
    private readonly bool _integrity;
    private readonly float _size;
    private readonly Folder _parent;

    public RoomFile(string name, string format, bool integrity, float size, Folder parent)
    {
        _name = name;
        _format = format;
        _integrity = integrity;
        _size = size;
        _parent = parent;
    }

    public string GetName()
    {
        return _name;
    }

    public string GetFormat()
    {
        return _format;
    }

    public bool GetIntegrity()
    {
        return _integrity;
    }

    public float GetSize()
    {
        return _size;
    }

    public Folder GetParent()
    {
        return _parent;
    }
    
    public string GetAbsolutePath()
    {
        return _parent.GetAbsolutePath() + "/" + _name;
    }
}

public class Folder
{
    public static bool DirtyAfterInsertion;
    public static string CurrentFileName;
    public static Folder Root;
    private GameObject _container;
    private readonly Folder _father;
    private readonly List<Folder> _children;
    private readonly List<RoomFile> _files;
    private readonly string _name;
    public static readonly Folder MainRoom = new("Main Room", null);
    public static GameObject MainRoomGo;
    private  BachecaFileController _bacheca;



    public List<RoomFile> GetAllFiles()
    {
        var list = new List<RoomFile>();
        foreach (var file in _files)
        {
            list.Add(file);
        }

        foreach (var child in _children)
        {
            list.AddRange(child.GetAllFiles());
        }
        return list;
    }
    private Folder(string name, Folder father)
    {
        _father = father;
        _name = name;
        _children = new List<Folder>();
        _files = new List<RoomFile>();
    }

    public static void InsertNewFolder(string newFolderName, Folder father)
    {
        var newFolder = new Folder(newFolderName, father);
        father.AddChild(newFolder);
        WriteNewFolderStructureToFile();
        DirtyAfterInsertion = true;
    }

    private void SetBacheca(BachecaFileController bachecaFileController)
    {
        _bacheca = bachecaFileController;
    }

    public void InsertFile(FileGrabber fileGrabber)
    {
        var file = fileGrabber.GetFile();
        _files.Add(file);
        
        WriteNewFolderStructureToFile();
        DirtyAfterInsertion = true;
    }

    public static void InsertNewFile(RoomFile newFile, Folder father)
    {
        father._files.Add(newFile);
        WriteNewFolderStructureToFile();
        DirtyAfterInsertion = true;
    }

    public static bool IsMainRoomVisible()
    {
        return MainRoomGo.activeSelf;
    }

    public static void ShowMainRoom(bool show)
    {
        MainRoomGo.SetActiveRecursivelyExt(show);
    }

    public static Folder GetFolderFromAbsolutePath(string[] absolutePath, Folder root)
    {
        if (absolutePath.Length == 1 && root._name == absolutePath[0]) return root;
        if (absolutePath.Length == 1) return null;
        foreach (var child in root._children)
        {
            if (child._name == absolutePath[1])
            {
                if (absolutePath.Length == 2) return child;
                return GetFolderFromAbsolutePath(absolutePath[1..], child);
            }
        }
        return null;
    }

    public int GetChildrenCount()
    {
        return _children.Count;
    }

    private void AddChild(Folder folder)
    {
        _children.Add(folder);
    }

    private void SetContainer(GameObject container)
    {
        _container = container;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ActivateRoomComponents(bool active)
    {
        var boxCollider = _container.GetComponent(typeof(BoxCollider)) as BoxCollider;
        if (boxCollider != null) boxCollider.enabled = active;
        _container.transform.GetChild(0).gameObject.SetActive(active);
        _container.transform.GetChild(1).gameObject.SetActive(active);
    }

    public string GetAbsolutePath()
    {
        if (_father == null) return _name;
        return _father.GetAbsolutePath() + "/" + _name;
    }

    public GameObject GetContainer()
    {
        return _container;
    }

    public Folder GetChildrenFromDoorController(DoorController controller)
    {
        var doorControllers = _container.transform.GetChild(0).GetComponentsInChildren(typeof(DoorController));
        var index = Array.IndexOf(doorControllers, controller);
        return index != -1 ? _children[index] : null;
    }

    /*public void ActivateChildComponents(DoorController controller, bool active)
    {
        GetChildrenFromDoorController(controller).ActivateRoomComponents(active);
    }*/

    public Folder GetFather()
    {
        return _father;
    }

    public string GetName()
    {
        return _name;
    }

    public List<RoomFile> GetFiles()
    {
        return _files;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void ActivateGreatGrandFather(bool active)
    {
        if (_father is { _father: { _father: {  } } })
        {
            _father._father._father.ActivateRoomComponents(active);
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public static Folder GetFolderFromCollider(Folder folder, Collider collider)
    {
        return folder.GetContainer().GetComponent(typeof(BoxCollider)) == collider ?
            folder
            :
            folder._children.Select(child => GetFolderFromCollider(child, collider)).FirstOrDefault(folderAnalyzed => folderAnalyzed != null);
    }

    public bool IsChildNameAvailable(string name)
    {
        return _children.All(child => child._name != name.Trim());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void InstantiateFolder(Folder folder, Transform parentTransform, Direction direction, GameObject[] roomsPrefabs, Entrance entrance, GameObject[] entrancesPrefabs, float roomLength, float roomHeight, LayerMask roomLayer)
    {
        var container = new GameObject(folder._name)
        {
            layer = roomLayer
        };
        var collider = container.AddComponent(typeof(BoxCollider)) as BoxCollider;
        if (collider != null)
        {
            var center = container.transform.position;
            center += new Vector3(0f, roomHeight / 2, 0f);
            collider.center = center;
            collider.size = new Vector3(roomLength, roomHeight, roomLength);
            collider.isTrigger = true;
            collider.enabled = false;
        }
        var playerDetector = container.AddComponent(typeof(PlayerDetector)) as PlayerDetector;
        if (playerDetector != null)
        {
            playerDetector.SetFolderReferred(folder);
        }
        Vector3 offset;
        switch (direction)
        {
            case Direction.North:
                offset = new Vector3(roomLength, 0f, 0f);
                container.transform.position = parentTransform.position + offset;
                break;
            case Direction.West:
                offset = new Vector3(0f, 0f, roomLength);
                container.transform.position = parentTransform.position + offset;
                break;
            case Direction.South:
                offset = new Vector3(-roomLength, 0f, 0f);
                container.transform.position = parentTransform.position + offset;
                break;
            case Direction.East:
                offset = new Vector3(0f, 0f, -roomLength);
                container.transform.position = parentTransform.position + offset;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        var roomPre = Object.Instantiate(roomsPrefabs[folder._children.Count], container.transform);
        roomPre.transform.Find("Wall_North").Find("RoomLabel").GetComponent<TMP_Text>().text = folder._name;
        // prendo il Gameobject bacheca figlio della stanza per andare a settargli la folder a cui si riferisce
        var bacheca = roomPre.transform.Find("Bacheca").GetComponent<BachecaFileController>();
        bacheca.SetFolder(folder);
        var entrancePre = Object.Instantiate(entrancesPrefabs[(int)entrance], container.transform);
        container.transform.Rotate(0f, (int) direction, 0f, Space.Self);
        container.transform.parent = parentTransform;
        foreach (var child in folder._children)
        {
            var tuple = ComputeAbsoluteDirectionAndEntranceForChildren(folder, child, direction);
            InstantiateFolder(child, container.transform, tuple.Item1, roomsPrefabs, tuple.Item2, entrancesPrefabs, roomLength, roomHeight, roomLayer);
        }
        folder.SetContainer(container);
        folder.SetBacheca(bacheca);
        roomPre.SetActive(false);
        entrancePre.SetActive(false);
    }

    private static (Direction, Entrance) ComputeAbsoluteDirectionAndEntranceForChildren(Folder folder, Folder children, Direction parentDirection)
    {
        int index;
        switch (folder._children.Count)
        {
            case 0:
                break;
            case 1:
                return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.M);
            case 2:
                index = folder._children.IndexOf(children);
                switch (index)
                {
                    case 0:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.M);
                    case 1:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.M);
                }
                break;
            case 3:
                index = folder._children.IndexOf(children);
                switch (index)
                {
                    case 0:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.M);
                    case 1:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.M);
                    case 2:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.M);
                }
                break;
            case 4:
                index = folder._children.IndexOf(children);
                switch (index)
                {
                    case 0:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.M);
                    case 1:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.L1);
                    case 2:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.R1);
                    case 3:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.M);
                }
                break;
            case 5:
                index = folder._children.IndexOf(children);
                switch (index)
                {
                    case 0:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.L1);
                    case 1:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.R1);
                    case 2:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.M);
                    case 3:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.L1);
                    case 4:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.R1);
                }
                break;
            case 6:
                index = folder._children.IndexOf(children);
                switch (index)
                {
                    case 0:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.L1);
                    case 1:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.R1);
                    case 2:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.L1);
                    case 3:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.R1);
                    case 4:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.L1);
                    case 5:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.R1);
                }
                break;
            case 7:
                index = folder._children.IndexOf(children);
                switch (index)
                {
                    case 0:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.L1);
                    case 1:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.R1);
                    case 2:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.L2);
                    case 3:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.M);
                    case 4:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.R2);
                    case 5:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.L1);
                    case 6:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.R1);
                }
                break;
            case 8:
                index = folder._children.IndexOf(children);
                switch (index)
                {
                    case 0:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.L2);
                    case 1:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.M);
                    case 2:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.R2);
                    case 3:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.L1);
                    case 4:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.R1);
                    case 5:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.L2);
                    case 6:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.M);
                    case 7:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.R2);
                }
                break;
            case 9:
                index = folder._children.IndexOf(children);
                switch (index)
                {
                    case 0:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.L2);
                    case 1:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.M);
                    case 2:
                        return (ComputeAbsoluteDirection(Direction.West, parentDirection), Entrance.R2);
                    case 3:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.L2);
                    case 4:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.M);
                    case 5:
                        return (ComputeAbsoluteDirection(Direction.North, parentDirection), Entrance.R2);
                    case 6:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.L2);
                    case 7:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.M);
                    case 8:
                        return (ComputeAbsoluteDirection(Direction.East, parentDirection), Entrance.R2);
                }
                break;
        }
        return (parentDirection, Entrance.M);
    }


    private static void WriteNewFolderStructureToFile()
    {
        var jsonFolderStructure = ConvertFolderStructureToJsonFolderStructure(Root);
        JsonFolder.SaveToFile(CurrentFileName, jsonFolderStructure);
    }
    
    private static Direction ComputeAbsoluteDirection(Direction direction, Direction parentDirection)
    {
        return (Direction) (((int) parentDirection + (int) direction) % 360);
    }

    public static Folder RetrieveQuest1()
    {
        var jsonFolderStructure = JsonFolder.FromFileName("Quest1");
        return ConvertJsonFolderStructureToFolderStructure(jsonFolderStructure, null);
    }

    public static Folder GenerateFolderStructureFromFile()
    {
        var jsonFolderStructure = JsonFolder.FromFileName(CurrentFileName);
        return ConvertJsonFolderStructureToFolderStructure(jsonFolderStructure, null);
    } 
    
    private static Folder ConvertJsonFolderStructureToFolderStructure(JsonFolder jsonFolder, Folder father)
    {
        var folder = new Folder(jsonFolder.Name, father);
        foreach (var file in jsonFolder.Files)
        {
            folder._files.Add(new RoomFile(file.Name, file.Format, file.Integrity, file.Size, folder));
        }
        foreach (var child in jsonFolder.Children)
        {
            folder.AddChild(ConvertJsonFolderStructureToFolderStructure(child, folder));
        }
        return folder;
    }

    private static JsonFolder ConvertFolderStructureToJsonFolderStructure(Folder folder)
    {
        var jsonFolder = new JsonFolder
        {
            Name = folder._name,
            Children = new List<JsonFolder>(),
            Files = new List<JsonFile>()
        };
        // ciclo per aggiungere i file di ogni cartella nella struttura json
        foreach (var file in folder._files)
        {
            jsonFolder.Files.Add(new JsonFile
            {
                Name = file.GetName(),
                Format = file.GetFormat(),
                Integrity = file.GetIntegrity(),
                Size = file.GetSize()
            });
        }
        foreach (var child in folder._children)
        {
            jsonFolder.Children.Add(ConvertFolderStructureToJsonFolderStructure(child));
        }
        return jsonFolder;
    }
}

internal class UnsupportedFileFormatException : Exception
{
    public UnsupportedFileFormatException(string message) : base(message) {}
}

public class JsonFile
{
    [JsonProperty("Name")]
    public string Name { get; set; }
    
    [JsonProperty("Format")]
    public string Format { get; set; }
    
    [JsonProperty("Integrity")]
    public bool Integrity { get; set; }
    
    [JsonProperty("Size")]
    public float Size { get; set; }
    
    
    private static JsonFile FromJson(string json)
    {
        return JsonConvert.DeserializeObject<JsonFile>(json);
    }
    private static string ToJson(JsonFile file)
    {
        return JsonConvert.SerializeObject(file, Formatting.Indented);
    }

    public override string ToString()
    {
        return ToJson(this);
    }
}

public class JsonFolder
{
    [JsonProperty("Name")]
    public string Name { get; set; }
    
    [JsonProperty("Children")]
    public List<JsonFolder> Children { get; set; }
    
    [JsonProperty("Files")]
    public List<JsonFile> Files { get; set; }

    private static JsonFolder FromJson(string json)
    {
        return JsonConvert.DeserializeObject<JsonFolder>(json);
    }

    private static string ToJson(JsonFolder folder)
    {
        return JsonConvert.SerializeObject(folder, Formatting.Indented);
    }

    public static JsonFolder FromFileName(string filename)
    {
        if (filename.Contains(".") && !filename.EndsWith(".json"))
        {
            throw new UnsupportedFileFormatException(
                $"Unsupported file format: {filename[filename.LastIndexOf('.')..]}");
        }
        var convertedFileName = filename.EndsWith(".json") ? filename : $"{filename}.json";
        return FromJson(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, convertedFileName)));
    }

    public static void SaveToFile(string filename, JsonFolder folder)
    {
        if (filename.Contains(".") && !filename.EndsWith(".json"))
        {
            throw new UnsupportedFileFormatException(
                $"Unsupported file format: {filename[filename.LastIndexOf('.')..]}");
        }
        var convertedFileName = filename.EndsWith(".json") ? filename : $"{filename}.json";
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, convertedFileName), ToJson(folder));
    }

    public override string ToString()
    {
        return ToJson(this);
    }
}

public static class Utility
{
    
    public static void SetActiveRecursivelyExt(this GameObject obj, bool state)
    {
        obj.SetActive(state);
        foreach (Transform child in obj.transform)
        {
            SetActiveRecursivelyExt(child.gameObject, state);
        }
    }
}
