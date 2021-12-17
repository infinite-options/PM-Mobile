﻿using System;
namespace PM.Model.Login.LoginClasses
{
    public class DirectSignUpResponse
    {
        public string message { get; set; }
        public int code { get; set; }
        //public string sql { get; set; }
        public DirectResult result { get; set; }
    }

    public class DirectResult
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string customer_uid { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string social_id { get; set; }
    }

    public class Result
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string customer_uid { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }

    public class SignUpResponse
    {
        public string message { get; set; }
        public int code { get; set; }
        //public string sql { get; set; }
        public Result result { get; set; }
    }

    public class SignUpExisted
    {
        public string message { get; set; }
        public int code { get; set; }
        //public string sql { get; set; }
        public Result2[] result { get; set; }
    }

    public class Result2
    {
        public string customer_email { get; set; }
        public string role { get; set; }
        public string customer_uid { get; set; }
    }
}
