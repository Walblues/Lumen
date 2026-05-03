
using UnityEngine;

// Attach to the pedestal cylinder. Assign the mirror child object in the Inspector.
//
// Prefab structure:
//   Pedestal  [Cylinder]  — this script + XRSimpleInteractable
//   └── Mirror [thin Box] — RotatableMirror, tagged "Mirror"
//
// Wire XRSimpleInteractable's "Select Entered" event to Toggle() for press-to-retract.
// Set isRetractable = false in the Inspector for a fixed pedestal that never retracts.
public class MirrorPedestal : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The mirror child object that slides up and down.")]
    public Transform mirror;

    [Header("Retraction")]
    [Tooltip("Uncheck to lock the mirror in the extended position permanently.")]
    public bool isRetractable = true;
    [Tooltip("How far (in local Y units) the mirror slides down when retracted.")]
    public float retractDistance = 1f;
    [Tooltip("Speed in local units per second.")]
    public float animationSpeed = 3f;


    [Header("Sound")]
    public AudioClip retractSound;
    public float soundVolume = 1f;
    


    [Header("Initial State")]
    [Tooltip("Start with the mirror already retracted.")]
    public bool startRetracted = false;

    private float extendedLocalY;
    private float retractedLocalY;
    private float targetLocalY;
    private bool isRetracted;
    private Collider mirrorCollider;

    void Awake()
    {
        if (mirror == null)
        {
            Debug.LogError("MirrorPedestal: Mirror reference is not assigned.", this);
            return;
        }

        mirrorCollider = mirror.GetComponent<Collider>();
        extendedLocalY  = mirror.localPosition.y;
        retractedLocalY = extendedLocalY - retractDistance;

        isRetracted  = startRetracted;
        targetLocalY = isRetracted ? retractedLocalY : extendedLocalY;

        // Snap to starting position without animation
        ApplyMirrorY(targetLocalY);
    }

    void Update()
    {
        if (mirror == null) return;

        float currentY = mirror.localPosition.y;
        float newY     = Mathf.MoveTowards(currentY, targetLocalY, animationSpeed * Time.deltaTime);

        if (!Mathf.Approximately(newY, currentY))
            ApplyMirrorY(newY);

        // Keep collider in sync — only active when fully extended so the beam
        // system stops reflecting off the mirror the moment it starts moving.
        if (mirrorCollider != null)
        {
            bool fullyExtended = Mathf.Approximately(mirror.localPosition.y, extendedLocalY);
            if (mirrorCollider.enabled != fullyExtended)
                mirrorCollider.enabled = fullyExtended;
        }
    }

    // ── Public methods — wire these to XRI Unity Events ──────────────────────

    // Wire to the pedestal's XRSimpleInteractable "Select Entered" event.
    public void Toggle()
    {
        if (!isRetractable) return;
        SoundFXManager.Instance?.PlaySound(retractSound, transform, soundVolume);
        SetRetracted(!isRetracted);
    }

    public void Retract()
    {
        if (!isRetractable) return;
        SetRetracted(true);
    }

    // Extend works even when isRetractable is false, so you can raise the mirror
    // programmatically (e.g. as a puzzle reward) on a normally-locked pedestal.
    public void Extend()
    {
        SetRetracted(false);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    void SetRetracted(bool retract)
    {
        isRetracted  = retract;
        targetLocalY = isRetracted ? retractedLocalY : extendedLocalY;

        // Disable the collider immediately on retract so beams stop
        // reflecting the instant the mirror begins to move.
        if (isRetracted && mirrorCollider != null)
            mirrorCollider.enabled = false;
    }

    void ApplyMirrorY(float y)
    {
        var pos = mirror.localPosition;
        pos.y = y;
        mirror.localPosition = pos;
    }
}
