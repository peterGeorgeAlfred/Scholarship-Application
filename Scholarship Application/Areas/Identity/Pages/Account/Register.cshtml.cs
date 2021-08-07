using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Scholarship_Application.Models;

namespace Scholarship_Application.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            #region FirstName
            [Required]
            [Display(Name = "Firs Name")]
            public string FirstName { get; set; }
            #endregion



            #region LastName
            [Required]
            [Display(Name = "LastName")]
            public string LastName { get; set; }
            #endregion


            #region BirthDate
            [Display(Name = "Data Of Birth")]
            public DateTime BirthDate { get; set; }
            #endregion


            #region ID
            [Required]
            [MaxLength(14, ErrorMessage = "National ID must Be 14 Numebr")]
            [MinLength(14, ErrorMessage = "National ID must Be 14 Numebr")]
            [Display(Name = "NationalID")]
            public string NationalID { get; set; }
            #endregion

            #region Universaity
            [Required]
            [Display(Name = "University")]
            public string University { get; set; }
            #endregion

            #region Major
            [Required]
            [Display(Name = "Major")]
            public string Major { get; set; }
            #endregion

            #region GPA

            [Required]
            [Display(Name = "GPA")]
            public string GPA { get; set; } 
            #endregion

            public string Resume { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file ,string returnUrl = null )
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                if (file == null || file.Length == 0)
                    return Content("file not selected");

                string extinsion = file.ContentType.Split("/")[1];

                Guid guid = new Guid();

                string FileName = $"{guid}.{extinsion}";

                Input.Resume = FileName;


                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/Resume", FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

            


                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, FirstName = Input.FirstName, LastName= Input.LastName , BirthDate = Input.BirthDate , GPA = Input.GPA , Major = Input.Major , University = Input.University, NationalID = Input.NationalID ,Resume =Input.Resume };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                      await _signInManager.SignInAsync(user, isPersistent: false);
                      await _userManager.AddToRoleAsync(user, "Student");
                      return RedirectToRoute ($"student/edit/{user.Id}");
                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            string extinsion = file.ContentType.Split("/")[1];       

            Guid guid = new Guid();

            string FileName = $"{guid}.{extinsion}";

            Input.Resume = FileName; 
            

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Resume", FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return RedirectToAction("Files");
        }

    }
}
