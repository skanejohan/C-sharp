using System.IO;

namespace Theseus.Extensions
{
    public static class DirectoryExtensions
    {
        public static void CreateOrClear(this DirectoryInfo directoryInfo, string path)
        {
            if (Directory.Exists(directoryInfo.FullName))
            {
                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (var subDirectory in directoryInfo.GetDirectories())
                {
                    subDirectory.Delete(true);
                }
            }
            else
            {
                Directory.CreateDirectory(directoryInfo.FullName);
            }
        }
    }
}
