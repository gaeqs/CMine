using System.IO;

namespace CMineNew.Util{
    public static class FolderUtils{

        public static void CreateRegionFolderIfNotExist(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }
        
    }
}