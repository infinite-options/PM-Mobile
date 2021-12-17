using System;
namespace PM.Model.Login.LoginClasses
{
    public class SocialLogInPost
    {
        public string email { get; set; }
        public string password { get; set; }
        public string social_id { get; set; }
        public string signup_platform { get; set; }
    }
}

/*
 {
    "email": "bmhuss33@gmail.com",
    "password": "",
    "social_id": "109756159098394616145",
    "signup_platform": "GOOGLE"
}
 */

//response if you have a direct login account already associated with the email
//{
//    "message": "Wrong password",
//    "code": 406,
//    "result": ""
//}