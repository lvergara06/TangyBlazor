﻿using Microsoft.AspNetCore.Components.Forms;
using TangyWeb_Server.Service.IService;

namespace TangyWeb_Server.Service
{
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUpload(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public bool Deletefile(string filePath)
        {
            if(File.Exists(_webHostEnvironment.WebRootPath+filePath))
            {
                File.Delete(_webHostEnvironment.WebRootPath + filePath);
                return true;
            }
            return false;
        }

        public async Task<string> UploadFile(IBrowserFile file)
        {
            FileInfo fileInfo = new(file.Name);
            var fileName = Guid.NewGuid().ToString() + fileInfo.Extension;
            var folderDir = $"{_webHostEnvironment.WebRootPath}\\images\\product";

            if(!Directory.Exists(folderDir))
            {
                Directory.CreateDirectory(folderDir);
            }

            var filePath = Path.Combine(folderDir, fileName);
            await using FileStream fs = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream().CopyToAsync(fs);
            var fullPath = $"/images/product/{fileName}";
            return fullPath;


        }
    }
}
