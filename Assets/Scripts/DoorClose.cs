using System.Collections;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    [Header("Door")]
    public Transform doorPivot;
    public float ajarAngle = 90f;
    public float doorCloseSpeed = 2f;

    [Header("Light")]
    public Light outsideLight; // Drag the Point Light here
    public Renderer outsideCubeRenderer; // Drag the cube here
    public float lightFadeDuration = 0.3f; // How quickly light fades after door closes

    [Header("Sound")]
    public SoundFXManager soundFXManager;
    public AudioClip doorCloseSound;
    public float soundVolume = 1f;

    private bool hasTriggered = false;
    private Quaternion openRotation;
    private Quaternion closedRotation;
    private Material cubeMaterial;
    private float initialLightIntensity;

    private void Awake()
    {
        closedRotation = doorPivot.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, ajarAngle, 0f);
        doorPivot.localRotation = openRotation;

        // Cache the initial light intensity so we can fade from it
        initialLightIntensity = outsideLight.intensity;

        // Get a local instance of the cube's material
        cubeMaterial = outsideCubeRenderer.material;
    }

    public void OnDoorInteract()
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(RotateDoorClosed());
        }
    }

    private IEnumerator RotateDoorClosed()
    {
        soundFXManager.PlaySound(doorCloseSound, transform, soundVolume);

        float elapsed = 0f;
        float duration = 1f / doorCloseSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = t * t * t;
            doorPivot.localRotation = Quaternion.Lerp(openRotation, closedRotation, smoothT);
            yield return null;
        }

        doorPivot.localRotation = closedRotation;

        // Fade the light out after the door finishes closing
        yield return StartCoroutine(FadeOutLight());
    }

    private IEnumerator FadeOutLight()
    {
        float elapsed = 0f;

        while (elapsed < lightFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Clamp01(elapsed / lightFadeDuration);

            // Fade the point light intensity
            outsideLight.intensity = initialLightIntensity * t;

            // Fade the emission on the cube material
            cubeMaterial.SetColor("_EmissionColor", Color.white * t);

            yield return null;
        }

        // Make sure everything is fully off
        outsideLight.intensity = 0f;
        outsideLight.gameObject.SetActive(false);
        cubeMaterial.SetColor("_EmissionColor", Color.black);
    }
}