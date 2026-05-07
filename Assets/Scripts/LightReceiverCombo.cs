using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Place on the receiver cylinder. Called by LightEmitter when the beam terminates here.
// Wire OnActivated / OnDeactivated Unity Events to drive puzzle logic (e.g. open a door).
public class LightReceiverCombo : MonoBehaviour
{
    public ProgressManager progressManager;     

    private MaterialPropertyBlock propBlock;
    private bool isActivated = false;


    // The color tag and visual color of the beam currently hitting this receiver.
    // Both are empty/clear when the receiver is inactive.
    public string CurrentColorTag { get; private set; } = "";
    public Color CurrentBeamColor { get; private set; } = Color.clear;
    public List<LightReceiver> lightReceivers = new List<LightReceiver>();

    // Called by LightEmitter every frame. colorTag and beamColor are optional so
    // existing plain SetActivated(false) call sites remain valid without changes.
    public void Active(LightReceiver lightReceiver)
    {
        Debug.Log("Active. Light Receivers Count: " + lightReceivers.Count);
        if (!lightReceivers.Contains(lightReceiver))
        {
            Debug.Log("Doesn't contain: " + lightReceiver);
            lightReceivers.Add(lightReceiver);            
        }
        if (lightReceivers.Count == 2)
        {
            if (!isActivated)
            {
                Debug.Log("progressed " + lightReceivers.Count);
                Debug.Log(lightReceivers[0]);
                Debug.Log(lightReceivers[1]);
                progressManager.Progress();
                isActivated = true;
            }
        }
    }
    public void Unactive(LightReceiver lightReceiver)
    {
        lightReceivers.Remove(lightReceiver);
    }
}
