using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Scholarship_Application.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarship_Application.Controllers
{
    public class StudentController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        public StudentController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user , IFormFile file)
        {
            var userResult = await userManager.FindByIdAsync(user.Id);
            userResult.FirstName = user.FirstName; 
            userResult.LastName = user.LastName;
            userResult.BirthDate = user.BirthDate;
            userResult.NationalID = user.NationalID;
            userResult.University = user.University;
            userResult.Major = user.Major;
            userResult.GPA = user.GPA;
          


            var result = await userManager.UpdateAsync(userResult);


            if (  !(file == null || file.Length == 0) )
            {
                string extinsion = file.ContentType.Split("/")[1];



                string FileName = $"{userResult.Resume}";


                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/Resume", FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
              
           


            return RedirectToAction("UpdateSuccess");

        }

        public IActionResult UpdateSuccess()
        {
            return View();
        }
        public async Task<IActionResult> Download(string filename, string name)
        {
            try
            {


                if (filename == null)
                    return Content("filename not present");

                var path = Path.Combine(
                               Directory.GetCurrentDirectory(),
                               "wwwroot/Resume", filename);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), $"{name}{Path.GetExtension(path).ToLowerInvariant()}");
            }
            catch 
            {
                var path = Path.Combine(
                             Directory.GetCurrentDirectory(),
                             "wwwroot/Resume", "notfound.png");
                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), $"notfound.png");
            }
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        public async Task UploadFile(IFormFile file , string cv)
        {
            if (file == null || file.Length == 0)
                return ;

            string extinsion = file.ContentType.Split("/")[1];

            

            string FileName = $"{cv}";          


            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Resume", FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

           
        }
    }
}
