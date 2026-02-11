using System;
using System.IO;
using System.Xml.Serialization;

public static class ProgressManager
{
    private static string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "meteor_dodger_progress.xml"
    );

    public static void Save(ProgressData data)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProgressData));
            serializer.Serialize(fs, data);
        }
    }

    public static ProgressData Load()
    {
        if (!File.Exists(path))
            return new ProgressData();

        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProgressData));
            return (ProgressData)serializer.Deserialize(fs);
        }
    }
}
