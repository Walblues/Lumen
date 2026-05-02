using UnityEngine;

public class MirrorOutline : MonoBehaviour
{
    private static readonly Color CanRotateColor    = new Color(0x69 / 255f, 0xF9 / 255f, 0xFF / 255f);
    private static readonly Color CannotRotateColor = new Color(0xFF / 255f, 0x2E / 255f, 0x38 / 255f);

    private Outline outline;
    private RotatableMirror mirror;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        mirror = GetComponent<RotatableMirror>();

        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(_ => ShowOutline());
        interactable.hoverExited.AddListener(_ => outline.enabled = false);
    }

    void ShowOutline()
    {
        outline.OutlineColor = mirror != null && mirror.canRotate ? CanRotateColor : CannotRotateColor;
        outline.enabled = true;
    }
}