using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public Transform target;
    public int boidCount = 50;
    public Boid boidPrefab;
    private List<Boid> boids = new List<Boid>();

    private void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            GameObject boidGO = Instantiate(boidPrefab.gameObject,
                new Vector3(Random.Range(-10, 10), Random.Range(-10, 10)), Quaternion.identity, transform);

            Boid boid = boidGO.GetComponent<Boid>();

            boid.Init(Alignment, Cohesion, Separation, Direction);
            boids.Add(boid);
        }
    }

    public Vector2 Alignment(Boid boid)
    {
        List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector2 avg = Vector2.zero;

        if (insideRadiusBoids.Count == 0)
        {
            return transform.up;
        }

        foreach (Boid b in insideRadiusBoids)
        {
            if (b == boid)
                continue;
            avg += (Vector2)b.transform.up;
        }

        avg /= insideRadiusBoids.Count;
        return avg.normalized;
    }

    public Vector2 Cohesion(Boid boid)
    {
        List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector2 avg = Vector2.zero;

        if (insideRadiusBoids.Count == 0)
        {
            return Vector2.zero;
        }

        foreach (Boid b in insideRadiusBoids)
        {
            if (b == boid) 
            {
                continue;
            }

            avg += (Vector2)b.transform.position;
        }

        avg /= insideRadiusBoids.Count;
        return (avg - (Vector2)boid.transform.position).normalized;
    }

    public Vector2 Separation(Boid boid)
    {
        List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector2 avg = Vector2.zero;

        if (insideRadiusBoids.Count == 0)
        {
            return Vector2.zero;
        }

        foreach (Boid b in insideRadiusBoids)
        {
            if (b == boid) 
            {
                continue;
            }

            avg += ((Vector2)boid.transform.position - (Vector2)b.transform.position);
        }

        avg /= insideRadiusBoids.Count;
        return avg.normalized;
    }

    public Vector2 Direction(Boid boid)
    {
        return (target.position - boid.transform.position).normalized;
    }

    public List<Boid> GetBoidsInsideRadius(Boid boid)
    {
        List<Boid> insideRadiusBoids = new List<Boid>();

        foreach (Boid b in boids)
        {
            if (Vector2.Distance(boid.transform.position, b.transform.position) < boid.detectionRadious)
            {
                insideRadiusBoids.Add(b);
            }
        }

        return insideRadiusBoids;
    }
}