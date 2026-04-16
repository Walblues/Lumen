using UnityEngine;
using System.Collections.Generic;

// Attach to the central cylinder of the combiner prefab.
// Wire up three LightReceiver components (on the input arm cylinders) and
// one LightEmitter (on the output arm cylinder) in the Inspector.
//
// Behaviour:
//   - No inputs active          → output emitter is silent.
//   - Exactly one color active  → output fires that same color (pass-through).
//   - A matching CombineRule    → output fires the rule's pre-set color.
//   - Multiple colors, no rule  → output is silent (ambiguous / unsolved).
//
// CombineRules are checked first, so a two-color combination always wins
// over a single-color pass-through even if only one of the two is present.
public class LightCombiner : MonoBehaviour
{
    [System.Serializable]
    public struct CombineRule
    {
        [Tooltip("First input color tag required for this combination.")]
        public string colorTagA;
        [Tooltip("Second input color tag required for this combination.")]
        public string colorTagB;
        [Tooltip("Color tag the output emitter will use when this rule fires.")]
        public string outputColorTag;
        [Tooltip("Visual beam color the output emitter will use when this rule fires.")]
        public Color outputBeamColor;
    }

    [Header("Inputs")]
    [Tooltip("LightReceiver components on the three input arm cylinders.")]
    public LightReceiver[] inputReceivers;

    [Header("Output")]
    [Tooltip("LightEmitter component on the output arm cylinder.")]
    public LightEmitter outputEmitter;

    [Header("Combine Rules")]
    [Tooltip("Each rule defines two input colors that combine into a specific output color. " +
             "Rules are evaluated in order — the first match wins.")]
    public CombineRule[] combineRules;

    void LateUpdate()
    {
        // Collect every unique color tag currently active across all input receivers.
        var activeColors = new HashSet<string>();
        foreach (var receiver in inputReceivers)
        {
            if (receiver != null && receiver.IsActivated && receiver.CurrentColorTag != "")
                activeColors.Add(receiver.CurrentColorTag);
        }

        // --- Check combine rules first (two-color combinations take priority) ---
        foreach (var rule in combineRules)
        {
            if (activeColors.Contains(rule.colorTagA) && activeColors.Contains(rule.colorTagB))
            {
                SetOutput(true, rule.outputColorTag, rule.outputBeamColor);
                return;
            }
        }

        // --- Single color pass-through ---
        if (activeColors.Count == 1)
        {
            // Find the active receiver to read its visual beam color.
            foreach (var receiver in inputReceivers)
            {
                if (receiver != null && receiver.IsActivated)
                {
                    SetOutput(true, receiver.CurrentColorTag, receiver.CurrentBeamColor);
                    return;
                }
            }
        }

        // --- No valid combination — silence the output ---
        SetOutput(false, "", Color.clear);
    }

    void SetOutput(bool active, string colorTag, Color beamColor)
    {
        if (outputEmitter == null) return;
        outputEmitter.isActive   = active;
        outputEmitter.colorTag   = colorTag;
        outputEmitter.beamColor  = beamColor;
    }
}
