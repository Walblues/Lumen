using UnityEngine;
using System.Collections.Generic;

// Place on the emitter cylinder. Assign a BeamSegment prefab.
// Beams reflect off objects tagged "Mirror", pass through matching ColorFilters,
// and activate any LightReceiver they terminate on.
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

    [Header("Color")]
    [Tooltip("Logical color identifier (e.g. 'red', 'blue', 'orange'). " +
             "Must match the Allowed Color Tag on any ColorFilter this beam should pass through.")]
    public string colorTag = "white";
    [Tooltip("Visual tint applied to all beam segments.")]
    public Color beamColor = Color.white;

    [Header("State")]
    [Tooltip("Set to false to silence this emitter (e.g. via LightCombiner). " +
             "All downstream receivers are automatically deactivated.")]
    public bool isActive = true;

    private List<GameObject> activeBeams = new List<GameObject>();
    private int currentBeamIndex;
    private MaterialPropertyBlock propBlock;

    // Swap buffers to track receiver activation changes without per-frame allocation
    private HashSet<LightReceiver> hitThisFrame = new HashSet<LightReceiver>();
    private HashSet<LightReceiver> hitLastFrame = new HashSet<LightReceiver>();

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    // LateUpdate ensures XRI has already applied any tracked/grabbed transform changes
    // before we read the emitter's position and rotation this frame.
    void LateUpdate()
    {
        currentBeamIndex = 0;
        hitThisFrame.Clear();

        // Only shoot when active. When inactive, hitThisFrame stays empty so the
        // swap-buffer below automatically deactivates all previously-hit receivers.
        if (isActive)
            ShootLaser();

        // Deactivate receivers the beam no longer reaches
        foreach (var receiver in hitLastFrame)
            if (!hitThisFrame.Contains(receiver))
                receiver.SetActivated(false);

        // Update all currently-hit receivers with this frame's color (handles both
        // first activation and mid-flight color changes from LightCombiner).
        foreach (var receiver in hitThisFrame)
            receiver.SetActivated(true, colorTag, beamColor);

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
        Vector3 direction = transform.TransformDirection(emissionAxis).normalized;
        Vector3 position = transform.position + direction * startOffset;

        // Glass pass-throughs don't count as reflections, so track them separately.
        // The safety cap on total iterations prevents any edge-case infinite loops.
        int reflections = 0;
        int iterations = 0;
        int maxIterations = (maxReflections + 1) * 2;

        while (reflections <= maxReflections && iterations < maxIterations)
        {
            iterations++;

            if (!Physics.Raycast(position, direction, out RaycastHit hit, maxDistance))
            {
                CreateBeamSegment(position, direction, maxDistance);
                break;
            }

            CreateBeamSegment(position, direction, hit.distance);

            if (hit.collider.CompareTag("Mirror"))
            {
                Transform mirror = hit.collider.transform;
                Vector3 normal = hit.normal;

                if (Vector3.Dot(direction, normal) > 0)
                    normal = -normal;


                Vector3 planePoint = hit.collider.bounds.center;

                float denom = Vector3.Dot(direction, normal);

                if (Mathf.Abs(denom) > 0.0001f)
                {
                    float t = Vector3.Dot(planePoint - position, normal) / denom;
                    Vector3 intersectionPoint = position + direction * t;

                    // Reflect from that corrected intersection
                    direction = Vector3.Reflect(direction, normal);

                    position = intersectionPoint + direction * 0.01f;
                }

                reflections++;
            }
            else if (hit.collider.CompareTag("ColorFilter"))
            {
                var glass = hit.collider.GetComponent<ColorFilter>();
                if (glass != null && glass.allowedColorTag == colorTag)
                {
                    // Matching color — beam passes through, continuing in the same direction.
                    position = hit.point + direction * 0.02f;
                    // Intentionally no reflections++ — passing a filter is free.
                }
                else
                {
                    // Wrong color — beam is absorbed by the filter.
                    break;
                }
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
            // Disable the collider so the player can walk through the beam.
            // Beam segments are purely visual and need no physics presence.
            var col = beam.GetComponent<Collider>();
            if (col != null) col.enabled = false;
            activeBeams.Add(beam);
        }

        currentBeamIndex++;

        beam.transform.position = start + direction * (length / 2f);
        beam.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);
        beam.transform.localScale = new Vector3(0.05f, length / 2f, 0.05f);

        // Tint the beam segment to match this emitter's color.
        // Both _BaseColor (URP standard) and _Color (legacy/custom shaders) are set
        // so the color applies regardless of which property name the shader uses.
        var rend = beam.GetComponent<Renderer>();
        if (rend != null)
        {
            propBlock.SetColor("_BaseColor", beamColor);
            propBlock.SetColor("_Color", beamColor);
            propBlock.SetColor("_EmissionColor", beamColor);
            rend.SetPropertyBlock(propBlock);
        }
    }
}
