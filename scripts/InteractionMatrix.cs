using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class InteractionMatrix {
    private Dictionary<Color, Dictionary<Color, float>> Matrix;
    private Dictionary<Color, Dictionary<Color, float>> ChangeMatrix;
    private double TimeSinceLastChange = 0;
    private Random rng;

    public float GetAtraction(Color from, Color to) {
        return Matrix[from][to];
    }
    public float[] AsSimple() {
        int innerLength = Matrix.Values.First().Count;
        float[] newMatrix = new float[innerLength * Matrix.Count];
        int i = 0;
        // Flatten the dictionary
        foreach (var outerPair in Matrix)
        {
            int j = 0;
            foreach (var innerPair in outerPair.Value)
            {
                newMatrix[i * innerLength + j] = innerPair.Value;
                j++;
            }
            i++;
        }
        return newMatrix;
    }
    public InteractionMatrix(Color[] colors, Random rand) {
        Dictionary<Color, Dictionary<Color, float>> matrix = new Dictionary<Color, Dictionary<Color, float>>();
        foreach (Color color in colors) {
            matrix[color] = new Dictionary<Color, float>();
            foreach (Color otherColor in colors) {
                matrix[color][otherColor] = RandomNumber(rand);
            }
        }
        Dictionary<Color, Dictionary<Color, float>> changeMatrix = new Dictionary<Color, Dictionary<Color, float>>();
        foreach (Color color in colors) {
            changeMatrix[color] = new Dictionary<Color, float>();
            foreach (Color otherColor in colors) {
                changeMatrix[color][otherColor] = RandomNumber(rand);
            }
        }

        this.rng = rand;
        this.Matrix = matrix;
        this.ChangeMatrix = changeMatrix;
    }
    public InteractionMatrix(Dictionary<Color, Dictionary<Color, float>> matrix) {
        this.Matrix = matrix;
    }

    public void Next(double delta) {
        this.TimeSinceLastChange += delta;
        if (this.TimeSinceLastChange > 10.0) {
            this.TimeSinceLastChange = 0.0;
            foreach (var entry in this.ChangeMatrix) {
                var color = entry.Key;
                foreach (var otherColor in entry.Value) {
                    this.ChangeMatrix[color][otherColor.Key] = (float) (((this.rng.NextDouble() * 2) - 1) / 10.0);
                }
            }
        } else {
            foreach (var entry in this.Matrix) {
                var color = entry.Key;
                foreach (var otherColor in entry.Value) {
                    this.Matrix[color][otherColor.Key] = Math.Clamp(this.Matrix[color][otherColor.Key] + (this.ChangeMatrix[color][otherColor.Key] * (float) delta), -1, 1);
                }
            }
        }
    }

    public static float RandomNumber(Random rand) {
        double u = rand.NextDouble(); // Generate a random number between 0 and 1
        double x = Math.Pow(u, 1); // Linear transformation
        float sign = MathF.Sign((float)rand.NextDouble() - .5f);
        return (float)x * sign;
    }
}