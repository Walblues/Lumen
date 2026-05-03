using UnityEngine;

// Place on a mirror object (tag it "Mirror"). Exposes RotateCW / RotateCCW for XRI Unity Events.
// Setup: add an XRSimpleInteractable and wire its "Select Entered" event to RotateCW or RotateCCW,
// exactly like VRCubeArrayController is wired to button press events.
//
// The mirror rotates around its local Y axis. Adjust rotationAxis in the Inspector if needed.
public class RotatableMirror : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Each button press snaps the mirror by this many degrees.")]
    public float angleStep = 45f;
    [Tooltip("How fast the mirror visually animates to the target angle.")]
    public float rotationSpeed = 8f;
    [Header("Rotation")]
    
    [Header("Sound")]
    public AudioClip rotateSound;
    public float soundVolume = 1f;


    [Tooltip("Uncheck to lock the mirror in place — RotateCW and RotateCCW will do nothing.")]
    public bool canRotate = true;

    private Quaternion initialLocalRotation;
    private float targetAngle = 0f;
    private float currentAngle = 0f;

    void Awake()
    {
        initialLocalRotation = transform.localRotation;
    }

    void Update()
    {
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
        transform.localRotation = initialLocalRotation * Quaternion.AngleAxis(currentAngle, Vector3.up);
    }


    // Wire to XRI "Select Entered" or a poke button's Unity Event.
    public void RotateCW()
    {
        if (!canRotate) return;
        SoundFXManager.Instance?.PlaySound(rotateSound, transform, soundVolume);
        targetAngle -= angleStep;
        NormalizeAngle();
    }

    public void RotateCCW()
    {
        if (!canRotate) return;
        SoundFXManager.Instance?.PlaySound(rotateSound, transform, soundVolume);
        targetAngle += angleStep;
        NormalizeAngle();
    }

    void NormalizeAngle()
    {
        // Keep in [0, 360) so LerpAngle doesn't spin the wrong way across 360/0
        targetAngle = ((targetAngle % 360f) + 360f) % 360f;
    }
}
