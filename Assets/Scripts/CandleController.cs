using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CandleController : MonoBehaviour
{
    [Header("Candle")]
    public GameObject candle;
    public float candleActivationDistance = 2f;

    [Header("Door")]
    public DoorClose doorClose;
    public float doorActivationDistance = 3f;

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
    public float sceneLoadDelay = 3f; // How long after door closes before loading

    [Header("Sound")]
    public SoundFXManager soundFXManager;
    public AudioClip candleSnuffSound;
    public float soundVolume = 1f;

    [Header("Player")]
    public Transform player;

    private bool hasSnuffed = false;
    private bool hasTriggedDoor = false;

    private void Awake()
    {
        candle.SetActive(true);
    }

    private void Update()
{
    float distanceToCandle = Vector3.Distance(player.position, candle.transform.position);
    float distanceToDoor = Vector3.Distance(player.position, doorClose.transform.position);

    if (!hasTriggedDoor) {
        Debug.Log($"Candle distance: {distanceToCandle}, Door distance: {distanceToDoor}, Snuffed: {hasSnuffed}");
    }

    if (!hasSnuffed && distanceToCandle <= candleActivationDistance)
    {
        hasSnuffed = true;
        soundFXManager.PlaySound(candleSnuffSound, candle.transform, soundVolume);
        candle.SetActive(false);
    }

    if (hasSnuffed && !hasTriggedDoor && distanceToDoor <= doorActivationDistance)
    {
        hasTriggedDoor = true;
        doorClose.OnDoorInteract();
        StartCoroutine(LoadSceneAfterDelay());
    }
}

    private IEnumerator LoadSceneAfterDelay()
    {
        Debug.Log("Waiting...");
        yield return new WaitForSeconds(sceneLoadDelay);
        
        SceneManager.LoadScene(sceneToLoad);
    }
}