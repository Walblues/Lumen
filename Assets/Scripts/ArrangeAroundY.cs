using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Utility: add to a parent GameObject, configure the fields, then right-click
// the component header in the Inspector and choose "Arrange Children Around Y Axis".
// Each child is placed radius units out from the centre and rotated so its local
// Y axis (the cylinder height / LightEmitter emission axis) points outward.
// Safe to remove this component once you're happy with the placement.
public class ArrangeAroundY : MonoBehaviour
{
    [Tooltip("Distance from the parent's centre to each child's pivot.")]
    public float radius = 1f;

    [Tooltip("Vertical offset applied to every child (useful if the arms should sit " +
             "above or below the parent's origin).")]
    public float heightOffset = 0f;

    [ContextMenu("Arrange Children Around Y Axis")]
    void Arrange()
    {
        int count = transform.childCount;
        if (count == 0)
        {
            Debug.LogWarning("ArrangeAroundY: no children to arrange.");
            return;
        }

#if UNITY_EDITOR
        // Record every child so the action can be undone with Ctrl+Z.
        var targets = new Object[count];
        for (int i = 0; i < count; i++)
            targets[i] = transform.GetChild(i);
        Undo.RecordObjects(targets, "Arrange Children Around Y Axis");
#endif

        for (int i = 0; i < count; i++)
        {
            float angleDeg = (360f / count) * i;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // Outward radial direction for this slot (in parent's local XZ plane).
            Vector3 outward = new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));

            Transform child = transform.GetChild(i);
            child.localPosition = outward * radius + Vector3.up * heightOffset;

            // Rotate so local Y (the cylinder height / emission axis) points outward.
            child.localRotation = Quaternion.FromToRotation(Vector3.up, outward);
        }

        Debug.Log($"ArrangeAroundY: arranged {count} children at {360f / count}° intervals.");
    }
}
