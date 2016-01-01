using System;
using System.Data;
using System.Configuration;
using System.Web;

using System.Globalization;

/// <summary>
/// Summary description for DateConversion
/// </summary>
public class DateConversion
{
    public static string GD2JD(DateTime Gregorian)
    {
        PersianCalendar pc = new PersianCalendar();
        int y, m, d;
        y = pc.GetYear(Gregorian);
        m = pc.GetMonth(Gregorian);
        d = pc.GetDayOfMonth(Gregorian);
        string ans = string.Format("{0}/{1:d2}/{2:d2}", y, m, d);
        return ans;
    }

    public static string GD2JD(DateTime Gregorian, bool H)
    {
        PersianCalendar pc = new PersianCalendar();
        int y, m, d, h, M;
        y = pc.GetYear(Gregorian);
        m = pc.GetMonth(Gregorian);
        d = pc.GetDayOfMonth(Gregorian);
        h = pc.GetHour(Gregorian);

        M = pc.GetMinute(Gregorian);

        string ans = string.Format("{0}/{1:d2}/{2:d2} {3:d2}:{4:d2}", y, m, d, h, M);
        return ans;
    }

    public static DateTime JD2GD(string Jalali)
    {
        try
        {
            int y, m, d;
            y = int.Parse(Jalali.Substring(0, 4));
            m = int.Parse(Jalali.Substring(5, 2));
            d = int.Parse(Jalali.Substring(8, 2));
            PersianCalendar pc = new PersianCalendar();
            DateTime ans = new DateTime(y, m, d, pc);
            return ans;
        }
        catch
        {
            return DateTime.Now.AddYears(-100);
        }

    }


    public static string  H2G2H(DateTime DateConv, string Calendar, string DateLangCulture)
    {
        DateTimeFormatInfo DTFormat;
        DateLangCulture = DateLangCulture.ToLower();
        /// We can't have the hijri date writen in English. We will get a runtime error - LAITH - 11/13/2005 1:01:45 PM -

        if (Calendar == "H" && DateLangCulture.StartsWith("en-"))
        {
            DateLangCulture = "ar-sa";
        }

        /// Set the date time format to the given culture - LAITH - 11/13/2005 1:04:22 PM -
        DTFormat = new System.Globalization.CultureInfo(DateLangCulture, false).DateTimeFormat;

        /// Set the calendar property of the date time format to the given calendar - LAITH - 11/13/2005 1:04:52 PM -
        switch (Calendar)
        {
            case "H":
                DTFormat.Calendar = new System.Globalization.HijriCalendar();
                break;

            case "G":
                DTFormat.Calendar = new System.Globalization.GregorianCalendar();
                break;

            default:
                return "";
        }

        /// We format the date structure to whatever we want - LAITH - 11/13/2005 1:05:39 PM -
        DTFormat.ShortDatePattern = "dd/MM/yyyy";
        return (DateConv.Date.ToString("f", DTFormat));
    }
}
