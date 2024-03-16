using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Logging.Manager
{
    public class LogHandler
    {
        private readonly string _filePath;

        public LogHandler(string filePath)
        {
            _filePath = filePath;
        }

        private void Create()
        {
            if (!System.IO.File.Exists(_filePath)
                //=>Check File Extension && IsFile(_filePath))
                )
            {
                System.IO.Directory.CreateDirectory(
                    System.IO.Path.GetDirectoryName(_filePath));
            }

            this.WriteFileLog(_filePath);
        }


        /// <summary>
        /// เขียนไฟล์แถวเดียว
        /// </summary>
        /// <param name="lines"></param>
        public void Append(string lines) => this.Append(new string[] { lines });

        /// <summary>
        /// เขียนไฟล์หลายแถว
        /// </summary>
        /// <param name="lines"></param>
        public void Append(params string[] lines) => this.WriteLog(_filePath, lines);

        /// <summary>
        /// เขียนไฟล์หลายแถว
        /// </summary>
        /// <param name="lines"></param>
        public void Append(List<string> lines) => this.WriteLog(_filePath, lines.ToArray());


        /// <summary>
        /// สร้างไฟล์ล็อก
        /// </summary>
        /// <param name="filePath"></param>
        private void WriteFileLog(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return;
            }

            using (var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write))
            {
                using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    writer.Flush();
                }

                fileStream.Close();
            }

            return;
        }

        /// <summary>
        /// เขียน Logs
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="line"></param>
        private void WriteLog(string filePath, params string[] lines)
        {
            if (!System.IO.File.Exists(filePath))
            {
                this.Create();
            }

            using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    foreach (var line in lines)
                    {
                        writer.WriteLine(line);
                    }

                    writer.Flush();
                    writer.Close();
                }

                fileStream.Close();
            }
        }
    }
}
