using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Utility: place all your puzzle pieces in the scene, assign them to the slots below,
// then right-click the component header → "Apply Layout".
// Positions and rotations are relative to the Layout Origin transform (set it to
// wherever the centre of your puzzle should sit in the world).
// Safe to remove this component once everything is placed.
//
// Y rotation guide for mirrors (box collider, thin in local Z):
//   -45° : right→up,   up→right,   left→down,  down→left
//   +45° : right→down, down→right, left→up,    up→left
[System.Serializable]
public class PuzzleLayout : MonoBehaviour
{
    [System.Serializable]
    public struct PieceData
    {
        [Tooltip("Human-readable label — not used at runtime.")]
        public string label;
        public Transform piece;
        [Tooltip("World X position (relative to Layout Origin).")]
        public float x;
        [Tooltip("World Z position (relative to Layout Origin).")]
        public float z;
        [Tooltip("Y-axis rotation in degrees. 0 = no rotation.")]
        public float yRotation;
    }

    [Header("Origin")]
    [Tooltip("All X/Z coordinates are applied relative to this transform's position. " +
             "Place an empty GameObject where you want the puzzle centre to be.")]
    public Transform layoutOrigin;

    [Header("Height")]
    [Tooltip("Y (height) applied to every piece. " +
             "Leave at 0 and position the Layout Origin at the right height instead.")]
    public float heightY = 0f;

    [Header("Options")]
    [Tooltip("Also apply yRotation to each piece.")]
    public bool applyRotations = true;

    [Header("Pieces")]
    public PieceData[] pieces;

    // Populate with diagram coordinates when the component is first added.
    void Reset()
    {
        pieces = new PieceData[]
        {
            // ── Emitters ─────────────────────────────────────────────────────
            new PieceData { label = "Yellow Emitter",
                x = -6.3f, z =  2.6f, yRotation =   0f },
            new PieceData { label = "Red Emitter",
                x = -6.3f, z = -2.6f, yRotation =   0f },

            // ── Mirrors ───────────────────────────────────────────────────────
            // Mirror rotations are estimated from the beam paths in the diagram.
            // Use RotatableMirror.RotateCW / RotateCCW (45° snaps) to fine-tune.

            new PieceData { label = "Mirror – Upper-Left 1 (retractable)",
                x = -3.6f, z =  2.6f, yRotation = -45f },  // right → up

            new PieceData { label = "Mirror – Upper-Left 2 (retractable)",
                x = -3.6f, z =  1.4f, yRotation =  45f },  // up → left / right → down

            new PieceData { label = "Mirror – Top-Left (retractable)",
                x = -2.0f, z =  5.1f, yRotation = -45f },  // up → right

            new PieceData { label = "Mirror – Top-Center (non-retractable)",
                x =  1.3f, z =  5.1f, yRotation =  45f },  // right → down

            new PieceData { label = "Mirror – Top-Right (retractable)",
                x =  4.6f, z =  5.1f, yRotation =  45f },  // right → down

            new PieceData { label = "Mirror – Far-Right (retractable)",
                x =  6.8f, z =  3.5f, yRotation =  45f },  // up → left

            new PieceData { label = "Mirror – Right-Mid (non-retractable)",
                x =  4.6f, z =  1.4f, yRotation =  45f },  // down → right

            new PieceData { label = "Mirror – Middle (non-retractable)",
                x =  1.3f, z =  1.4f, yRotation = -45f },  // right → up

            new PieceData { label = "Mirror – Non-Rotatable",
                x = -0.1f, z = -2.6f, yRotation = -45f },  // down → right (toward combiner)

            new PieceData { label = "Mirror – Bottom-Left (retractable)",
                x = -4.6f, z = -5.1f, yRotation = -45f },  // left → down / up → right

            new PieceData { label = "Mirror – Bottom-Center (retractable)",
                x = -1.1f, z = -5.1f, yRotation =  45f },  // right → down / up → left

            // ── Combiner, Filter, Receiver ────────────────────────────────────
            new PieceData { label = "Combiner",
                x =  1.1f, z = -2.6f, yRotation =   0f },

            new PieceData { label = "Light Filter",
                x =  2.8f, z = -2.5f, yRotation =   0f },

            new PieceData { label = "Receiver (Fin)",
                x =  4.9f, z = -2.5f, yRotation =   0f },
        };
    }

    [ContextMenu("Apply Layout")]
    void ApplyLayout()
    {
        Vector3 origin = layoutOrigin != null ? layoutOrigin.position : Vector3.zero;

#if UNITY_EDITOR
        var targets = new List<Object>();
        foreach (var d in pieces)
            if (d.piece != null) targets.Add(d.piece);
        Undo.RecordObjects(targets.ToArray(), "Apply Puzzle Layout");
#endif

        int count = 0;
        foreach (var data in pieces)
        {
            if (data.piece == null) continue;
            data.piece.position = origin + new Vector3(data.x, heightY, data.z);
            if (applyRotations)
                data.piece.rotation = Quaternion.Euler(0f, data.yRotation, 0f);
            count++;
        }

        Debug.Log($"PuzzleLayout: placed {count} / {pieces.Length} pieces.");
    }
}
