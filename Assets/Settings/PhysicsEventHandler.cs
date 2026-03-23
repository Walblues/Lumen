using UnityEngine;

public class PhysicsEventHandler : MonoBehaviour
{
    public enum EventMode { Trigger, Collision }
    public enum ActionType { ChangeColor, ShowObject, HideObject, SpawnObject }

    [Header("Event Settings")]
    public EventMode eventMode = EventMode.Trigger;
    public string targetTag = "Projectile"; // Only react to this tag ("" = react to anything)

    [Header("Action Settings")]
    public ActionType actionType = ActionType.ChangeColor;
    public Color newColor = Color.red;        // Used by ChangeColor
    public GameObject targetObject;           // Used by Show/Hide
    public GameObject prefabToSpawn;          // Used by SpawnObject
    public Transform spawnPoint;              // Used by SpawnObject

    private bool _hasTriggered = false;       // Fire once only

    // ── Trigger mode ──────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (eventMode != EventMode.Trigger) return;
        if (!IsValidTarget(other.gameObject)) return;
        FireEvent();
    }

    // ── Collision mode ────────────────────────────────────────
    void OnCollisionEnter(Collision collision)
    {
        if (eventMode != EventMode.Collision) return;
        if (!IsValidTarget(collision.gameObject)) return;
        FireEvent();
    }

    // ── Shared logic ──────────────────────────────────────────
    bool IsValidTarget(GameObject other)
    {
        if (_hasTriggered) return false;
        if (targetTag != "" && !other.CompareTag(targetTag)) return false;
        return true;
    }

    void FireEvent()
    {
        _hasTriggered = true;

        switch (actionType)
        {
            case ActionType.ChangeColor:
                var renderer = GetComponent<Renderer>();
                if (renderer) renderer.material.color = newColor;
                break;

            case ActionType.ShowObject:
                if (targetObject) targetObject.SetActive(true);
                break;

            case ActionType.HideObject:
                if (targetObject) targetObject.SetActive(false);
                break;

            case ActionType.SpawnObject:
                if (prefabToSpawn && spawnPoint)
                    Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
                break;
        }
    }
}