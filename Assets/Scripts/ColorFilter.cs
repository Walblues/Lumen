using UnityEngine;

// Place on a plane. Tag the GameObject "ColoredGlass".
// Beams whose colorTag matches allowedColorTag pass straight through.
// All other beam colors are stopped here.
//
// Material setup:
//   1. Create a new Material using URP/Lit (or URP/Unlit).
//   2. Set Surface Type to "Transparent".
//   3. Leave the Base Color white — this script drives the tint and alpha via
//      MaterialPropertyBlock, so the material itself stays shared/uninstanced.
[RequireComponent(typeof(Renderer))]
public class ColorFilter : MonoBehaviour
{
    [Tooltip("The beam colorTag that passes through. All other colors are blocked.")]
    public string allowedColorTag = "white";

    [Tooltip("Visual tint of the glass. Alpha controls transparency (0.3 is a good starting point).")]
    public Color glassColor = new Color(1f, 1f, 1f, 0.3f);

    [Tooltip("Emission brightness multiplier. 0 = no glow, 1 = matches base color, 2+ = HDR bloom.")]
    public float emissionIntensity = 0.8f;

    private MaterialPropertyBlock propBlock;

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        ApplyColor();
    }

#if UNITY_EDITOR
    // Refresh the tint in the Scene view whenever values are changed in the Inspector.
    void OnValidate()
    {
        if (propBlock == null) propBlock = new MaterialPropertyBlock();
        ApplyColor();
    }
#endif

    void ApplyColor()
    {
        var rend = GetComponent<Renderer>();
        if (rend == null) return;
        propBlock.SetColor("_BaseColor", glassColor);
        propBlock.SetColor("_Color", glassColor);
        // Emission uses HDR — multiply the RGB by intensity (alpha is ignored for emission).
        Color emissionColor = new Color(glassColor.r, glassColor.g, glassColor.b) * emissionIntensity;
        propBlock.SetColor("_EmissionColor", emissionColor);
        rend.SetPropertyBlock(propBlock);
    }
}
