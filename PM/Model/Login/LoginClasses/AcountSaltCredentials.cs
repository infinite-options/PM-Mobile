using System;
using System.Collections.Generic;

namespace PM.LogInClasses
{
    public class AccountSalt
    {
        public string password_algorithm { get; set; }
        public string password_salt { get; set; }
        public string user_social_media { get; set; }
    }

    public class AcountSaltCredentials
    {
        public string message { get; set; }
        public int code { get; set; }
        public AccountSalt[] result { get; set; }
    }
}
