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
        targetAngle -= angleStep;
        NormalizeAngle();
    }

    public void RotateCCW()
    {
        targetAngle += angleStep;
        NormalizeAngle();
    }

    void NormalizeAngle()
    {
        // Keep in [0, 360) so LerpAngle doesn't spin the wrong way across 360/0
        targetAngle = ((targetAngle % 360f) + 360f) % 360f;
    }
}
