using Godot;
using System;

public partial class Verlet
{
	public static void PhysicsProcess(Particle particle, float maxRadius, Particle[] otherParticles, InteractionMatrix interactionMatrix, float delta, float timeScale) {
        float dt = delta*timeScale;
        const float frictionHalfLife = 0.04f;
        float frictionFactor = MathF.Pow(0.5f, dt/frictionHalfLife);
        // Velocity Verlet integration
        Vector2 newParticlePosition = particle.ParticlePosition + particle.Velocity * dt + 0.5f * particle.Acceleration * dt * dt; // Literally MRUA
        Vector2 newAcceleration = particle.CalculateAcceleration(newParticlePosition, otherParticles, maxRadius, interactionMatrix);
        Vector2 newVelocity = frictionFactor*particle.Velocity + 0.5f * (particle.Acceleration + newAcceleration) * dt;
        particle.Velocity = newVelocity;
        particle.Acceleration = newAcceleration;
        
        particle.ParticlePosition = newParticlePosition;

        
    }
}
