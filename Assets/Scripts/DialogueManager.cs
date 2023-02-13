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
    private Coroutine _runningPauseTimer;
    public GameObject CursorCanvas;

    public void OpenDialogue(Action endDialogCallback, string[] messages, string actorName, Sprite sprite)
    {
        _currentMessages = messages;
        ActorNameText.text = actorName;
        ActorImage.sprite = sprite;
        _endDialogCallback = endDialogCallback;
        _activeMessage = 0;
        CursorCanvas.SetActive(false);
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
        PlaySoundForDuration(transform, 1.5f);
    }

    private IEnumerator StopAfterDuration(Transform actorTransform, float duration)
    {
        yield return new WaitForSeconds(duration);
        AudioManager.PauseRobotTalking(actorTransform);
        _runningPauseTimer = null;
    }

    private void PlaySoundForDuration(Transform actorTransform, float duration)
    {
        if (_runningPauseTimer != null)
        {
            StopCoroutine(_runningPauseTimer);
        }
        AudioManager.PlayRobotTalking(actorTransform);
        _runningPauseTimer = StartCoroutine(StopAfterDuration(actorTransform, duration));
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
        AudioManager.StopRobotTalking(transform);
        _endDialogCallback();
        CursorCanvas.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_dialogRunning) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextMessage();
        }
    }
}
