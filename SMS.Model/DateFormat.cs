using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel
{
    public static class DateFormat
    {
        public const string ShortDate = "dd/MM/yyyy";
        public const string LongDate = "dd/MM/yyyy hh:mm:ss tt";
        public static string ToInputDateFormat(DateTime? date)
        {
            return date?.ToString(ShortDate);
        }

        public static string ToDisplayDateFormat(DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy");
        }
    }
}
