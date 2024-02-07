namespace ResumeApplication.Models
{
    public class RegisterModel
    {
        public string username { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string confirmpassword { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }
        public string returnURL { get; set; }
    }
}