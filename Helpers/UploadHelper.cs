namespace Assignment_CS5.Helpers
{
    public interface IUploadHelper
    {
        void UploadImage(IFormFile file, string rootPath, string category);
        void RemoveImage(string filePath);

    }
    public class UploadHelper : IUploadHelper
    {
        public void UploadImage(IFormFile file, string rootPath, string category)
        {
            //string path = Path.Combine(_hostingEnvironment.WebRootPath, "images", file.FileName);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            string dirPath = rootPath + @"\" + category;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string filePath = dirPath + @"\" + file.FileName;

            if (!File.Exists(filePath))
            {
                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
        }

        public void RemoveImage(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);

                //string path = Path.Combine(_hostingEnvironment.WebRootPath, "images", file.FileName);
                //var getFile = new FileInfo(filePath);
                //getFile.Delete();
            }
        }

    }
}
