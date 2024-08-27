using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
public struct Cell {
	public int start;
	public int end;
	public int left;
	public int upLeft;
	public int up;
	public int upRight;
	public int right;
	public int downRight;
	public int down;
	public int downLeft;
}

public struct CellArrayBuffers {
	public int[] particleCellMembership;
	public Cell[] cellsArray;
	public int[] flatCelledIndices;
}

public class CellGrid {
    public int xCells;
    public int yCells;
    public float maxRadius;

    public CellGrid(int xCells, int yCells, float maxRadius) {
        this.xCells = xCells;
        this.yCells = yCells;
        this.maxRadius = maxRadius;
    }
    public CellArrayBuffers BuildParticleCells(Particle[] particles) {
	    var celledIndices = new List<int>[xCells, yCells];
		var particleCellMembership = new int[particles.Length];
		var flatCelledParticleIndices = new List<int>();
		var bins = new List<Cell>();

		for (int i = 0; i < particles.Length; i++) {
			var particle = particles[i];
			var binX = (int) (particle.ParticlePosition.X / maxRadius);
			var binY = (int) (particle.ParticlePosition.Y / maxRadius);
            
			celledIndices[binX, binY] = new List<int>();
            celledIndices[binX, binY].Add(i);
			particleCellMembership[i] = binX + binY * xCells;
		}
		
		for (int j = 0; j < yCells; j++) {
			for (int i = 0; i < xCells; i++) {
				celledIndices[i, j] ??= new List<int>();
				var celledParticleIndices = celledIndices[i, j];
				var start = flatCelledParticleIndices.Count;
				var end = start + celledParticleIndices.Count;
				flatCelledParticleIndices.AddRange(celledParticleIndices);
				var bin = new Cell {
					start = start,
					end = end,
					left = (i - 1 + xCells) % xCells + j * xCells,
					upLeft = (i - 1 + xCells) % xCells + ((j - 1 + yCells) % yCells) * xCells,
					up = i + ((j - 1 + yCells) % yCells) * xCells,
					upRight = (i + 1) % xCells + (j - 1 + yCells) % yCells * xCells,
					right = (i + 1) % xCells + j * xCells,
					downRight = (i + 1) % xCells + (j + 1) % yCells * xCells,
					down = i + (j + 1) % yCells * xCells,
					downLeft = (i - 1 + xCells) % xCells + (j + 1) % yCells * xCells,
				};
				bins.Add(bin);
			}
		}

		return new CellArrayBuffers {
			particleCellMembership = particleCellMembership,
			cellsArray = bins.ToArray(),
			flatCelledIndices = flatCelledParticleIndices.ToArray()
		};
	}
}