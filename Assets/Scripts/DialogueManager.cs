using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [FormerlySerializedAs("actorImage")] public Image ActorImage;
    [FormerlySerializedAs("actorNameText")] public TMP_Text ActorNameText;
    [FormerlySerializedAs("message")] public TMP_Text Message;
    //public RectTransform backgroundBox;
    private DialogueTrigger _currentDialogueTrigger;
    private string[] _currentMessages;
    private int _activeMessage;
    

    public void OpenDialogue(DialogueTrigger dialogueTrigger, string[] messages, string actorName, Sprite sprite)
    {
        _currentMessages = messages;
        ActorNameText.text = actorName;
        ActorImage.sprite = sprite;
        _currentDialogueTrigger = dialogueTrigger;
        _activeMessage = 0;
        //Qui bisogna fare in modo che il player guardi Lamp
        DisplayMessage();
    }

    private void DisplayMessage()
    {
        var messageToDisplay = _currentMessages[_activeMessage];
        Message.text = messageToDisplay;
    }

    private void NextMessage()
    {
        _activeMessage++;
        if (_activeMessage < _currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("Conversazione terminata");
            _currentDialogueTrigger.EndDialogue();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextMessage();
        }
    }
}
