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
    
    
    [FormerlySerializedAs("quest1Messages")] [SerializeField]
    string[] Quest1Messages;
    
    [FormerlySerializedAs("quest2Messages")] [SerializeField]
    string[] Quest2Messages;
    
    [FormerlySerializedAs("quest3Messages")] [SerializeField]
    string[] Quest3Messages;
    
    public void StartDialogue(int questNumber)
    {
        string[] messages = null;
        Cursor.lockState = CursorLockMode.None;
        Player.IgnoreInput();
        switch (questNumber)
        {
            case 1:
                messages = Quest1Messages;
                break;
            case 2:
                messages = Quest2Messages;
                break;
            case 3:
                messages = Quest3Messages;
                break;
        }
        InteractCanvas.SetActive(false);
        DialogueCanvas.SetActive(true);
        DialogueManager.OpenDialogue(this, messages, ActorName, ActorSprite);
    }

    public void EndDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Player.ReactivateInput();
        DialogueCanvas.SetActive(false);
        InteractCanvas.SetActive(true);
        _checkInteraction = true; 
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
        transform.LookAt(Player.transform);
        if (!_checkInteraction) return;

        if (Input.GetKeyDown(KeyPressed))
        {
            var p = transform.position;
            var look = new Vector3(p.x, p.y + 0.5f, p.z);
            Player.transform.GetComponentInChildren<Camera>().transform.LookAt(look);
            _checkInteraction = false;
            StartDialogue(HouseManager.ActualQuest);
        }
        
    }
    
}
