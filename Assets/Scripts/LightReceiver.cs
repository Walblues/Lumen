using UnityEngine;
using UnityEngine.Events;

// Place on the receiver cylinder. Called by LightEmitter when the beam terminates here.
// Wire OnActivated / OnDeactivated Unity Events to drive puzzle logic (e.g. open a door).
public class LightReceiver : MonoBehaviour
{
    [Header("Visuals")]
    public Material inactiveMaterial;
    public Material activeMaterial;
    public ProgressManager progressManager;

    [Header("Activation")]
    [Tooltip("Seconds the beam must hold on the receiver before it activates.")]
    public float activationHoldTime = 0.2f;

    [Header("Events")]
    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;
    private bool isActivated = false;
    private bool hasTriggeredActivate = false;
    private Renderer rend;
    private MaterialPropertyBlock propBlock;
    public LightReceiverCombo lightReceiverCombo;
    public bool IsActivated => isActivated;

    private bool beamIsHitting = false;
    private float holdTimer = 0f;
    private string pendingColorTag = "";
    private Color pendingBeamColor = Color.clear;

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

    void Update()
    {
        if (beamIsHitting && !isActivated)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= activationHoldTime)
                CommitActivation(pendingColorTag, pendingBeamColor);
        }
    }

    // Called by LightEmitter every frame. colorTag and beamColor are optional so
    // existing plain SetActivated(false) call sites remain valid without changes.
    public void SetActivated(bool value, string colorTag = "", Color beamColor = default)
    {
        if (!value)
        {
            beamIsHitting = false;
            holdTimer = 0f;
            pendingColorTag = "";
            pendingBeamColor = Color.clear;

            if (!isActivated) return;

            isActivated = false;
            CurrentColorTag = "";
            CurrentBeamColor = Color.clear;

            if (rend != null)
            {
                rend.material = inactiveMaterial;
                rend.SetPropertyBlock(null);
            }

            OnDeactivated.Invoke();
            lightReceiverCombo?.Unactive(this);
            return;
        }

        // Beam is hitting — store pending color and start/continue the hold timer.
        beamIsHitting = true;
        pendingColorTag = colorTag;
        pendingBeamColor = beamColor;

        // If already active, handle a mid-flight color change immediately.
        if (isActivated)
        {
            if (CurrentColorTag == colorTag) return;
            CurrentColorTag = colorTag;
            CurrentBeamColor = beamColor;
            ApplyActiveVisuals(beamColor);
        }
        // Otherwise wait for Update() to count up the hold timer.
    }

    void CommitActivation(string colorTag, Color beamColor)
    {
        isActivated = true;
        CurrentColorTag = colorTag;
        CurrentBeamColor = beamColor;

        if (rend != null)
        {
            rend.material = activeMaterial;
            ApplyActiveVisuals(beamColor);
        }

        OnActivated.Invoke();
        if (!hasTriggeredActivate && progressManager != null)
        {
            progressManager.Progress();
            hasTriggeredActivate = true;
        }
        lightReceiverCombo?.Active(this);
    }

    void ApplyActiveVisuals(Color beamColor)
    {
        propBlock.SetColor("_BaseColor", beamColor);
        propBlock.SetColor("_Color", beamColor);
        propBlock.SetColor("_EmissionColor", beamColor);
        rend.SetPropertyBlock(propBlock);
    }
}
