using System;
using System.Collections.Generic;

namespace PM.Model
{
    public class ClientIntakePost
    {
        public string customer_uid { get; set; }
        public string name { get; set; }
        public int last4_ss { get; set; }
        public string dob { get; set; }
        public string address { get; set; }
        //public string unit { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string home_phone { get; set; }
        public string cell_phone { get; set; }
        public List<CI_HouseholdMembers> household_members { get; set; }
        public int under_18 { get; set; }
        public int over_18 { get; set; }
        public int over_65 { get; set; }
        public string housing_status { get; set; }
        public string living_situation { get; set; }
        public string submit_date { get; set; }
    }

    public class CI_HouseholdMembers
    {
        public string name { get; set; }
        public int age { get; set; }
        public string relationship { get; set; }
    }

    public class WestValleyPost
    {
        public string customer_uid { get; set; }
        public string name { get; set; }
        public string last_permanent_zip { get; set; }
        public string last_sleep_city { get; set; }
        public string extent_homelessness { get; set; }
        public string gender { get; set; }
        public string marital_status { get; set; }
        public string education { get; set; }
        public string highest_grade_level { get; set; }
        public string hispanic_origin { get; set; }
        public string primary_ethnicity { get; set; }
        public string veteran { get; set; }
        public string long_disability { get; set; }
        public string long_disability_desc { get; set; }
        public string primary_lang { get; set; }
        public string english_fluency { get; set; }
        public string employment_status { get; set; }
        public string medical_insurance { get; set; }
        public string special_nutrition { get; set; }
        public string calfresh { get; set; }
        public double calfresh_amount { get; set; }
        public string emergency_name { get; set; }
        public string emergency_phone { get; set; }
        public string emergency_relationship { get; set; }
        public string emergency_client { get; set; }
        public List<WV_HouseholdMembers> household_members { get; set; }
        public string submit_date { get; set; }
    }

    public class WV_HouseholdMembers
    {
        public string name { get; set; }
        public int last4_ss { get; set; }
        public string relationship { get; set; }
        public string dob { get; set; }
        public int age { get; set; }
    }

    public class FormsGet
    {
        public string message { get; set; }
        public List<FormsGetResult> result { get; set; }
    }

    public class FormsGetResult
    {
        public string household_uid { get; set; }
        public string customer_uid { get; set; }
        public string name { get; set; }
        public string last4_ss { get; set; }
        public string dob { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string home_phone { get; set; }
        public string cell_phone { get; set; }
        public string household_members { get; set; } //how do I process this
        public string under_18 { get; set; }
        public string over_18 { get; set; }
        public string over_65 { get; set; }
        public string housing_status { get; set; }
        public string living_situation { get; set; }
        public string submit_date { get; set; }
        public string last_permanent_zip { get; set; }
        public string last_sleep_city { get; set; }
        public string extent_homelessness { get; set; }
        public string gender { get; set; }
        public string marital_status { get; set; }
        public string education { get; set; }
        public string highest_grade_level { get; set; }
        public string hispanic_origin { get; set; }
        public string primary_ethnicity { get; set; }
        public string veteran { get; set; }
        public string long_disability { get; set; }
        public string long_disability_desc { get; set; }
        public string primary_lang { get; set; }
        public string english_fluency { get; set; }
        public string employment_status { get; set; }
        public string medical_insurance { get; set; }
        public string special_nutrition { get; set; }
        public string calfresh { get; set; }
        public string calfresh_amount { get; set; }
        public string emergency_name { get; set; }
        public string emergency_phone { get; set; }
        public string emergency_relationship { get; set; }
        public string emergency_client { get; set; }
    }
}
