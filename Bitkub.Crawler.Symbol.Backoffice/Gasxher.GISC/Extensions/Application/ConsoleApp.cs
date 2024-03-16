using GasxherGIS.Application;
using GasxherGIS.Application.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GasxherGIS.Extensions.Application
{
    public static class ConsoleApp
    {
        /// <summary>
        /// สร้าง Instance ใหม่ของ Class <see cref="ConsoleBuilder"/> ด้วยค่าเริ่มต้นที่กำหนดไว้ล่วงหน้า
        /// </summary>
        /// <returns></returns>
        public static IConsoleBuilder CreateDefaultBuilder() => CreateDefaultBuilder(args: null);


        public static IConsoleBuilder CreateDefaultBuilder(string[] args)
        {
            SetDateEnv();
            var builder = new ConsoleBuilder();
            var container = builder.CreateAppContainer();
            builder.SetContainer(container);

            return builder;
        }

        public static void SetDateEnv()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo info = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            info.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:ss";
            info.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = info;
        }
    }
}
