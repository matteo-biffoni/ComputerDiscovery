using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanClipManager : MonoBehaviour
{
    public AudioClip LowFan;

    public AudioClip MidFan;

    public AudioClip HighFan;

    private AudioSource _source;

    private int _value;

    private void Start()
    {
        _source = transform.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (HouseManager.ActualQuest == 7 && _value != 1)
        {
            _source.clip = HighFan;
            _source.Play();
            _value = 1;
        }
        else if (HouseManager.ActualQuest == 8 && _value != 2)
        {
            _source.clip = MidFan;
            _source.Play();
            _value = 2;
        }
        else if (HouseManager.ActualQuest != 7 && HouseManager.ActualQuest != 8 && _value != 0)
        {
            _source.clip = LowFan;
            _source.Play();
            _value = 0;
        }
        
    }
}
