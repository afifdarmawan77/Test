using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace DOT_Test.ApiModel
{
    public class User
    {
        public string Id { get; set; }

        public string Nik { get; set; }

        public string Role { get; set; }

        [Required]
        [StringLength(256)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(256)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public DateTime Dob { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Photo URL")]
        public string PhotoUrl { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Signin To Mobile")]
        public bool SigninToMobile { get; set; }

        public Position Position { get; set; }

        public int EmployeeID { get; set; }

        public bool? IsLeader { get; set; }

        public Dictionary<string, bool> Permissions { get; set; }

        public Dictionary<string, bool> MenuApp { get; set; }

        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }

        public bool IsLoginAllowed { get; set; }

        public string CompanyName { get; set; }

        public string PositionName { get; set; }
        public string LevelName { get; set; }

        public string Website { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ContractExpiredDate { get; set; }
        public string FullName { get; set; }
    }
}
