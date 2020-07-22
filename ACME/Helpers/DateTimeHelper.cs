using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACME.Helpers
{
    public class DateTimeHelper
    {
        private static List<string> months = new List<string>
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        public static string IntToString(int monthNum)
        {
            return months[monthNum - 1];
        }

        public static int StringToInt(string monthName)
        {
            return months.IndexOf(monthName) + 1;
        }

    }
}
