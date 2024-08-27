// using Godot;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;

// public class CellGrid
// {
//     public Vector2I cellGridDimensions;
//     public float maxRadius;
//     public List<List<Cell>> Cells = new List<List<Cell>>(); // First X then Y (First row then columns)

//     public CellGrid(Vector2I cellGridDimensions, float maxRadius) {
//         this.cellGridDimensions = cellGridDimensions;
//         this.maxRadius = maxRadius;

//         for (int x = 0; x < cellGridDimensions.X; x++) {
//             Cells.Add(new List<Cell>());
//             for (int y = 0; y < cellGridDimensions.Y; y++) {
//                 Cells[x].Add(new Cell());
//             }
//         }
//     }

//     public void Clear() { 
//         foreach (var cellRow in Cells) {
//             foreach (var cell in cellRow) {
//                 cell.Clear();
//             }
//         }
//     }
//     public (CellInfo[], int[]) GetForGPU() {
//         List<int> indices = new List<int>();
//         List<CellInfo> cellInfos = new List<CellInfo>();
//         int currentIndex = 0;
//         foreach (Cell cell in Cells.SelectMany(i => i)) {
//             (CellInfo info, int[] cellIndices) = cell.GetForGPU(currentIndex);
//             indices = indices.Concat(cellIndices).ToList();
//             currentIndex += (int)info.numberOfParticles;
//             cellInfos.Add(info);
//         }
//         return (cellInfos.ToArray(), indices.ToArray());
//     }
//     public void UpdateParticle(Particle particle) {
//         var currentCellIndex = particle.CellIndex;
//         var correctCellIndex = GetCellAt(particle.ParticlePosition);

//         if (correctCellIndex != currentCellIndex) {
//             RemoveParticle(particle);
//             AddParticle(particle);
//         }
//     }

//     public void AddParticle(Particle particle) {
//         var cellIndex = GetCellAt(particle.ParticlePosition);
//         try {
//             Cells[cellIndex.X][cellIndex.Y].AddParticle(particle);

//         } catch {
//             Debug.Print("X: " + cellIndex.X);
//             Debug.Print("Y: " + cellIndex.Y);
//             Debug.Print("Pos: " + particle.ParticlePosition);

//         }
        
//         particle.CellIndex = cellIndex;
//     }
//     public void RemoveParticle(Particle particle) {
//         var cellIndex = particle.CellIndex;
//         Cells[cellIndex.X][cellIndex.Y].RemoveParticle(particle);
//     }

//     public Vector2I GetCellAt(Vector2 point) {
//         int x = Mathf.FloorToInt(point.X / maxRadius);
//         int y = Mathf.FloorToInt(point.Y / maxRadius);
//         return new Vector2I(x, y);
//     }

//     // public List<Particle> GetCloseParticles(Particle particle) {
//     //     var cellCoords = GetCellAt(particle.ParticlePosition);
//     //     var adjacentCells = GetAdjacentCells(cellCoords);
//     //     var particlesInCell = Cells[cellCoords.X][cellCoords.Y].ParticlesInCell;
//     //     var closeParticles = new List<Particle>(particlesInCell);

//     //     foreach (var cell in adjacentCells) {
//     //         closeParticles.AddRange(cell.ParticlesInCell);
//     //     }

//     //     return closeParticles;
//     // }

//     // public List<Cell> GetAdjacentCells(Vector2I cellCoords) {
//     //     var cells = new List<Cell>();
//     //     int x = cellCoords.X;
//     //     int y = cellCoords.Y;

//     //     if (IsValidCell(x, y - 1)) cells.Add(Cells[x][y - 1]); // UP
//     //     if (IsValidCell(x - 1, y - 1)) cells.Add(Cells[x - 1][y - 1]); // UP & LEFT
//     //     if (IsValidCell(x + 1, y - 1)) cells.Add(Cells[x + 1][y - 1]); // UP & RIGHT
//     //     if (IsValidCell(x, y + 1)) cells.Add(Cells[x][y + 1]); // DOWN
//     //     if (IsValidCell(x - 1, y + 1)) cells.Add(Cells[x - 1][y + 1]); // DOWN & LEFT
//     //     if (IsValidCell(x + 1, y + 1)) cells.Add(Cells[x + 1][y + 1]); // DOWN & RIGHT
//     //     if (IsValidCell(x - 1, y)) cells.Add(Cells[x - 1][y]); // LEFT
//     //     if (IsValidCell(x + 1, y)) cells.Add(Cells[x + 1][y]); // RIGHT

//     //     return cells;
//     // }

//     // private bool IsValidCell(int x, int y) {
//     //     return x >= 0 && x < cellGridDimensions.X && y >= 0 && y < cellGridDimensions.Y;
//     // }
// }