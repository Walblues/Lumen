using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KinematicMover : MonoBehaviour
{
    [Header("Oscillation Settings")]
    public Vector3 moveDirection = Vector3.right;
    public float distance = 3f;
    public float speed = 2f;

    private Vector3 _startPosition;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _startPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Use MovePosition — correct way to move kinematic Rigidbodies
        float offset = Mathf.Sin(Time.time * speed) * distance;
        Vector3 targetPosition = _startPosition + moveDirection.normalized * offset;
        _rb.MovePosition(targetPosition);
    }
}