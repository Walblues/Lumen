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
    private MaterialPropertyBlock propBlock;

    public bool IsActivated => isActivated;

    // The color tag and visual color of the beam currently hitting this receiver.
    // Both are empty/clear when the receiver is inactive.
    public string CurrentColorTag { get; private set; } = "";
    public Color CurrentBeamColor { get; private set; } = Color.clear;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        if (rend != null && inactiveMaterial != null)
            rend.material = inactiveMaterial;
    }

    // Called by LightEmitter every frame. colorTag and beamColor are optional so
    // existing plain SetActivated(false) call sites remain valid without changes.
    public void SetActivated(bool value, string colorTag = "", Color beamColor = default)
    {
        bool colorChanged = value && (CurrentColorTag != colorTag);
        if (isActivated == value && !colorChanged) return;

        isActivated = value;
        CurrentColorTag = value ? colorTag : "";
        CurrentBeamColor = value ? beamColor : Color.clear;

        if (rend != null)
        {
            rend.material = isActivated ? activeMaterial : inactiveMaterial;

            if (isActivated)
            {
                // Tint the active material to match the beam's color.
                propBlock.SetColor("_BaseColor", beamColor);
                propBlock.SetColor("_Color", beamColor);
                propBlock.SetColor("_EmissionColor", beamColor);
                rend.SetPropertyBlock(propBlock);
            }
            else
            {
                // Clear the override so the inactive material renders unmodified.
                rend.SetPropertyBlock(null);
            }
        }

        if (isActivated)
            OnActivated.Invoke();
        else
            OnDeactivated.Invoke();
    }
}
