
using Newtonsoft.Json;
using Skynet.Services;

namespace Skynet.db
{
    /// <summary>
    /// Contains static methods to help deal with Json files.
    /// </summary>
    public static class ReaderJson
    {
        public static bool SaveFile(string fileName, string fileContent)
        {
            var dir = Directory.GetCurrentDirectory();
            var fileNamePath = Path.Combine(dir,"db", fileName + ".json");
            try
            {
                File.WriteAllText(fileNamePath, fileContent);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static void SerialiseAndSave<T>(T objectToSerialize, string fileName)
        {
            var dir = Directory.GetCurrentDirectory();
            var fileNamePath = Path.Combine(dir, "JsonLib", fileName + ".json");
            File.WriteAllText(fileNamePath, JsonConvert.SerializeObject(objectToSerialize));
        }
        //Returns a string containing the entirety of a Json file.

        /// <summary>
        /// Provide 1 string to read all json file content. Provide 2 strings ( filename + folder name) to read from a specific folder.
        /// </summary>
        /// <returns>String with content of file</returns>
        public static string ReadFile(string fileName)
        {
            var dir = Directory.GetCurrentDirectory();
            var fileNamePath = Path.Combine(dir, "db", fileName + ".json");
            return File.ReadAllText(fileNamePath);
        } 
        /// <summary>
        /// Deserializes given json filename to provided Type. 
        /// Invoke: JsonFile.DeserializeFile<Player>("\\Stories\\ExampleCharacterName");
        /// </summary>
        /// <typeparam name="T">Class you want to deserialize to.</typeparam>
        /// <param name="fileName">Name of json file. </param>
        /// <returns>Object of class T</returns>
        public static T DeserializeFile<T>(string fileName)
        {
            bool fileExists = ReaderJson.FileExitsInDirectory(fileName);
            if (fileExists)
            {
                var textFromFile = ReaderJson.ReadFile(fileName);
                return JsonConvert.DeserializeObject<T>(textFromFile);
            }
            return default;
        }
        /// <summary>
        /// saves given json string under the given file. Provide 3rd param string yourFolderName for overload. 
        /// </summary>
        public static void SerializedToJson<T>(T objectToSerialize, string fileName)
        {
            var dir = Directory.GetCurrentDirectory();
            var fileNamePath = Path.Combine(dir, "db", fileName + ".json");
            File.WriteAllText(fileNamePath, JsonConvert.SerializeObject(objectToSerialize));
        }
        /// <summary>
        /// Checks if file exists in given directory. It assumes file is in JsonLib
        /// </summary>
        /// <returns>Bool</returns>
        public static bool FileExitsInDirectory(string fileName)
        {
            string dir = Directory.GetCurrentDirectory();
            var fileNamePath = Path.Combine(dir, "db", fileName + ".json");
            return File.Exists(fileNamePath);
        }

        /// <summary>
        /// Provide 1 string with the path you want to navigate to .  
        /// </summary>
        /// <param name="relationalPath"></param>
        /// <returns> a string array with all file names in given directory</returns>
        public static string[] FindAllFileNames(string relationalPath)
        {
            var dir = Directory.GetCurrentDirectory();
            string[] allFiles = Directory.GetFiles(dir + relationalPath);

            for (int i = 0; i < allFiles.Count(); i++)
            {
                allFiles[i] = Path.GetFileName(allFiles[i]).Split(".")[0];
            }

            return allFiles;
        }
    }
}
