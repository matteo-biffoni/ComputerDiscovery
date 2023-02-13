using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{

    [FormerlySerializedAs("player")] public FirstPersonCharacterController Player;
    [FormerlySerializedAs("keyPressed")] public KeyCode KeyPressed;
    [FormerlySerializedAs("interactCanvas")] public GameObject InteractCanvas;
    [FormerlySerializedAs("dialogueCanvas")] public GameObject DialogueCanvas;
    public DialogueManager DialogueManager;
    [FormerlySerializedAs("actorSprite")] public Sprite ActorSprite;
    [FormerlySerializedAs("actorName")] public string ActorName;
    [FormerlySerializedAs("houseManager")] public HouseManager HouseManager;
    private bool _checkInteraction;
    private Quaternion _previousPlayerRotation;
    private Quaternion _previousCameraRotation;
    public Transform LookAtLamp;
    private bool _firstQuestInstantiation;
    private bool _secondQuestInstantiation;
    private bool _thirdQuestInstantiation;
    private bool _fourthQuestInstantiation;
    public static bool FifthQuestInstantiation;
    public static bool SixthQuestInstantiation;
    private bool _seventhQuestInstantiation;
    private bool _eighthQuestInstantiation;
    private bool _showFinalOptions;

    public GameObject FinalOptionsPrefab;
    private GameObject _instantiatedFinalOptions;


    [FormerlySerializedAs("quest1Messages")] [SerializeField]
    string[] Quest1Messages;
    
    [FormerlySerializedAs("quest2Messages")] [SerializeField]
    string[] Quest2Messages;
    
    [FormerlySerializedAs("quest3Messages")] [SerializeField]
    string[] Quest3Messages;

    [SerializeField]
    string[] Quest4Messages;

    [SerializeField]
    string[] Quest5Messages;

    [SerializeField] 
    private string[] Quest6Messages;
    
    [SerializeField] 
    private string[] Quest7Messages;

    [SerializeField]
    private string[] Quest8Messages;

    [SerializeField]
    private string[] FinalDialog;

    private string _toReplace = "Scoperte.docx";

    private void Awake()
    {
        FifthQuestInstantiation = false;
        SixthQuestInstantiation = false;
    }

    private IEnumerator StartDialogue(int questNumber)
    {
        InteractCanvas.SetActive(false);
        Player.IgnoreInput();
        yield return SmoothTurnToLamp();
        var messages = questNumber switch
        {
            1 => Quest1Messages,
            2 => Quest2Messages,
            3 => Quest3Messages,
            4 => Quest4Messages,
            5 => Quest5Messages,
            6 => Quest6Messages,
            7 => Quest7Messages,
            8 => Quest8Messages,
            9 => FinalDialog,
            _ => null
        };
        if (questNumber == 5)
        {
            Quest5Messages[3] = Quest5Messages[3].Replace(_toReplace, RoomFile.ScoperteFile.GetName());
            _toReplace = RoomFile.ScoperteFile.GetName();
        }
        DialogueCanvas.SetActive(true);
        NotificationManager.HardCloseNotification();
        AudioManager.Instance.StopIfLooping = true;
        DialogueManager.OpenDialogue(EndDialogue, messages, ActorName, ActorSprite);
    }
    
    private IEnumerator SmoothTurnToLamp()
    {
        _previousPlayerRotation = Player.transform.rotation;
        Transform transform1;
        _previousCameraRotation = (transform1 = Player.transform).GetComponentInChildren<Camera>().transform.localRotation;
        var direction = (LookAtLamp.position - transform1.position).normalized;
        var cameraT = Player.transform.GetComponentInChildren<Camera>().transform;
        var lookRotationCamera = Quaternion.LookRotation(direction);
        direction.y = 0;
        var lookRotation = Quaternion.LookRotation(direction);
        var localRotation = cameraT.localRotation;
        var cameraTo = Quaternion.Euler(new Vector3(lookRotationCamera.eulerAngles.x, localRotation.eulerAngles.y,
            localRotation.eulerAngles.z));
        while (Quaternion.Angle(Player.transform.rotation, lookRotation) > 0.1f)
        {
            Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, lookRotation, Time.deltaTime * 8f);
            cameraT.localRotation = Quaternion.Slerp(cameraT.localRotation, cameraTo, Time.deltaTime * 8f);
            yield return null;
        }
    }
    
    private IEnumerator SmoothReturnToPreviousOrientation(Operation operation)
    {
        var cameraT = Player.transform.GetComponentInChildren<Camera>().transform;
        while (Quaternion.Angle(Player.transform.rotation, _previousPlayerRotation) > 0.1f)
        {
            Player.transform.rotation =
                Quaternion.Slerp(Player.transform.rotation, _previousPlayerRotation, Time.deltaTime * 8f);
            cameraT.localRotation =
                Quaternion.Slerp(cameraT.localRotation, _previousCameraRotation, Time.deltaTime * 8f);
            yield return null;
        }
        Folder.TriggerReloading(operation);
        _checkInteraction = true;
        if (_showFinalOptions)
        {
            ShowFinalOptions();
        }
    }

    private void ShowFinalOptions()
    {
        _checkInteraction = false;
        Cursor.lockState = CursorLockMode.None;
        _instantiatedFinalOptions = Instantiate(FinalOptionsPrefab);
        var continueToPlay = _instantiatedFinalOptions.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        continueToPlay.onClick.AddListener(BackToPlay);
        var backToMenu = _instantiatedFinalOptions.transform.GetChild(0).GetChild(3).GetComponent<Button>();
        backToMenu.onClick.AddListener(BackToMainMenu);
    }

    private static void BackToMainMenu()
    {
        HouseManager.BackToMainMenu();
    }

    private void BackToPlay()
    {
        Destroy(_instantiatedFinalOptions);
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
        _showFinalOptions = false;
        _checkInteraction = true;
    }

    private void EndDialogue()
    {
        var oper = Operation.Nop;
        if (HouseManager.ActualQuest == 1 && !_firstQuestInstantiation)
        {
            _firstQuestInstantiation = true; 
            List<string> messages = new List<string>(new []
            {
                "1. Mira un oggetto e fai click con pulsante sinistro per afferrarlo",  
                "2. Spostati verso la cartella in cui vuoi posizionare il file",
                "3. Fai click con pulsante sinistro per rilasciarlo"
            });
           LavagnettaManager.SpecialWriteOnLavagnetta("GUIDA", "", messages);
        }
        //Controllo sulla quest 2 per generazione nuovo albero
        if (HouseManager.ActualQuest == 2 && !_secondQuestInstantiation)
        {
            _secondQuestInstantiation = true;
            List<string> messages = new List<string>(new []
            {
                "1. Mira un file e fai click con pulsante destro per aprire il pannello 'Operazioni'",  
                "2. Clicca su 'Rinomina'",
                "3. Inserisci un nuovo nome a tuo piacimento che rappresenti il file e fai click su 'Conferma'"
            });
            LavagnettaManager.SpecialWriteOnLavagnetta("GUIDA", "Per rinominare un file immagine:", messages);
            
            var Immagini = Folder.GetFolderFromAbsolutePath(new [] { "Desktop", "Immagini"}, Folder.Root);
            var Torino = new Folder("Torino", Immagini, null, Guid.NewGuid().ToString());
            var file1 = new RoomFile("wByLQTNYLN.png", "png", 9, 70, Torino, null, Guid.NewGuid().ToString());
            var file2 = new RoomFile("jesnotNaJF.png", "png", 10, 70, Torino, null, Guid.NewGuid().ToString());
            var file3 = new RoomFile("sUhVzbsXFg.jpg", "jpeg", 11, 70, Torino, null, Guid.NewGuid().ToString());
            var file4 = new RoomFile("rncyZCLgUV.jpg", "jpeg", 12, 70, Torino, null, Guid.NewGuid().ToString());
            var fileList = new List<RoomFile> { file1, file2, file3, file4 };
            Torino.SetFiles(fileList);
            Immagini.GetChildren().Add(Torino);
            QuestManager.ImageNamesAtQuest2Start = new List<string>();
            foreach (var roomFile in Immagini.GetAllFiles())
            {
                QuestManager.ImageNamesAtQuest2Start.Add(roomFile.GetName());
            }
            oper = Operation.Quest1Completed;
        }

        if (HouseManager.ActualQuest == 3 && !_thirdQuestInstantiation)
        {
            _thirdQuestInstantiation = true;
            List<string> messages = new List<string>(new string[]
            {
                "1. Recati in Garage al piano di sopra",  
                "2. Fai partire il download"
            });
            LavagnettaManager.SpecialWriteOnLavagnetta("GUIDA", "Per scaricare dalla macchina USB:", messages);
            oper = Operation.Quest2Completed;
            GameObject.FindGameObjectWithTag("Serranda").transform.GetComponent<BoxCollider>().enabled = true;
        }

        if (HouseManager.ActualQuest == 4 && !_fourthQuestInstantiation)
        {
            _fourthQuestInstantiation = true;
            List<string> messages = new List<string>(new []
            {
                "1. Posizionati nel luogo in cui vuoi creare una nuova cartella",  
                "2. Premi 'n', poi inserisci il nome della cartella e conferma",
                "Se proprio non te lo ricordavi...la cartella va creata nel Desktop e deve essere chiamata 'Viaggi'"
                
            });
            LavagnettaManager.SpecialWriteOnLavagnetta("GUIDA", "Crea una nuova cartella:", messages);
            oper = Operation.Quest3Completed;
        }

        if (HouseManager.ActualQuest == 5 && !FifthQuestInstantiation)
        {
            FifthQuestInstantiation = true;
            List<string> messages = new List<string>(new []
            {
                "1. Cerca il file nella cartella 'Viaggi'",  
                "2. Mira il file e fai click con il tasto destro per accedere alle operazioni",
                "3. Copia il file e consegnalo ad ADSL"
                
            });
            LavagnettaManager.SpecialWriteOnLavagnetta("GUIDA", $"Invia una copia del file '{RoomFile.ScoperteFile.GetName()}' in rete:", messages);
            oper = Operation.Quest4Completed;
        }

        if (HouseManager.ActualQuest == 6 && !SixthQuestInstantiation)
        {
            SixthQuestInstantiation = true;
            List<string> messages = new List<string>(new []
            {
                "1. Posizionati dentro la cartella 'Viaggi'",  
                "2. Crea una nuova sottocartella chiamata 'Immagini e video'",
                "3. Sposta dentro la cartella tutti i file di formato immagine e video"
                
            });
            LavagnettaManager.SpecialWriteOnLavagnetta("GUIDA", "Crea una nuova sotto cartella 'Immagini e video':", messages);
            oper = Operation.Quest5Completed;
        }
        if (HouseManager.ActualQuest == 7 && !_seventhQuestInstantiation)
        {
            _seventhQuestInstantiation = true;
            List<string> messages = new List<string>(new []
            {
                "Per eliminare un file...",  
                "1. Mira il file zip che vuoi eliminare ",
                "2. Fai click con il tasto destro per accedere al menu operazioni ed elimina"
                
            });
            LavagnettaManager.SpecialWriteOnLavagnetta("GUIDA", "Cerca ed elimina i file pesanti di tipo Zip nella cartella 'Documenti'", messages);
            oper = Operation.Quest6Completed;
        }
        
        if (HouseManager.ActualQuest == 8 && !_eighthQuestInstantiation)
        {
            _eighthQuestInstantiation = true;
            LavagnettaManager.SpecialWriteOnLavagnetta("GUIDA", "Elimina definitivamente i file zip all'interno del Cestino per ripristinare il corretto funzionamento magnetico della casa", null);
            oper = Operation.Quest7Completed;
        }

        if (HouseManager.ActualQuest == 9)
        {
            LavagnettaManager.SpecialWriteOnLavagnetta("COMPLIMENTI", "Hai completato tutte le missioni, goditi la tua casa computer!", null);
            oper = Operation.Quest8Completed;
            _showFinalOptions = true;
        }
        DialogueCanvas.SetActive(false);
        InteractCanvas.SetActive(true);
        StartCoroutine(SmoothReturnToPreviousOrientation(oper));
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            _checkInteraction = true;
            InteractCanvas.SetActive(true);
        } 
    }
    
    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            _checkInteraction = false;
            InteractCanvas.SetActive(false);
        } 
    }


    private void Update()
    {

        var lookAtPlayer = Player.transform.position;
        lookAtPlayer.y = transform.position.y;
        transform.LookAt(lookAtPlayer);
        if (_checkInteraction && Input.GetKeyDown(KeyPressed))
        {
            _checkInteraction = false;
            StartCoroutine(StartDialogue(HouseManager.ActualQuest));
        }
        else if (_showFinalOptions)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                BackToPlay();
            else if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    BackToPlay();
                }
            }
        }

    }
    
}
