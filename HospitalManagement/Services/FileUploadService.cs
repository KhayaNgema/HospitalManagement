﻿using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public class FileUploadService
    {
        private readonly string _uploadsDirectory = @"C:\inetpub\UploadedFiles";

        public FileUploadService()
        {
            Directory.CreateDirectory(_uploadsDirectory);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_uploadsDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("UploadedFiles", uniqueFileName);
        }
    }
}
