using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DoorSceneTransition : MonoBehaviour
{
    [Header("Door")]
    public Transform doorPivot;
    public float doorOpenAngle = 90f;
    public float doorOpenSpeed = 2f;

    [Header("Sound")]
    public SoundFXManager soundFXManager;
    public AudioClip doorOpenSound;
    public float soundVolume = 1f;

    [Header("Fade")]
    public Renderer fadeQuad; // Drag your FadeQuad here
    public float fadeDuration = 1.5f;

    [Header("Scene")]
#if UNITY_EDITOR
    public SceneAsset sceneAsset;
    private void OnValidate()
    {
        if (sceneAsset != null)
            sceneToLoad = sceneAsset.name;
    }
#endif
    [HideInInspector] public string sceneToLoad;

    private bool hasTriggered = false;
    private Material fadeMaterial;

    void Start()
    {
        // Get a local instance of the material so we don't affect other objects
        fadeMaterial = fadeQuad.material;
        SetFadeAlpha(0f);
        fadeQuad.gameObject.SetActive(false);
    }

    public void OnDoorInteract()
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(TransitionSequence());
        }
    }

    private IEnumerator TransitionSequence()
    {
        soundFXManager.PlaySound(doorOpenSound, transform, soundVolume);

        yield return StartCoroutine(RotateDoorOpen());

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator RotateDoorOpen()
    {
        Quaternion startRotation = doorPivot.localRotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, doorOpenAngle, 0f);

        float elapsed = 0f;
        float duration = 1f / doorOpenSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            doorPivot.localRotation = Quaternion.Lerp(startRotation, targetRotation, smoothT);
            yield return null;
        }

        doorPivot.localRotation = targetRotation;
    }

    private IEnumerator FadeToBlack()
    {
        fadeQuad.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetFadeAlpha(Mathf.Clamp01(elapsed / fadeDuration));
            yield return null;
        }

        SetFadeAlpha(1f);
    }

    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeMaterial.color;
        color.a = alpha;
        fadeMaterial.color = color;
    }
}