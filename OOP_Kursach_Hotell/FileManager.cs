using System.Collections.Generic;
using System.IO;

namespace OOP_Kursach_Guest
{
    public static class FileManager
    {
        /// <summary>
        /// Путь к файлу с данными.
        /// </summary>
        private static string filePath = "input.txt";
        public static List<Guest> ReadFromFile()
        {
            List<Guest> Guests = new List<Guest>();
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 5 && long.TryParse(parts[2], out long phonenumber) && int.TryParse(parts[3], out int room) && bool.TryParse(parts[4], out bool onLiving))
                    {
                        Guests.Add(new Guest(parts[0], parts[1], phonenumber, room, onLiving));
                    }
                }
            }
            return Guests;
        }

        public static void WriteToFile(List<Guest> Guests)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (var Guest in Guests)
                {
                    sw.WriteLine($"{Guest.Name}|{Guest.Surname}|{Guest.PhoneNumber}|{Guest.Room}|{Guest.OnLiving}");
                }
            }
        }

        public static void AppendToFile(Guest Guest)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine($"{Guest.Name}|{Guest.Surname}|{Guest.PhoneNumber}|{Guest.Room}|{Guest.OnLiving}");
            }
        }

        /// <summary>
        /// Удаляет файл с данными.
        /// </summary>
        public static void DeleteFile()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
