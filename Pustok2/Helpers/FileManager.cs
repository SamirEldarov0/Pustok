using Microsoft.AspNetCore.Http;
using Pustok2.Models;
using System.IO;
using System;

namespace Pustok2.Helpers
{
    public static class FileManager
    {
        
        public static string Save(string rootpath,string folder,IFormFile formFile)
        {
            string newFileName = Guid.NewGuid().ToString() +formFile.FileName;
            string path = Path.Combine(rootpath, folder, newFileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
            return newFileName;
        }
        public static bool Delete(string rootpath,string folder,string filename)
        {
            string path = Path.Combine(rootpath, folder, filename);
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
