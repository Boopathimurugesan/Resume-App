using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ResumeApplication.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]

        public string password { get; set; }
        //public string returnURL { get; set; }
    }
}