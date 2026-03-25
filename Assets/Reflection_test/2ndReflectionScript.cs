using UnityEngine;
using System.Collections.Generic;

public class VolumetricLaser : MonoBehaviour
{
    public GameObject beamPrefab;
    public int maxReflections = 5;
    public float maxDistance = 100f;
    int currentBeamIndex = 0;
    
    private List<GameObject> activeBeams = new List<GameObject>();

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        currentBeamIndex = 0;
        Vector3 direction = transform.forward;
        Vector3 position = transform.position;

        int reflections = 0;

        while (reflections < maxReflections)
        {
            Ray ray = new Ray(position, direction);
            RaycastHit hit;

            float distance;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                distance = hit.distance;

                CreateBeamSegment(position, direction, distance);

                if (hit.collider.CompareTag("Mirror"))
                {
                    direction = Vector3.Reflect(direction, hit.normal);
                    position = hit.point + direction * 0.01f; // prevent sticking
                }
                else
                {
                    break;
                }
            }
            else
            {
                distance = maxDistance;
                CreateBeamSegment(position, direction, distance);
                break;
            }

            reflections++;
        }
        for (int i = currentBeamIndex; i < activeBeams.Count; i++)
        {
            activeBeams[i].SetActive(false);
        }
    }

    void CreateBeamSegment(Vector3 start, Vector3 direction, float length)
    {
        GameObject beam;

        if (currentBeamIndex < activeBeams.Count)
        {
            beam = activeBeams[currentBeamIndex];
            beam.SetActive(true);
        }
        else
        {
            beam = Instantiate(beamPrefab);
            activeBeams.Add(beam);
        }

        currentBeamIndex++;

        beam.transform.position = start + direction * (length / 2f);
        beam.transform.rotation = Quaternion.LookRotation(direction);
        beam.transform.localScale = new Vector3(0.1f, length / 2f, 0.1f);
    }
}