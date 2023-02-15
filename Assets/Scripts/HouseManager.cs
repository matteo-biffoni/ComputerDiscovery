using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private const string DefaultFileSystem = "FileSystemDefault";

    public PlayerNavigatorManager Player;

    public GameObject MainRoomGo;

    public TrashBinController TrashBinController;

    public static int ActualQuest;

    private static List<string> ImageFileNames;
    private static List<string> DocFileNames;
    private static List<string> MultimediaFileNames;


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

    public static void BackToMainMenu()
    {
        SceneManager.LoadScene("mainmenu");
    }

    private void OnDestroy()
    {
        RestoreFileSystemToDefault();
    }

    private void Awake()
    {
        ActualQuest = 1;
        ImageFileNames = new List<string> { "IimMaAggGiInE", "FfooTOoGRaFia", "IiCCcoONNnAa", "RrriItTRraTtTOo" };
        DocFileNames = new List<string> { "Appunti", "Itinerari Solari", "Regolamento Intergalattico", "Archivio Storico", "Diario Personale", "Racconti Galassia X34", "Elenco Contatti", "Ricette Terrestri" }; 
        MultimediaFileNames = new List<string> { "Buco Nero Yu85", "Passaggio della Cometa R47U2", "Orchestra Galattica", "Tempesta di Meteoriti", "Cascata Terrestre", "Terra: Shakira", "Terra: Beethoven", "Terra: John Lennon" };
        RoomFile.ScoperteFile = null;
        Folder.DirtyAfterInsertion = false;
        Folder.CurrentFileName = null;
        Folder.Root = null;
        Folder.TrashBin = new Folder("TrashBin", null, null, Guid.NewGuid().ToString());
        Folder.MainRoom = new Folder("Main Room", null, null, Guid.NewGuid().ToString());
        Folder.Garage = new Folder("Garage", null, null, Guid.NewGuid().ToString());
        Folder.MainRoomGo = null;
        Folder.DocumentiIndex =  "8791e50f-033e-4497-a122-33fbee7c1bfb";
        Folder.ShouldDoorsHaveGrabberAttached = false;
    }

    private void Start()
    {
        Folder.MainRoomGo = MainRoomGo;
        var mainRoomPlayerDetector = transform.Find("MainRoom").AddComponent<PlayerDetector>();
        mainRoomPlayerDetector.SetFolderReferred(Folder.MainRoom);
        var garagePlayerDetector = transform.Find("MainRoom").Find("Garage").AddComponent<PlayerDetector>();
        garagePlayerDetector.SetFolderReferred(Folder.Garage);
        InstantiateScene(true);
        SpawnObjectsForQuest1();
    }
    

    private GameObject PickPrefabFromFile(RoomFile file)
    {
        var sizeIndex = file.GetSize() >= MediumSizeMin ? (file.GetSize() >= LargeSizeMin ? 2 : 1) : 0;
        return file.GetFormat() switch
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
    
    private void SpawnObjectsForQuest1()
    {
        var quest1ObjectsSpawner =
            transform.Find("MainRoom").Find("MainRoom_Outside").Find("Quest1_ObjSpawners");
        var objectSpawners = new List<GameObject>();
        for (var i = 0; i < 8; i++)
        {
            objectSpawners.Add(quest1ObjectsSpawner.GetChild(i).gameObject);
        }

        var random = new Random();
        var indiceNome = random.Next(0, ImageFileNames.Count);
        var nomeImmagine1 = ImageFileNames[indiceNome];
        ImageFileNames.Remove(nomeImmagine1);
        indiceNome = random.Next(0, ImageFileNames.Count);
        var nomeImmagine2 = ImageFileNames[indiceNome];
        ImageFileNames.Remove(nomeImmagine2);
        nomeImmagine1 += ".png";
        nomeImmagine2 += ".jpg";
        var immagine1 = new RoomFile(nomeImmagine1, "png", random.Next(0, 3), random.Next(1, 150), null, null, Guid.NewGuid().ToString());
        var immagine2 = new RoomFile(nomeImmagine2, "jpeg", random.Next(4, 8), random.Next(1, 150), null, null, Guid.NewGuid().ToString());
        indiceNome = random.Next(0, DocFileNames.Count);
        var nomeDocumento1 = DocFileNames[indiceNome];
        DocFileNames.Remove(nomeDocumento1);
        indiceNome = random.Next(0, DocFileNames.Count);
        var nomeDocumento2 = DocFileNames[indiceNome];
        DocFileNames.Remove(nomeDocumento2);
        indiceNome = random.Next(0, DocFileNames.Count);
        var nomeDocumento3 = DocFileNames[indiceNome];
        DocFileNames.Remove(nomeDocumento3);
        nomeDocumento1 += ".docx";
        nomeDocumento2 += ".pdf";
        nomeDocumento3 += ".txt";
        var documento1 = new RoomFile(nomeDocumento1, "doc", -1, random.Next(1, 150), null,  null, Guid.NewGuid().ToString());
        var documento2 = new RoomFile(nomeDocumento2, "pdf", -1, random.Next(1, 150), null, null, Guid.NewGuid().ToString());
        var documento3 = new RoomFile(nomeDocumento3, "txt", -1, random.Next(1, 150), null, null, Guid.NewGuid().ToString());
        indiceNome = random.Next(0, MultimediaFileNames.Count);
        var nomeMultFile1 = MultimediaFileNames[indiceNome];
        MultimediaFileNames.Remove(nomeMultFile1);
        indiceNome = random.Next(0, MultimediaFileNames.Count);
        var nomeMultFile2 = MultimediaFileNames[indiceNome];
        MultimediaFileNames.Remove(nomeMultFile2);
        nomeMultFile1 += ".mp3";
        nomeMultFile2 += ".mov";
        var multFile1 = new RoomFile(nomeMultFile1, "mp3", -1, random.Next(1, 150), null, null, Guid.NewGuid().ToString());
        var multFile2 = new RoomFile(nomeMultFile2, "mov", -1, random.Next(1, 150), null, null, Guid.NewGuid().ToString());
        var fileBonus = new RoomFile("", "", -1, 0f, null, null, Guid.NewGuid().ToString());
        string nomeFileBonus;
        var formatoBonusIndice = random.Next(0, 1);
        switch (formatoBonusIndice)
        {
            case 0:
            {
                indiceNome = random.Next(0, ImageFileNames.Count);
                nomeFileBonus = ImageFileNames[indiceNome];
                ImageFileNames.Remove(nomeFileBonus);
                nomeFileBonus += ".png";
                fileBonus = new RoomFile(nomeFileBonus, "png", 3, random.Next(1, 150), null, null, Guid.NewGuid().ToString());
                break;
            }
            case 1:
            {
                indiceNome = random.Next(0, ImageFileNames.Count);
                nomeFileBonus = ImageFileNames[indiceNome];
                ImageFileNames.Remove(nomeFileBonus);
                nomeFileBonus += ".jpg";
                fileBonus = new RoomFile(nomeFileBonus, "jpeg", 8, random.Next(1, 150), null, null, Guid.NewGuid().ToString());
                break;
            }
        }
        // Ora gli 8 file sono: { immagine1, immagine2, documento1, documento2, documento3, multFile1, multFile2, fileBonus }
        var extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        var objToSpawn = PickPrefabFromFile(immagine1);
        var instantiated = Instantiate(objToSpawn, extracted.transform);
        instantiated.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SpriteSelector.GetSpriteFromInt(immagine1.GetImageIndex());
        var fileGrabber = instantiated.transform.GetComponent<Grabber>();
        fileGrabber.SetReferred(immagine1);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(immagine2);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        instantiated.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SpriteSelector.GetSpriteFromInt(immagine2.GetImageIndex());
        fileGrabber = instantiated.transform.GetComponent<Grabber>();
        fileGrabber.SetReferred(immagine2);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(documento1);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<Grabber>();
        fileGrabber.SetReferred(documento1);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(documento2);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<Grabber>();
        fileGrabber.SetReferred(documento2);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(documento3);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<Grabber>();
        fileGrabber.SetReferred(documento3);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(multFile1);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<Grabber>();
        fileGrabber.SetReferred(multFile1);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(multFile2);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        fileGrabber = instantiated.transform.GetComponent<Grabber>();
        fileGrabber.SetReferred(multFile2);
        extracted = objectSpawners[random.Next(0, objectSpawners.Count)];
        objectSpawners.Remove(extracted);
        objToSpawn = PickPrefabFromFile(fileBonus);
        instantiated = Instantiate(objToSpawn, extracted.transform);
        instantiated.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SpriteSelector.GetSpriteFromInt(fileBonus.GetImageIndex());
        fileGrabber = instantiated.transform.GetComponent<Grabber>();
        fileGrabber.SetReferred(fileBonus);
    }

    private void Update()
    {
        if (!Folder.DirtyAfterInsertion) return;
        Folder.DirtyAfterInsertion = false;
        StartCoroutine(DelayInstantiation(Folder.Root.GetContainer()));
    }

    private IEnumerator DelayInstantiation(Object oldRoot)
    {
        if (Player.GetRoomIn() != Folder.MainRoom && Player.GetRoomIn() != Folder.Garage)
        {
            var offsetPosition = Player.OffsetInTheRoom();
            yield return new WaitForFixedUpdate();
            InstantiateScene(false);
            var folder = Folder.GetFolderFromAbsolutePath(Player.GetRoomIn().GetAbsolutePath().Split("/"), Folder.Root);
            var newRoomPosition = folder.GetContainer().transform.position;
            var p = Player.gameObject;
            p.transform.position = new Vector3(newRoomPosition.x + offsetPosition.x,
                p.transform.position.y, newRoomPosition.z + offsetPosition.z);
            Destroy(oldRoot);
        }
        else
        {
            yield return new WaitForFixedUpdate();
            InstantiateScene(false);
            Destroy(oldRoot);
        }
        Player.transform.GetComponent<FirstPersonCharacterController>().ReactivateInput();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void InstantiateScene(bool firstTime)
    {
        try
        {
            Folder.CurrentFileName = FileName;
            var desktop = Folder.GenerateFolderStructureFromFile(Folder.CurrentFileName);
            if (desktop == null)
            {
                throw new Exception("Could not generate folder structure");
            }
            Folder.InstantiateFolder(desktop, transform, Direction.North, RoomsPrefabs, Entrance.None, EntrancesPrefabs,
                RoomLenght, RoomHeight, (int)Mathf.Log(RoomLayer.value, 2));
            Folder.Root = desktop;
            TrashBinController.PopulateTrashBin();
            if (firstTime)
            {
                Folder.Root.ActivateRoomComponents(true);
            }
            else
            {
                var newPlayerFolder = Folder.GetFolderFromAbsolutePath(Player.GetRoomIn().GetAbsolutePath().Split("/"), Folder.Root);
                newPlayerFolder?.GetParent()?.GetParent()?.ActivateRoomComponents(true);
                newPlayerFolder?.GetParent()?.ActivateRoomComponents(true);
                newPlayerFolder?.ActivateRoomComponents(true);
                if (newPlayerFolder == null) Folder.Root.ActivateRoomComponents(true);
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
    

    private static void RestoreFileSystemToDefault()
    {
        var defaultFolderStructure = Folder.GenerateFolderStructureFromFile(DefaultFileSystem);
        Folder.WriteFolderStructureToFile(defaultFolderStructure);
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

public abstract class Grabbable
{
    public abstract Folder GetParent();

    public abstract void SetParent(Folder folder);

    public abstract string GetName();

    public abstract void SetName(string newName);

    public abstract void Delete();

    public abstract string GetAbsolutePath();

    public abstract void PermDelete();

    public abstract void SetParentOnDeletionAbsolutePath(string folder);

    public abstract string GetParentOnDeletionAbsolutePath();

    public abstract void Recover();

    public abstract Grabbable GetACopy();

    public abstract void Rename(string newName);

    public abstract bool IsACopyOf(Grabbable grabbable);

    public abstract bool IsACopy();

    public abstract string GetIndex();

    public abstract string GetCopyOf();
}

public class RoomFile : Grabbable
{
    private string _name;
    private readonly string _format;
    private readonly int _imageIndex;
    private readonly float _size;
    private Folder _parent;
    private string _parentOnDeletionAbsolutePath;
    public static RoomFile ScoperteFile;
    private string _index;
    private string _copyOf;

    public RoomFile(string name, string format, int imageIndex, float size, Folder parent, string copyOf, string index)
    {
        _name = name;
        _format = format;
        _imageIndex = imageIndex;
        _size = size;
        _parent = parent;
        _copyOf = copyOf;
        _index = index;
    }

    public override bool IsACopyOf(Grabbable grabbable)
    {
        if (_copyOf == null || grabbable == null) return false;
        return _copyOf == grabbable.GetIndex() || Folder.FindGrabbableWithIndexFromRoot(_copyOf).IsACopyOf(grabbable);
    }

    public override string GetCopyOf()
    {
        return _copyOf;
    }

    public override string GetName()
    {
        return _name;
    }

    public override void SetName(string newName)
    {
        _name = newName;
    }

    public string GetFormat()
    {
        return _format;
    }

    public int GetImageIndex()
    {
        return _imageIndex;
    }

    public float GetSize()
    {
        return _size;
    }

    public override Folder GetParent()
    {
        return _parent;
    }

    public override string GetAbsolutePath()
    {
        return _parent.GetAbsolutePath() + "/" + _name;
    }

    public override void Delete()
    {
        if (_parent != null)
            _parent.DeleteFile(this, false);
        else
        {
            Folder.TrashBin.GetFiles().Add(this);
            _parent = Folder.TrashBin;
            Folder.TriggerReloading(Operation.FileDeleted);
        }
    }

    public override void SetParent(Folder folder)
    {
        _parent = folder;
    }

    public override void PermDelete()
    {
        _parent.DeleteFile(this, true);
    }

    public override void SetParentOnDeletionAbsolutePath(string folder)
    {
        _parentOnDeletionAbsolutePath = folder;
    }

    public override string GetParentOnDeletionAbsolutePath()
    {
        return _parentOnDeletionAbsolutePath;
    }

    public override void Recover()
    {
        if (_parentOnDeletionAbsolutePath != null)
        {
            var backFolder = Folder.GetFolderFromAbsolutePath(_parentOnDeletionAbsolutePath.Split("/"), Folder.Root);
            if (backFolder != null)
            {
                backFolder.InsertFileOrFolder(this, true);
            }
            else
            {
                NotificationManager.Notify(Operation.RestoreRedirectedToDesktop);
                Debug.Log("Non si è riusciti a ripristinare correttamente il file, verrà inserito nel Desktop");
                Folder.Root.InsertFileOrFolder(this, true);
            }
        }
        else
        {
            NotificationManager.Notify(Operation.RestoreRedirectedToDesktop);
            Folder.Root.InsertFileOrFolder(this, true);
        }
    }

    public override bool IsACopy()
    {
        return _copyOf != null;
    }

    public override Grabbable GetACopy()
    {
        var copyName = _name.Split(".")[0] + "_copia." + _name.Split(".")[1];
        return new RoomFile(copyName, _format, this == ScoperteFile ? -1 : _imageIndex, _size, null, _index, Guid.NewGuid().ToString());
    }

    public override void Rename(string newName)
    {
        newName = newName + "." + GetFormatExtensionFromFormat(_format);
        if (_parent != null)
        {
            while (!_parent.IsFileNameAvailable(newName))
                newName = newName.Split(".")[0] + "_copia." + newName.Split(".")[1];
        }
        _name = newName;
        Folder.TriggerReloading(Operation.FileRenamed);
        NotificationManager.Notify(Operation.FileRenamed);
        //check quest 2
        if (HouseManager.ActualQuest == 2)
        {
            QuestManager.Quest2FormatChecker();
        }
    }

    public bool IsScoperte()
    {
        return _imageIndex == -2;
    }

    public bool IsZipOf(Grabbable grabbable)
    {
        return _format == "zip" && IsACopyOf(grabbable);
    }

    private static string GetFormatExtensionFromFormat(string format)
    {
        return format switch
        {
            "png" => "png",
            "jpeg" => "jpg",
            "doc" => "docx",
            "pdf" => "pdf",
            "txt" => "txt",
            "mp3" => "mp3",
            "mov" => "mov",
            "zip" => "zip",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Format error")
        };
    }

    // ReSharper disable once UnusedMember.Local
    private static string GetFormatFromFormatExtension(string formatExtension)
    {
        return formatExtension switch
        {
            "png" => "png",
            "jpg" => "jpeg",
            "docx" => "doc",
            "pdf" => "pdf",
            "txt" => "txt",
            "mp3" => "mp3",
            "mov" => "mov",
            "zip" => "zip",
            _ => throw new ArgumentOutOfRangeException(nameof(formatExtension), formatExtension,
                "Format extension error")
        };
    }

    public override string GetIndex()
    {
        return _index;
    }
}

public enum Operation
{
    Nop,
    FileOrFolderInserted,
    FileDeleted,
    FilePermDeleted,
    FileRenamed,
    FolderMoving,
    FolderMoved,
    FolderDeleted,
    FolderPermDeleted,
    FolderCreated,
    FolderRenamed,
    TrashBinEmpty,
    FileMoved,
    FileCopied,
    FolderCopied,
    ReleaseNotAllowed,
    FileRestored,
    FolderRestored,
    FolderFullOfSubfolders,
    FolderFullOfFiles,
    Quest1Completed,
    Quest2Completed,
    Quest3Completed,
    Quest4Completed,
    Quest5Completed,
    Quest6Completed,
    Quest7Completed,
    Quest8Completed,
    LockedFunctionality,
    RestoreRedirectedToDesktop,
    ReleaseIONotCopy,
    BringFolderToUseZipper,
    UnzipNotAllowed,
    ShouldBringScoperte,
    ShouldBringImmaginiEVideoFolder,
    ShouldBringImmaginiEVideoFolderToZip,
    CompleteFirstHalfOfZip,
    NoSpaceInTrashBin
}


public class Folder : Grabbable
{
    public static bool DirtyAfterInsertion;
    public static string CurrentFileName;
    public static Folder Root;
    public static Folder TrashBin;
    private GameObject _container;
    private Folder _father;
    private readonly List<Folder> _children;
    private List<RoomFile> _files;
    private string _name;
    public static Folder MainRoom;
    public static Folder Garage;
    public static Folder ImmaginiEVideoFolder;
    public static GameObject MainRoomGo;
    private BachecaFileController _bacheca;
    public static string DocumentiIndex;
    private string _parentOnDeletionAbsolutePath;
    public const int MaxNumberOfSubfolders = 9;
    public const int MaxNumberOfFilesPerFolder = 10;
    public static bool ShouldDoorsHaveGrabberAttached;
    private string _index;
    private string _copyOf;

    public override string GetCopyOf()
    {
        return _copyOf;
    }

    private List<Folder> GetAllChildren()
    {
        var list = new List<Folder>();
        foreach (var child in _children)
        {
            list.Add(child);
            list.AddRange(child.GetAllChildren());
        }
        return list;
    }

    public static Grabbable FindGrabbableWithIndexFromRoot(string index)
    {
        var allFiles = Root.GetAllFiles();
        var allFolders = Root.GetAllChildren();
        if (allFiles.FirstOrDefault(file => file.GetIndex() == index) is Grabbable possibility) return possibility;
        possibility = allFolders.FirstOrDefault(folder => folder.GetIndex() == index);
        return possibility;
    }

    public override bool IsACopy()
    {
        return _copyOf != null;
    }

    public override string GetIndex()
    {
        return _index;
    }

    public override bool IsACopyOf(Grabbable grabbable)
    {
        if (_copyOf == null || grabbable == null) return false;
        return _copyOf == grabbable.GetIndex() || FindGrabbableWithIndexFromRoot(_copyOf).IsACopyOf(grabbable);
    }

    public static void TriggerReloading(Operation lastOp)
    {
        if (lastOp == Operation.Quest3Completed)
        {
            CreateFolderManager.EnabledByQuest = true;
        }
        else if (lastOp == Operation.Quest5Completed)
        {
            ShouldDoorsHaveGrabberAttached = true;
        }
        WriteNewFolderStructureToFile();
        //_lastOperation = lastOp;
        DirtyAfterInsertion = true;
    }
    
    public override void Rename(string newName)
    {
        while (!_father.IsChildNameAvailable(newName))
        {
            newName += "_copia";
        }
        _name = newName;
        TriggerReloading(Operation.FolderRenamed);
        NotificationManager.Notify(Operation.FolderRenamed);
        
    }

    public override Grabbable GetACopy()
    {
        var f = new Folder(_name + "_copia", null, _index, Guid.NewGuid().ToString());
        foreach (var child in _children)
        {
            f._children.Add(child.GetACopy() as Folder);
        }
        foreach (var file in _files)
        {
            f._files.Add(file.GetACopy() as RoomFile);
        }
        f._parentOnDeletionAbsolutePath = null;
        return f;
    }

    public override void SetName(string newName)
    {
        _name = newName;
    }


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
    public Folder(string name, Folder father, string copyOf, string index)
    {
        _father = father;
        _name = name;
        _children = new List<Folder>();
        _files = new List<RoomFile>();
        _copyOf = copyOf;
        _index = index;
    }

    public void SetFiles(List<RoomFile> files)
    {
        _files = files;
    }

    public void DeleteFile(RoomFile file, bool perm)
    {
        if (!perm) file.SetParentOnDeletionAbsolutePath(file.GetParent().GetAbsolutePath());
        if (_files.Remove(file))
        {
            if (!perm)
            {
                TrashBin._files.Add(file);
                file.SetParent(TrashBin);
                TriggerReloading(Operation.FileDeleted);
                if (TrashBinController.EmptyOperation == false)
                {
                    NotificationManager.Notify(Operation.FileDeleted);
                }
            }
            else
            {
                TrashBin._files.Remove(TrashBin._files.FirstOrDefault(fileB => fileB.GetIndex() == file.GetIndex()));
                file.SetParent(null);
                TriggerReloading(Operation.FilePermDeleted);
                if (TrashBinController.EmptyOperation == false)
                {
                    NotificationManager.Notify(Operation.FilePermDeleted);
                }

                if (HouseManager.ActualQuest == 8)
                {
                    if (TrashBin.GetFiles().Count == 0)
                    {
                        HouseManager.ActualQuest = 9;
                        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonCharacterController>().MovementBackToNormal();
                        NotificationManager.Instance.StartCoroutine(
                            NotificationManager.QuestNotify("Lamp ti sta aspettando! :)"));
                    }
                }
            }
        }
    }

    public override void SetParentOnDeletionAbsolutePath(string folder)
    {
        _parentOnDeletionAbsolutePath = folder;
    }
    
    public override string GetParentOnDeletionAbsolutePath()
    {
        return _parentOnDeletionAbsolutePath;
    }
    public override void SetParent(Folder folder)
    {
        _father = folder;
    }

    private void DeleteFolder(Folder folder, bool perm)
    {
        if (!perm) folder.SetParentOnDeletionAbsolutePath(folder.GetParent().GetAbsolutePath());
        if (_children.Remove(folder))
        {
            if (!perm)
            {
                TrashBin._children.Add(folder);
                folder.SetParent(TrashBin);
                TriggerReloading(Operation.FolderDeleted);
                if (TrashBinController.EmptyOperation == false)
                {
                    NotificationManager.Notify(Operation.FolderDeleted);
                }
            }
            else
            {
                folder.SetParent(null);
                TriggerReloading(Operation.FolderPermDeleted);
                if (TrashBinController.EmptyOperation == false)
                {
                    NotificationManager.Notify(Operation.FolderPermDeleted);  
                }
            }
        }
    }

    public override void Delete()
    {
        _father.DeleteFolder(this, false);
    }

    public override void PermDelete()
    {
        _father.DeleteFolder(this, true);
    }

    private void SetBacheca(BachecaFileController bachecaFileController)
    {
        _bacheca = bachecaFileController;
    }


    public BachecaFileController GetBacheca()
    {
        return _bacheca;
    }

    public override void Recover()
    {
        var backFolder = GetFolderFromAbsolutePath(_parentOnDeletionAbsolutePath.Split("/"), Root);
        if (backFolder != null)
        {
            backFolder.InsertFileOrFolder(this, true);
        }
        else
        {
            NotificationManager.Notify(Operation.RestoreRedirectedToDesktop);
            Root.InsertFileOrFolder(this, true);
        }
    }

    public void InsertFileOrFolder(Grabbable file, bool isRecovering)
    {
        var comingFrom = file.GetParent();
        var alreadyPresentSameFile = false;
        switch (file)
        {
            case Folder folder:
                if (_children.Contains(folder))
                    alreadyPresentSameFile = true;
                break;
            case RoomFile roomFile:
                if (_files.Contains(roomFile))
                    alreadyPresentSameFile = true;
                break;
        }
        if (!alreadyPresentSameFile)
        {
            switch (file)
            {
                case Folder folder:
                    while(!IsChildNameAvailable(folder._name))
                        folder._name += "_copia";
                    _children.Add(folder);
                    if (isRecovering)
                    {
                        file.SetParentOnDeletionAbsolutePath(null);
                    }
                    comingFrom?.GetChildren().Remove(folder);
                    break;
                case RoomFile roomFile:
                    while(!IsFileNameAvailable(roomFile.GetName()))
                        roomFile.SetName(
                            roomFile.GetName().Split(".")[0] + "_copia." + roomFile.GetName().Split(".")[1]);
                    _files.Add(roomFile);
                    if (isRecovering)
                    {
                        file.SetParentOnDeletionAbsolutePath(null);
                    }
                    comingFrom?.GetFiles().Remove(roomFile);
                    break;
            }
            file.SetParent(this);
        }
        TriggerReloading(Operation.FileOrFolderInserted);
        if (HouseManager.ActualQuest == 4)
        {
            QuestManager.Quest4FormatChecker();
        }
    }

    /*
    public static void InsertNewFile(RoomFile newFile, Folder father)
    {
        father._files.Add(newFile);
        TriggerReloading(Operation.FileCreated);
    }
    */

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

    public void RemoveChild(Folder folder, Operation operation)
    {
        _children.Remove(folder);
        TriggerReloading(operation);
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

    public override string GetAbsolutePath()
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

    public override Folder GetParent()
    {
        return _father;
    }

    public override string GetName()
    {
        return _name;
    }

    public List<RoomFile> GetFiles()
    {
        return _files;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public static Folder GetFolderFromCollider(Folder folder, Collider collider)
    {
        return folder.GetContainer().GetComponent(typeof(BoxCollider)) == collider ?
            folder
            :
            folder._children.Select(child => GetFolderFromCollider(child, collider)).FirstOrDefault(folderAnalyzed => folderAnalyzed != null);
    }

    private bool IsChildNameAvailable(string name)
    {
        return _children.All(child => child._name != name.Trim());
    }

    public bool IsFileNameAvailable(string name)
    {
        return _files.All(file => file.GetName() != name.Trim());
    }

    public List<Folder> GetChildren()
    {
        return _children;
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
        SetFoldersForDoorControllers(roomPre, folder);
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

    private static void SetFoldersForDoorControllers(GameObject roomGo, Folder folder)
    {
        var doorControllers = roomGo.transform.GetComponentsInChildren<DoorController>();
        for (var i = 0; i < doorControllers.Length; i++)
        {
            doorControllers[i].SetRoom(folder);
            doorControllers[i].SetRoomTo(folder._children[i]);
        }

        var doorGrabbers = roomGo.transform.GetComponentsInChildren<Grabber>();
        for (var i = 0; i < doorGrabbers.Length; i++)
        {
            doorGrabbers[i].SetReferred(folder._children[i]);
        }
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

    public static void WriteFolderStructureToFile(Folder root)
    {
        var jsonRoot = ConvertFolderStructureToJsonFolderStructure(root);
        JsonFolder.SaveToFile(CurrentFileName, jsonRoot);
    }
    
    private static Direction ComputeAbsoluteDirection(Direction direction, Direction parentDirection)
    {
        return (Direction) (((int) parentDirection + (int) direction) % 360);
    }

    public static Folder GenerateFolderStructureFromFile(string fileName)
    {
        var jsonFolderStructure = JsonFolder.FromFileName(fileName);
        return ConvertJsonFolderStructureToFolderStructure(jsonFolderStructure, null);
    } 
    
    private static Folder ConvertJsonFolderStructureToFolderStructure(JsonFolder jsonFolder, Folder father)
    {
        var folder = new Folder(jsonFolder.Name, father, jsonFolder.CopyOf, jsonFolder.Index);
        foreach (var file in jsonFolder.Files)
        {
            var fileToAdd = new RoomFile(file.Name, file.Format, file.ImageIndex, file.Size, folder, file.CopyOf, file.Index);
            folder._files.Add(fileToAdd);
            if (fileToAdd.IsScoperte()) RoomFile.ScoperteFile = fileToAdd;
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
            Files = new List<JsonFile>(),
            Index = folder._index,
            CopyOf = folder._copyOf
        };
        // ciclo per aggiungere i file di ogni cartella nella struttura json
        foreach (var file in folder._files)
        {
            jsonFolder.Files.Add(new JsonFile
            {
                Name = file.GetName(),
                Format = file.GetFormat(),
                ImageIndex = file.GetImageIndex(),
                Size = file.GetSize(),
                Index =  file.GetIndex(),
                CopyOf = file.GetCopyOf()
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
    
    [JsonProperty("ImageIndex")]
    public int ImageIndex { get; set; }
    
    [JsonProperty("Size")]
    public float Size { get; set; }

    [JsonProperty("Index")]
    public string Index;
    
    [JsonProperty("CopyOf")]
    public string CopyOf;
    
    
    // ReSharper disable once UnusedMember.Local
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

    [JsonProperty("Index")]
    public string Index;
    
    [JsonProperty("CopyOf")]
    public string CopyOf;

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
