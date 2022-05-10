using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Common.Utilities
{
    public class GeneralUtils
    {
        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".svg", "image/svg+xml"}
            };
        }

         public static MemoryStream ExportToExcel<T>(List<T> data, string filename = null, string cols = null)
        {
            if (data?.Count == 0)
                return new MemoryStream();

            var properties = typeof(T).GetProperties();
            using var pck = new ExcelPackage();
            var workSheet = pck.Workbook.Worksheets.Add("Sheet1");
            workSheet.DefaultRowHeight = 12;
            for (var i = 1; i <= properties.Length; i++)
            {
                var property = properties[i - 1];
                if (cols != null && cols.Contains(property.Name) || cols == null)
                    workSheet.Cells[1, i].Value = property.Name;
            }

            for (var i = 1; i <= data.Count; i++)
            {
                var row = data[i - 1];
                for (var j = 0; j < properties.Length; j++)
                {
                    var property = properties[j];
                    if (cols != null && cols.Contains(property.Name) || cols == null)
                        workSheet.Cells[i + 1, j + 1].Value = property.GetValue(row);

                }
            }
            using (var range = workSheet.Cells[1, 1, 1, properties.Length])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.ReadingOrder = ExcelReadingOrder.LeftToRight;
            }

            using (var range = workSheet.Cells[2, 1, data.Count + 1, properties.Length])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.White);
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.ReadingOrder = ExcelReadingOrder.LeftToRight;
            }

            workSheet.Row(1).Height = 20;
            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
            var st = new MemoryStream();
            pck.SaveAs(st);
            return st;
        }


        public static string GetClientOs(string ua)
        {

            if (ua.Contains("Android"))
                return $"Android {GetMobileVersion(ua, "Android")}";

            if (ua.Contains("iPad"))
                return $"iPad OS {GetMobileVersion(ua, "OS")}";

            if (ua.Contains("iPhone"))
                return $"iPhone OS {GetMobileVersion(ua, "OS")}";

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return $"Windows Phone {GetMobileVersion(ua, "Windows Phone")}";

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            return (ua.Contains("Mobile") ? " Mobile " : "");
        }

        public static string GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device, StringComparison.Ordinal) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;

                if (int.TryParse(character.ToString(), out _))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }

        public static string GetBrowser(string userAgent)
        {
            if (userAgent.Contains("Edge"))
            {
                var split = userAgent.Split(new[] { "Edge" }, StringSplitOptions.RemoveEmptyEntries);
                return "Edge " + split[1];
            }
            return null;
        }
    }

}
