
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
