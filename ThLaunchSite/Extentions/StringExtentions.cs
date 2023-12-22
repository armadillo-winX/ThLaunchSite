using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThLaunchSite.Extentions
{
    internal static class StringExtentions
    {
        public static string RemoveRightOf(this string text, string removeLetter)
        {
            int length = text.IndexOf(removeLetter);
            if (length < 0)
            {
                return text;
            }

            return text.Substring(0, length);
        }
    }
}
