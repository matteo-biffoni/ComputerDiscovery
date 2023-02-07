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

    private void Awake()
    {
        Instance = this;
    }

    public static void Play(Transform source, AudioClip clip)
    {
        if (clip == Instance.AD5LDoor)
        {
            Instance.StartCoroutine(PlayAD5L_OpenDoor(source));
        }
        else
        {
            Instance.StartCoroutine(PlayCoroutine(source, clip));
        }
    }

    private static IEnumerator PlayCoroutine(Transform source, AudioClip clip)
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

    private static IEnumerator PlayAD5L_OpenDoor(Transform source)
    {
        yield return PlayCoroutine(source, Instance.AD5LDoor);
        yield return new WaitForSeconds(0.5f);
        yield return PlayCoroutine(source, Instance.AD5LDoor);
    }

    public static void StopRobotTalking(Transform source)
    {
        var audioSource = source.GetComponent<AudioSource>();
        if (audioSource == null) return;
        audioSource.Stop();
        Destroy(audioSource);
    }

    public static void PauseRobotTalking(Transform source)
    {
        var audioSource = source.GetComponent<AudioSource>();
        if (audioSource == null) return;
        audioSource.Pause();
    }

    public static void PlayRobotTalking(Transform source)
    {
        var audioSource = source.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
        else
        {
            audioSource = source.AddComponent<AudioSource>();
            audioSource.clip = Instance.RobotTalking;
            audioSource.loop = true;
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
        }
    }
}