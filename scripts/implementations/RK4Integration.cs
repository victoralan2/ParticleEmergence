using Godot;
using System;

public partial class RK4Integration
{
	public static void PhysicsProcess(Particle particle, float maxRadius, Particle[] otherParticles, InteractionMatrix interactionMatrix, float delta, float timeScale) {
        float dt = delta*timeScale;
        const float frictionHalfLife = 0.04f;
        float frictionFactor = MathF.Pow(0.5f, dt/frictionHalfLife);

        // RK4 integration
        Vector2 initialParticlePosition = particle.ParticlePosition;
        Vector2 initialVelocity = frictionFactor*particle.Velocity;
        Vector2 initialAcceleration = particle.Acceleration;

        // Calculate k1
        Vector2 k1_ParticlePosition = initialVelocity;
        Vector2 k1_velocity = initialAcceleration;

        // Calculate k2
        Vector2 midParticlePosition1 = initialParticlePosition + 0.5f * dt * k1_ParticlePosition;
        Vector2 midVelocity1 = initialVelocity + 0.5f * dt * k1_velocity;
        Vector2 midAcceleration1 = particle.CalculateAcceleration(midParticlePosition1, otherParticles, maxRadius, interactionMatrix);

        Vector2 k2_ParticlePosition = midVelocity1;
        Vector2 k2_velocity = midAcceleration1;

        // Calculate k3
        Vector2 midParticlePosition2 = initialParticlePosition + 0.5f * dt * k2_ParticlePosition;
        Vector2 midVelocity2 = initialVelocity + 0.5f * dt * k2_velocity;
        Vector2 midAcceleration2 = particle.CalculateAcceleration(midParticlePosition2, otherParticles, maxRadius, interactionMatrix);

        Vector2 k3_ParticlePosition = midVelocity2;
        Vector2 k3_velocity = midAcceleration2;

        // Calculate k4
        Vector2 endParticlePosition = initialParticlePosition + dt * k3_ParticlePosition;
        Vector2 endVelocity = initialVelocity + dt * k3_velocity;
        Vector2 endAcceleration = particle.CalculateAcceleration(endParticlePosition, otherParticles, maxRadius, interactionMatrix);

        Vector2 k4_ParticlePosition = endVelocity;
        Vector2 k4_velocity = endAcceleration;

        // Update ParticlePosition and velocity
        Vector2 newParticlePosition = initialParticlePosition + (dt / 6.0f) * (k1_ParticlePosition + 2.0f * k2_ParticlePosition + 2.0f * k3_ParticlePosition + k4_ParticlePosition);
        Vector2 newVelocity = initialVelocity + (dt / 6.0f) * (k1_velocity + 2.0f * k2_velocity + 2.0f * k3_velocity + k4_velocity);

        // Update transform and particleect properties
        particle.ParticlePosition = newParticlePosition;
        particle.Velocity = newVelocity;
        Vector2 newAcceleration = particle.CalculateAcceleration(newParticlePosition, otherParticles, maxRadius, interactionMatrix);
        particle.Acceleration = newAcceleration;


    }
}
