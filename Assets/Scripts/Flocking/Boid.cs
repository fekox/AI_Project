using System;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Boid References")]
    public float speed = 2.5f;
    public float turnSpeed = 5f;
    public float detectionRadious = 3.0f;
    public float boidSeparationRadius = 4.0f;
    public float separationMultiplier = 2.0f;


    private Func<Boid, Vector2> Alignment;
    private Func<Boid, Vector2> Cohesion;
    private Func<Boid, Vector2> Separation;
    private Func<Boid, Vector2> Direction;

    public void Init(Func<Boid, Vector2> Alignment, 
                     Func<Boid, Vector2> Cohesion, 
                     Func<Boid, Vector2> Separation, 
                     Func<Boid, Vector2> Direction) 
    {
        this.Alignment = Alignment;
        this.Cohesion = Cohesion;
        this.Separation = Separation;
        this.Direction = Direction;
    }

    private void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        transform.up = Vector3.Lerp(transform.up, ACS(), turnSpeed * Time.deltaTime);
    }

    public Vector2 ACS()
    {
        Vector2 ACS = Alignment(this) + Cohesion(this) + Separation(this) * separationMultiplier + Direction(this);
        return ACS;
    }
}