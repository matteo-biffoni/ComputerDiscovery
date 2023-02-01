using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    public FirstPersonCharacterController player;
    public KeyCode keyPressed;
    public GameObject interactCanvas;
    public GameObject dialogueCanvas;
    public DialogueManager DialogueManager;
    public Sprite actorSprite;
    public string actorName;
    public HouseManager houseManager;
    private bool checkInteraction;
    
    
    [SerializeField]
    string[] quest1Messages;
    
    [SerializeField]
    string[] quest2Messages;
    
    [SerializeField]
    string[] quest3Messages;
    
    public void StartDialogue(int questNumber)
    {
        string[] messages = null;
        Cursor.lockState = CursorLockMode.None;
        player.IgnoreInput();
        switch (questNumber)
        {
            case 1:
                messages = quest1Messages;
                break;
            case 2:
                messages = quest2Messages;
                break;
            case 3:
                messages = quest3Messages;
                break;
        }
        interactCanvas.SetActive(false);
        dialogueCanvas.SetActive(true);
        DialogueManager.OpenDialogue(this, messages, actorName, actorSprite);
    }

    public void EndDialogue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player.ReactivateInput();
        interactCanvas.SetActive(true);
        checkInteraction = true; 
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player") == true)
        {
            checkInteraction = true;
            interactCanvas.SetActive(true);
        } 
    }
    
    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player") == true)
        {
            checkInteraction = false;
            interactCanvas.SetActive(false);
        } 
    }


    private void Update()
    {
        transform.LookAt(player.transform);
        if (!checkInteraction) return;

        if (Input.GetKeyDown(keyPressed))
        {
            var look = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            player.transform.GetComponentInChildren<Camera>().transform.LookAt(look);
            checkInteraction = false;
            StartDialogue(houseManager.actualQuest);
        }
        
    }
    
}
