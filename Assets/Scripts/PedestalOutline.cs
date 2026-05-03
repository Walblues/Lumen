using UnityEngine;

public class PedestalOutline : MonoBehaviour
{
    private static readonly Color CanRetractColor    = new Color(0x69 / 255f, 0xF9 / 255f, 0xFF / 255f);
    private static readonly Color CannotRetractColor = new Color(0xFF / 255f, 0x2E / 255f, 0x38 / 255f);

    private Outline outline;
    private MirrorPedestal pedestal;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        pedestal = GetComponentInParent<MirrorPedestal>();

        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(_ => ShowOutline());
        interactable.hoverExited.AddListener(_ => outline.enabled = false);
    }

    void ShowOutline()
    {
        outline.OutlineColor = pedestal != null && pedestal.isRetractable ? CanRetractColor : CannotRetractColor;
        outline.enabled = true;
    }
}