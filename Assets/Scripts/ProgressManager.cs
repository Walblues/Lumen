using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public int counter = 0;
    public List<Light> lights = new List<Light>();
    public SoundFXManager soundManager;
    public AudioClip lightEnableSound;

    public void Progress()
    {
        Debug.Log("Counter incremented: " + counter);
        lights[counter].enabled = true;
        soundManager.PlaySound(lightEnableSound, lights[counter].transform);
        counter++;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
