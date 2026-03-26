using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserReflection : MonoBehaviour
{
    public int maxReflections = 5;
    public float maxDistance = 100f;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        Vector3 direction = transform.forward;
        Vector3 position = transform.position;

        line.positionCount = 1;
        line.SetPosition(0, position);

        int reflections = 0;

        while (reflections < maxReflections)
        {
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, hit.point);

                // Reflect only if it's a mirror
                if (hit.collider.CompareTag("Mirror"))
                {
                    direction = Vector3.Reflect(direction, hit.normal);
                    position = hit.point;
                }
                else
                {
                    break;
                }
            }
            else
            {
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, position + direction * maxDistance);
                break;
            }

            reflections++;
        }
    }
}