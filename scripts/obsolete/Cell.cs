// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Godot;

// public class Cell
// {
// 	public List<Particle> ParticlesInCell = new List<Particle>();

//     public (CellInfo, int[]) GetForGPU(int startIndex) {
//         int[] particleIndices = ParticlesInCell.Select((p) => p.OverallIndex).ToArray();
//         return (new CellInfo() {
//             startIndex = startIndex,
//             numberOfParticles = (uint)ParticlesInCell.Count(),
//         }, particleIndices);
//     }
//     public void RemoveParticle(Particle particle) {
//         ParticlesInCell.Remove(particle);
//     }
//     public void AddParticle(Particle particle) {
//         ParticlesInCell.Add(particle);
//     }
//     public void Clear() {
//         ParticlesInCell.Clear();
//     }
// }
