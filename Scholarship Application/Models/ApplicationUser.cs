using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarship_Application.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string NationalID { get; set; }
        public string University { get; set; }
        public string Major { get; set; }
        public string GPA { get; set; }

        public string Resume { get; set; }

        public Status Status { get; set; }


    }
}
