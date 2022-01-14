using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MilitaryCollege.Models
{
    public class FileManagementController : Controller
    {
        private IWebHostEnvironment _appEnvironment;

        public FileManagementController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        //used to upload file, uploadPAth is the path of the file example: images/events
        //fileToDelete is a boolean if there is an old file, we delete it in function

        public string UploadFile(IFormFile file, string uploadPath, string fileToDelete = null)
        {
            string UniqueFileName = null;
            string folderPath = Path.Combine(_appEnvironment.WebRootPath, uploadPath);

            //if we have a file to upload
            if (file != null)
            {
                // get unique file name
                UniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string FilePath = Path.Combine(folderPath, UniqueFileName);
                file.CopyTo(new FileStream(FilePath, FileMode.Create));
            }

            // if we have an old file to delete
            if (!String.IsNullOrEmpty(fileToDelete))
            {
                //full fileto delete path
                var fileToDeletePath = Path.Combine(folderPath, fileToDelete);
                //if file exists
                if (System.IO.File.Exists(fileToDeletePath))
                {
                    try
                    {
                        //clear unused memory
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(_appEnvironment.WebRootPath + "/" + uploadPath + "/" + fileToDelete);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return UniqueFileName;
        }


        // if we need to delete a poco record have image , we need to delete image related   
        public void DeleteFile(string deletepath, string fileToDelete = null)
        {
            string folderPath = Path.Combine(_appEnvironment.WebRootPath, deletepath);

            // if filetodelete  no null
            if (!String.IsNullOrEmpty(fileToDelete))
            {
                //full fileto delete path
                var fileToDeletePath = Path.Combine(folderPath, fileToDelete);
                //if file exists
                if (System.IO.File.Exists(fileToDeletePath))
                {
                    try
                    {
                        //clear unused memory
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.IO.File.Delete(_appEnvironment.WebRootPath + "/" + deletepath + "/" + fileToDelete);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }


        //validate file extension based on fileTYpe (document or image...)
        public string validateFileExtension(IFormFile file, string fileType)
        {
            var fileExt = Path.GetExtension(file.FileName).Substring(1).ToLower();
            //default file extensions to documents
            string[] supportedFileExtensions = FileExtensions.DocumentsExtensions; ;

            if (fileType == FileType.Document)
            {
                supportedFileExtensions = FileExtensions.DocumentsExtensions;
            }
            else if (fileType == FileType.Image)
            {
                supportedFileExtensions = FileExtensions.ImagesExtensions;
            }

            //if file extension belongs to list of supported file extensions
            if (!supportedFileExtensions.Contains(fileExt))
            {
                return FileErrorMessage.InvalidFileExtension;
            }
            return null;
        }

        //validate file size, default 5 Mbs
        public string validateFileSize(IFormFile file, int AllowedSizeInMbs = 5)
        {
            //validate size
            if (file.Length > (AllowedSizeInMbs * 1024 * 1024))
            {
                return FileErrorMessage.InvalidFileSize;
            }
            return null;
        }

        public bool ValidateFileUploadExtensionAndSize(ModelStateDictionary modelState, IFormFile file, string FileType, int fileSize = 5)
        {
            bool fileUploadError = false;
            //validate file extension
            string uploadExtensionErrorMessage = this.validateFileExtension(file, FileType);
            //if file extension error
            if (!String.IsNullOrEmpty(uploadExtensionErrorMessage))
            {
                modelState.AddModelError("", uploadExtensionErrorMessage);
                fileUploadError = true;
            }
            //validate file size
            string uploadSizeErrorMessage = this.validateFileSize(file, fileSize);
            //if file size error
            if (!String.IsNullOrEmpty(uploadSizeErrorMessage))
            {
                modelState.AddModelError("", uploadSizeErrorMessage);
                fileUploadError = true;
            }
            return fileUploadError;
        }
    }

    public static class FileExtensions
    {
        public static string[] DocumentsExtensions = new[] { "txt", "doc", "docx", "pdf", "xls", "xlsx" };
        public static string[] ImagesExtensions = new[] { "jpeg", "jpg", "png" };
    }

    public static class FileType
    {
        public const string Document = "Document";
        public const string Image = "Image";
    }
    public static class FileErrorMessage
    {
        public const string InvalidFileExtension = "File Extension Not Supported";
        public const string InvalidFileSize = "File Size Exceeds Allowed Limit";
    }
}
