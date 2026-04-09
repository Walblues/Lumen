using UnityEngine;
using UnityEngine.Events;

// Place on the receiver cylinder. Called by LightEmitter when the beam terminates here.
// Wire OnActivated / OnDeactivated Unity Events to drive puzzle logic (e.g. open a door).
public class LightReceiver : MonoBehaviour
{
    [Header("Visuals")]
    public Material inactiveMaterial;
    public Material activeMaterial;

    [Header("Events")]
    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;

    private bool isActivated = false;
    private Renderer rend;

    public bool IsActivated => isActivated;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && inactiveMaterial != null)
            rend.material = inactiveMaterial;
    }

    // Called by LightEmitter — do not call from other scripts.
    public void SetActivated(bool value)
    {
        if (isActivated == value) return;
        isActivated = value;

        if (rend != null)
            rend.material = isActivated ? activeMaterial : inactiveMaterial;

        if (isActivated)
            OnActivated.Invoke();
        else
            OnDeactivated.Invoke();
    }
}
