using UnityEngine;
using System.Collections.Generic;

// Place on the emitter cylinder. Assign a BeamSegment prefab.
// Beams reflect off objects tagged "Mirror" and activate any LightReceiver they terminate on.
public class LightEmitter : MonoBehaviour
{
    [Header("Beam Settings")]
    public GameObject beamPrefab;
    public int maxReflections = 10;
    public float maxDistance = 50f;
    [Tooltip("Small offset so the raycast doesn't hit the emitter's own collider.")]
    public float startOffset = 0.15f;
    [Tooltip("Local-space axis the beam shoots from. " +
             "A default Unity Cylinder has its height along Y (flat faces at +Y/-Y), " +
             "so keep this as (0,1,0) and aim the top of the cylinder at your first mirror.")]
    public Vector3 emissionAxis = Vector3.up;

    private List<GameObject> activeBeams = new List<GameObject>();
    private int currentBeamIndex;

    // Swap buffers to track receiver activation changes without per-frame allocation
    private HashSet<LightReceiver> hitThisFrame = new HashSet<LightReceiver>();
    private HashSet<LightReceiver> hitLastFrame = new HashSet<LightReceiver>();

    // LateUpdate ensures XRI has already applied any tracked/grabbed transform changes
    // before we read the emitter's position and rotation this frame.
    void LateUpdate()
    {
        currentBeamIndex = 0;
        hitThisFrame.Clear();

        ShootLaser();

        // Deactivate receivers the beam no longer reaches
        foreach (var receiver in hitLastFrame)
            if (!hitThisFrame.Contains(receiver))
                receiver.SetActivated(false);

        // Activate newly hit receivers
        foreach (var receiver in hitThisFrame)
            if (!hitLastFrame.Contains(receiver))
                receiver.SetActivated(true);

        // Swap buffers
        var temp = hitLastFrame;
        hitLastFrame = hitThisFrame;
        hitThisFrame = temp;

        // Hide unused beam segments
        for (int i = currentBeamIndex; i < activeBeams.Count; i++)
            activeBeams[i].SetActive(false);
    }

    void ShootLaser()
    {
        // Convert the local emission axis to world space so rotation is always respected.
        Vector3 direction = transform.TransformDirection(emissionAxis).normalized;
        Vector3 position = transform.position + direction * startOffset;

        for (int i = 0; i <= maxReflections; i++)
        {
            if (!Physics.Raycast(position, direction, out RaycastHit hit, maxDistance))
            {
                CreateBeamSegment(position, direction, maxDistance);
                break;
            }

            CreateBeamSegment(position, direction, hit.distance);

            if (hit.collider.CompareTag("Mirror"))
            {
                direction = Vector3.Reflect(direction, hit.normal);
                position = hit.point + direction * 0.01f; // prevent self-hit
            }
            else
            {
                var receiver = hit.collider.GetComponent<LightReceiver>();
                if (receiver != null)
                    hitThisFrame.Add(receiver);
                break;
            }
        }
    }

    void CreateBeamSegment(Vector3 start, Vector3 direction, float length)
    {
        GameObject beam;

        if (currentBeamIndex < activeBeams.Count)
        {
            beam = activeBeams[currentBeamIndex];
            beam.SetActive(true);
        }
        else
        {
            beam = Instantiate(beamPrefab);
            // Layer 2 = "Ignore Raycast" — prevents beam segment colliders from
            // blocking their own raycasts and causing the receiver to flicker.
            beam.layer = 2;
            activeBeams.Add(beam);
        }

        currentBeamIndex++;

        // Position at midpoint; scale Y to match length (capsule mesh elongated along Y)
        beam.transform.position = start + direction * (length / 2f);
        beam.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);
        beam.transform.localScale = new Vector3(0.05f, length / 2f, 0.05f);
    }
}
