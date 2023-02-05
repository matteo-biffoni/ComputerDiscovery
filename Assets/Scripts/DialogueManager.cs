using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [FormerlySerializedAs("actorImage")] public Image ActorImage;
    [FormerlySerializedAs("actorNameText")] public TMP_Text ActorNameText;
    [FormerlySerializedAs("message")] public TMP_Text Message;
    private Action _endDialogCallback;
    private string[] _currentMessages;
    private int _activeMessage;
    private bool _dialogRunning;
    

    public void OpenDialogue(Action endDialogCallback, string[] messages, string actorName, Sprite sprite)
    {
        _currentMessages = messages;
        ActorNameText.text = actorName;
        ActorImage.sprite = sprite;
        _endDialogCallback = endDialogCallback;
        _activeMessage = 0;
        StartCoroutine(ScaleAndStartDialogue());
    }

    private IEnumerator ScaleAndStartDialogue()
    {
        DisplayMessage();
        while (transform.localScale.x < 0.99f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 8f);
            yield return null;
        }
        _dialogRunning = true;
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
            StartCoroutine(ScaleBackAndCallback());
        }
    }

    private IEnumerator ScaleBackAndCallback()
    {
        while (transform.localScale.x > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 8f);
            yield return null;
        }
        _dialogRunning = false;
        _endDialogCallback();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_dialogRunning) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextMessage();
        }
    }
}
