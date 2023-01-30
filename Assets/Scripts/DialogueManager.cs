using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Image actorImage;
    public TMP_Text actorNameText;
    public TMP_Text message;
    public RectTransform backgroundBox;
    private DialogueTrigger currentDialogueTrigger;
    private string[] currentMessages;
    private int activeMessage = 0;
    

    public void OpenDialogue(DialogueTrigger dialogueTrigger, string[] messages, string actorName, Sprite sprite)
    {
        currentMessages = messages;
        actorNameText.text = actorName;
        actorImage.sprite = sprite;
        currentDialogueTrigger = dialogueTrigger;
        activeMessage = 0;
        //Qui bisogna fare in modo che il player guardi Lamp
        DisplayMessage();
    }

    void DisplayMessage()
    {
        string messageToDisplay = currentMessages[activeMessage];
        message.text = messageToDisplay;
    }

    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("Conversazione terminata");
            currentDialogueTrigger.EndDialogue();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextMessage();
        }
    }
}
