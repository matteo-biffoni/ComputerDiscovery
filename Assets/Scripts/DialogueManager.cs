using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [FormerlySerializedAs("actorImage")] public Image ActorImage;
    [FormerlySerializedAs("actorNameText")] public TMP_Text ActorNameText;
    [FormerlySerializedAs("message")] public TMP_Text Message;
    public RectTransform backgroundBox;
    private Action _endDialogCallback;
    private string[] _currentMessages;
    private int _activeMessage;
    

    public void OpenDialogue(Action endDialogCallback, string[] messages, string actorName, Sprite sprite)
    {
        _currentMessages = messages;
        ActorNameText.text = actorName;
        ActorImage.sprite = sprite;
        _endDialogCallback = endDialogCallback;
        _activeMessage = 0;
        backgroundBox.LeanScale(Vector3.one, 0.5f);
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
            backgroundBox.LeanScale(Vector3.zero, 0.5f);
            _endDialogCallback();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
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
