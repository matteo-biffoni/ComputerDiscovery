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
    
    private void Start()
    {
        Folder.MainRoomGo = MainRoomGo;
        var mainRoomPlayerDetector = transform.GetChild(0).AddComponent<PlayerDetector>();
        mainRoomPlayerDetector.SetFolderReferred(Folder.MainRoom);
        InstantiateScene(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Folder.InsertNewFile(new RoomFile("Ciao bello.png", "png", true, 129.2f), Folder.Root);
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

    public RoomFile(string name, string format, bool integrity, float size)
    {
        _name = name;
        _format = format;
        _integrity = integrity;
        _size = size;
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
            folder._files.Add(new RoomFile(file.Name, file.Format, file.Integrity, file.Size));
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
