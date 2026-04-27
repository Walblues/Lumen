using UnityEngine;


public class InteractableOutline : MonoBehaviour
{
    private Outline outline; // From Quick Outline asset
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(_ => outline.enabled = true);
        interactable.hoverExited.AddListener(_ => outline.enabled = false);
    }
}