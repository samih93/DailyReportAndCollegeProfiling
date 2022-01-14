using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MilitaryCollege.Controllers
{
    public class GlobalController : Controller
    {
        private IWebHostEnvironment _appEnvironment;

        public GlobalController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public static bool isImage(IFormFile file)
        {
            string ext = Path.GetExtension(file.FileName).Replace(".", "").ToLower();
            List<string> imagesFileExtensions = new List<string> { "png", "jpg", "jpeg", "tif", "gif" };
            return imagesFileExtensions.Contains(ext);
        }
        public void deletefile(string folderpath, string fileToDelete = null)
        {
            string folderPath = Path.Combine(_appEnvironment.WebRootPath, folderpath);

            var fileToDeletePath = Path.Combine(folderPath, fileToDelete);
            //if file exists
            if (System.IO.File.Exists(fileToDeletePath))
            {
                try
                {
                    //clear unused memory
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(_appEnvironment.WebRootPath + "/" + folderpath + "/" + fileToDelete);
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}