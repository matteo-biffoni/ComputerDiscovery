using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip DropClip;
    public AudioClip GrabClip;
    public AudioClip OpenDoor;
    public AudioClip AD5LDoor;
    public AudioClip Notification;
    public AudioClip RobotTalking;
    public AudioClip DeleteNotification;
    public AudioClip ConfirmNotification;

    public static bool Pause;
    public static bool DialogFinished;

    private void Awake()
    {
        Instance = this;
    }

    public static IEnumerator Play(Transform source, AudioClip clip)
    {
        var audioSource = source.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialize = true;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 0f;
        audioSource.maxDistance = 5f;
        var volumeCurve = new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 0.1f)
        );
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeCurve);
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.Play();
        yield return new WaitUntil(() => audioSource == null || !audioSource.isPlaying);
        Destroy(audioSource);
    }

    public static IEnumerator PlayAD5L_OpenDoor(Transform source)
    {
        yield return Play(source, Instance.AD5LDoor);
        yield return new WaitForSeconds(0.5f);
        yield return Play(source, Instance.AD5LDoor);
    }

    public static IEnumerator PlayRobotTalking(Transform source)
    {
        var audioSource = source.AddComponent<AudioSource>();
        audioSource.clip = Instance.RobotTalking;
        audioSource.spatialize = true;
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 0f;
        audioSource.maxDistance = 5f;
        var volumeCurve = new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 0.1f)
        );
        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeCurve);
        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.Play();
        audioSource.Pause();
        var shouldPlay = true;
        while (shouldPlay)
        {
            audioSource.UnPause();
            yield return new WaitUntil(() => Pause || DialogFinished);
            if (Pause)
            {
                audioSource.Pause();
                yield return new WaitUntil(() => !Pause || DialogFinished);
                
                if (DialogFinished)
                {
                    DialogFinished = false;
                    shouldPlay = false;
                }
            }
            else
            {
                DialogFinished = false;
                shouldPlay = false;
            }
        }
        DialogFinished = false;
        Pause = false;
        audioSource.time = 0f;
        Destroy(audioSource);
    }
}