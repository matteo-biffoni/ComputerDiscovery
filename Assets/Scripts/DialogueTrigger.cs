using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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
    private bool _secondQuestInstantiation;
    private bool _thirdQuestInstantiation;
    
    
    [FormerlySerializedAs("quest1Messages")] [SerializeField]
    string[] Quest1Messages;
    
    [FormerlySerializedAs("quest2Messages")] [SerializeField]
    string[] Quest2Messages;
    
    [FormerlySerializedAs("quest3Messages")] [SerializeField]
    string[] Quest3Messages;
    
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
            _ => null
        };
        DialogueCanvas.SetActive(true);
        NotificationManager.HardCloseNotification();
        DialogueManager.OpenDialogue(EndDialogue, messages, ActorName, ActorSprite, transform);
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
    
    private IEnumerator SmoothReturnToPreviousOrientation()
    {
        var cameraT = Player.transform.GetComponentInChildren<Camera>().transform;
        while (Quaternion.Angle(Player.transform.rotation, _previousPlayerRotation) > 0.1f)
        {
            Player.transform.rotation =
                Quaternion.Slerp(Player.transform.rotation, _previousPlayerRotation, Time.deltaTime * 12f);
            cameraT.localRotation =
                Quaternion.Slerp(cameraT.localRotation, _previousCameraRotation, Time.deltaTime * 12f);
            yield return null;
        }
        Player.ReactivateInput();
        yield return null;
    }

    private void EndDialogue()
    {
        //Controllo sulla quest 2 per generazione nuovo albero
        if (HouseManager.ActualQuest == 2 && !_secondQuestInstantiation)
        {
            _secondQuestInstantiation = true;
            var Immagini = Folder.GetFolderFromAbsolutePath(new [] { "Desktop", "Immagini"}, Folder.Root);
            var Torino = new Folder("Torino", Immagini);
            var file1 = new RoomFile("wByLQTNYLN.png", "png", 9, 70, Torino);
            var file2 = new RoomFile("jesnotNaJF.png", "png", 10, 70, Torino);
            var file3 = new RoomFile("sUhVzbsXFg.jpg", "jpeg", 11, 70, Torino);
            var file4 = new RoomFile("rncyZCLgUV.jpg", "jpeg", 12, 70, Torino);
            var fileList = new List<RoomFile> { file1, file2, file3, file4 };
            Torino.SetFiles(fileList);
            Immagini.GetChildren().Add(Torino);
            Folder.TriggerReloading(Operation.Nop);
            QuestManager.ImageNamesAtQuest2Start = new List<string>();
            foreach (var roomFile in Immagini.GetAllFiles())
            {
                QuestManager.ImageNamesAtQuest2Start.Add(roomFile.GetName());
            }
        }

        if (HouseManager.ActualQuest == 3 && !_thirdQuestInstantiation)
        {
            _thirdQuestInstantiation = true;
            Folder.TriggerReloading(Operation.Quest2Completed);
        }
        DialogueCanvas.SetActive(false);
        InteractCanvas.SetActive(true);
        _checkInteraction = true;
        StartCoroutine(SmoothReturnToPreviousOrientation());
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
        if (!_checkInteraction) return;

        if (Input.GetKeyDown(KeyPressed))
        {
            _checkInteraction = false;
            StartCoroutine(StartDialogue(HouseManager.ActualQuest));
        }
        
    }
    
}
