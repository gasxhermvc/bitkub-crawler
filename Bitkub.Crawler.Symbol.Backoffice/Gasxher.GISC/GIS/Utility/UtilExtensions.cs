using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Utility
{
    public static class UtilExtensions
    {
        public static DateTime StringToDateTime(string date)
        {
            string[] format = new string[] {
                "dd/MM/yyyy HH:mm:ss",
                "d/M/yyyy H:m:s",
                "dd/MM/yyyy HH:mm",
                "d/M/yyyy H:m",
                "dd/MM/yyyy",
                "d/M/yyyy",
                "HH:mm:ss",
                "HH:mm",
            };
            foreach (string f in format)
            {
                try
                {
                    return StringToDateTime(date, f);
                }
                catch { }
            }
            throw new Exception(string.Format("ไม่สามารถแปลง {0} เป็น DateTime ได้", date));
        }

        public static DateTime StringToDateTime(string date, string format)
        {
            try
            {
                return DateTime.ParseExact(date, format, new System.Globalization.CultureInfo("th-TH"));
            }
            catch
            {
                throw new Exception(string.Format("ไม่สามารถแปลง {0} เป็น DateTime ได้", date));
            }
        }

        public static string DateTimeToString(DateTime? date)
        {
            if (date != null && date.HasValue)
            {
                return DateTimeToString(date.Value);
            }
            return null;
        }

        public static string DateTimeToString(DateTime? date, string format)
        {
            if (date != null && date.HasValue)
            {
                return DateTimeToString(date.Value, format);
            }
            return null;
        }

        public static string DateTimeToString(DateTime? date, string format, System.Globalization.CultureInfo cultureInfo)
        {
            if (date != null && date.HasValue)
            {
                return DateTimeToString(date.Value, format, cultureInfo);
            }
            return null;
        }

        public static string DateTimeToString(DateTime date)
        {
            return date.ToString("dd/MM/yyyy HH:mm:ss", new System.Globalization.CultureInfo("th-TH"));
        }

        public static string DateTimeToString(DateTime date, string format)
        {
            return DateTimeToString(date, format, new System.Globalization.CultureInfo("th-TH"));
        }

        public static string DateTimeToString(DateTime date, string format, System.Globalization.CultureInfo cultureInfo)
        {
            return date.ToString(format, cultureInfo);
        }

        public static long DateTimeToUnixTimeStamp(DateTime date, double timezone = -420)
        {
            return long.Parse((date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMinutes(timezone * -1)).TotalMilliseconds.ToString("###0"));
        }

        public static DateTime UnixTimeStampToDateTime(long milliseconds, double timezone = -420)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds).AddMinutes(timezone * -1);
        }

        public static DateTime UnixTimeStampToDateTime(int seconds, double timezone = -420)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).AddMinutes(timezone * -1);
        }

        public static List<Dictionary<string, object>> DataTableToDictionary(DataTable dt)
        {
            return DataTableToDictionary(dt, "dd/MM/yyyy HH:mm:ss");
        }

        public static List<Dictionary<string, object>> DataTableToDictionary(DataTable dt, string format)
        {
            return DataTableToDictionary(dt, format, new System.Globalization.CultureInfo("th-TH"));
        }

        public static List<Dictionary<string, object>> DataTableToDictionary(DataTable dt, System.Globalization.CultureInfo cultureInfo)
        {
            return DataTableToDictionary(dt, "dd/MM/yyyy HH:mm:ss", cultureInfo);
        }

        public static List<Dictionary<string, object>> DataTableToDictionary(DataTable dt, string format, System.Globalization.CultureInfo cultureInfo)
        {
            List<Dictionary<string, object>> dataDict = null;
            try
            {
                dataDict = dt.AsEnumerable().Select(dr => dt.Columns.Cast<DataColumn>().ToDictionary(
                dc => dc.ColumnName,
                dc => dr[dc] is DateTime
                    ? DateTimeToString(dr[dc] as DateTime?, format, cultureInfo)
                    : dr[dc]
                    )).ToList();
                return dataDict;
            }
            catch (Exception ex)
            {
                throw new Exception("ไม่สามารถแปลงจาก DataTable เป็น Dictionary ได้ เนื่องจาก " + ex.Message);
            }
        }

        public static Type DBTypeMap(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Binary:
                    return typeof(byte[]);

                case DbType.Boolean:
                    return typeof(bool);

                case DbType.Byte:
                    return typeof(byte);

                case DbType.Currency:
                case DbType.Decimal:
                case DbType.VarNumeric:
                    return typeof(decimal);

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.Time:
                    //case DbType.DateTimeOffset:
                    return typeof(DateTime);

                case DbType.DateTimeOffset:
                    return typeof(DateTimeOffset);

                case DbType.Double:
                    return typeof(double);

                case DbType.Guid:
                    return typeof(Guid);

                case DbType.Int16:
                    return typeof(short);

                case DbType.Int32:
                    return typeof(int);

                case DbType.Int64:
                    return typeof(long);

                case DbType.Object:
                case DbType.Xml:
                    return typeof(object);

                case DbType.SByte:
                    return typeof(sbyte);

                case DbType.Single:
                    return typeof(float);

                case DbType.UInt16:
                    return typeof(ushort);

                case DbType.UInt32:
                    return typeof(uint);

                case DbType.UInt64:
                    return typeof(ulong);

                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.AnsiStringFixedLength:
                case DbType.AnsiString:
                default:
                    return typeof(string);
            }
        }


    }
}
