using ACME.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACME.Helpers
{
    public class SessionHelper
    {
        IHttpContextAccessor accessor;

        public SessionHelper(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public bool isLoggedIn()
        {
            var httpContext = accessor.HttpContext;
            return httpContext.Session.GetString("Email") != null;
        }

        public UserType GetUserType()
        {
            var httpContext = accessor.HttpContext;
            ACMEDbContext context = new ACMEDbContext();
            int userTypeID = httpContext.Session.GetInt32("UserType") ?? -1;
            UserType userType = (userTypeID != -1) ? context.UserTypes.Find(userTypeID) : null;
            return userType;
        }
    }
}
