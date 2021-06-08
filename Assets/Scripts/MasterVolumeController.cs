using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeController : MonoBehaviour
{
    private float masterVolume = 1f;

    public Slider volumeSlider;
    
    void Start()
    {
        masterVolume = PlayerPrefs.GetFloat("volume");
        AudioListener.volume = masterVolume;
        volumeSlider.value = masterVolume;
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = masterVolume;
        PlayerPrefs.SetFloat("volume", masterVolume);
    }
    
    public void UpdateVolume(float volume)
    {
        masterVolume = volume;
    }
}
