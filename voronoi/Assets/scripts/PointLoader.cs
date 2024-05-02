using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PointLoader
{
    public static List<Vector2> LoadPoints(string filePath)
    {
        var points = new List<Vector2>();

        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                // Split the line into point strings
                string[] pointStrings = line.Split(new[] { ')' }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (string pointString in pointStrings)
                {
                    // Trim the parentheses and split by comma
                    string[] coordinates = pointString.Trim('(', ' ', ')').Split(',');

                    // Parse the coordinates and add to the list
                    float x = float.Parse(coordinates[0]);
                    float y = float.Parse(coordinates[1]);
                    points.Add(new Vector2(x, y));
                }
            }
        }

        return points;
    }
}