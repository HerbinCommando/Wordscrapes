using System;
using System.Collections.Generic;
using System.Linq;

public static class ArrayExtensions
{
    private static Random rng = new Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T[][] Shuffle<T>(this T[][] array)
    {
        // Create a new list of lists to hold the shuffled result
        List<List<T>> newList = array.Select(subArray => subArray.ToList()).ToList();

        // Shuffle each sub-list
        foreach (var subList in newList)
        {
            subList.Shuffle();
        }

        // Shuffle the main list
        newList.Shuffle();

        // Convert the list of lists back to a jagged array
        return newList.Select(subList => subList.ToArray()).ToArray();
    }
}
