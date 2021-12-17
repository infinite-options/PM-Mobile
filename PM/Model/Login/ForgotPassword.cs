using System;
namespace PM.Model.Login
{
    public class ForgotPassword
    {
        public string email { get; set; }
    }

    public class UpdatePassword
    {
        public string customer_uid { get; set; }
        public string old_password { get; set; }
        public string new_password { get; set; }
    }
}