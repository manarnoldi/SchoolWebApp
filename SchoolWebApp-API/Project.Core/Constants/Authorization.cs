using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Constants
{
    public class Authorization
    {
        public enum Roles
        {
            Administrator,
            HeadTeacher,
            Teacher,
            Student,
            Parent,
            Accounts,
            Visitor
        }
        public const string default_username = "admin";
        public const string default_email = "admin@kodetek.co.ke";
        public const string default_password = "Admin@123";
        public const Roles default_role = Roles.Administrator;
    }
}
