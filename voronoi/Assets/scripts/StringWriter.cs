using System.IO;

public static class StringWriter
{
    public static void WriteString(string content, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            // Write the string to the file
            writer.Write(content);
        }
    }
} 