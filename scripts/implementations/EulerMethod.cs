using System;
using Godot;

public partial class EulerMethod
{
	public static void PhysicsProcess(Particle particle, float maxRadius, Particle[] otherParticles, InteractionMatrix interactionMatrix, float delta, float timeScale) {
        float dt = delta*timeScale;
        const float frictionHalfLife = 0.04f;

        float frictionFactor = MathF.Pow(0.5f, dt/frictionHalfLife);
        
        Vector2 newParticlePosition = particle.ParticlePosition + frictionFactor*particle.Velocity * dt;
        Vector2 newAcceleration = particle.CalculateAcceleration(newParticlePosition, otherParticles, maxRadius, interactionMatrix);
        Vector2 newVelocity = frictionFactor*particle.Velocity + newAcceleration * dt;
        particle.Acceleration = newAcceleration;
        particle.Velocity = newVelocity;
        particle.ParticlePosition = newParticlePosition;
    }
}