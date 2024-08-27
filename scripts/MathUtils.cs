using System;
using System.Linq;

public static class MathUtils {
    // Implement the fmod function
    public static T GetRandomItem<T>(T[] items, float[] probabilities, Random rand)
    {
        float[] normalizedProbabilites = SoftMax(probabilities);
        if (items.Length != normalizedProbabilites.Length)
        {
            throw new ArgumentException("The items array and normalizedProbabilites array must have the same length.");
        }

        float n = (float)rand.NextDouble();
        float cumulative = 0.0f;
        for (int i = 0; i < normalizedProbabilites.Length; i++)
        {
            cumulative += normalizedProbabilites[i];
            if (n < cumulative)
            {
                return items[i];
            }
        }
        // In case of any floating point precision issues, return the last item.
        return items[items.Length - 1];
    }
    public static float[] SoftMax(float[] values)
    {
        // Calculate the exponentials of all values
        float[] exponentials = values.Select(MathF.Exp).ToArray();

        // Calculate the sum of all exponentials
        float sumExponentials = exponentials.Sum();

        // Calculate the softmax for each value
        float[] softmax = exponentials.Select(e => e / sumExponentials).ToArray();

        return softmax;
    }
}