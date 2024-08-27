using Godot;
using System;
using System.Diagnostics;

public partial class Particle
{
    public Color Color; // Color also defins the behavior of the particle
    public Vector2 ParticlePosition;
    public Vector2 Velocity;
    public Vector2 Acceleration;
    public readonly float Radius;
    public Vector2I CellIndex;
    public int OverallIndex;
    public Particle(Color color, Vector2 position, Vector2 velocity, float radius) {
        this.Color = color;
        this.ParticlePosition = position;
        this.Velocity = velocity;
        this.Radius = radius;
    }
    
    public Vector2 CalculateAcceleration(Vector2 position, Particle[] otherObjects, float maxRadius, InteractionMatrix interactionMatrix)
    {
        Vector2 acceleration = Vector2.Zero;
        float forceMultiplier = 1f;
        foreach (Particle other in otherObjects) {
            if (other == this) {
                continue;
            }
            Vector2 difference = other.ParticlePosition - position;
            float distance = difference.Length();

            if (distance == 0 || distance > maxRadius) {
                continue;
            }
            float distanceNormalized = distance / maxRadius;
            float attractionFactor = interactionMatrix.GetAtraction(this.Color, other.Color);

            float forceModule = CalculateAttraction(distanceNormalized, attractionFactor);

            Vector2 forceVector = difference * (forceModule / distance);
            acceleration += forceVector;
        }
        return acceleration * maxRadius * forceMultiplier; 
    }
    public float CalculateAttraction(float distance, float attractionFactor) {
        const float BETA = 0.5f;

        if (distance < BETA) {
            return distance/BETA - 1;
        }
        if (distance < 1f) {
            return attractionFactor * (1 - MathF.Abs(2*distance - 1 - BETA) / (1-BETA));
        }
        return 0;
    }
}
