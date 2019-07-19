using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Markup;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

namespace UpdateTest.ViewModel
{
    class MainViewModel
    {
        public MainViewModel()
        {
            string updateFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Diablo", "update Diablo.json");
            UpdateFiles[] updateStruct = JsonConvert.DeserializeObject<UpdateFiles[]>(File.ReadAllText(updateFile));

            StringBuilder sbLog = new StringBuilder();
            string _workDir = Directory.GetCurrentDirectory();

            foreach (var fileStruct in updateStruct)
            {
                string pathOldFile = Path.Combine(_workDir, fileStruct.Name);

                if(File.Exists(pathOldFile))
                {
                    int indexVer = fileStruct.Comment.IndexOf("|");

                    string comment = string.Empty;
                    if (!string.IsNullOrEmpty(fileStruct.Comment))
                        comment = fileStruct.Comment.Remove(0, indexVer + 1);


                    if (new FileInfo(pathOldFile).Length != fileStruct.Size)
                        sbLog.Append(AddToLog(fileStruct, comment));
                    else
                    if (indexVer > 0)
                    {
                        string strNewFile = fileStruct.Comment.Substring(0, indexVer);
                        Version verNewFile = new Version(strNewFile);

                        string strOldFile = FileVersionInfo.GetVersionInfo(pathOldFile).FileVersion;
                        Version verOldFile = new Version(strOldFile);

                        if (verNewFile > verOldFile)
                            sbLog.Append(AddToLog(fileStruct, comment));
                    }
                }
            }
            UpdLog = sbLog.ToString();
        }


        private StringBuilder AddToLog(UpdateFiles fileStruct, string comment)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"<Italic>Устаревший файл:</Italic> <Bold>{fileStruct.Name}</Bold>");
            if (!string.IsNullOrEmpty(fileStruct.Comment))
                sb.AppendLine($"<Italic>Последние изменения:</Italic> {comment}");
            sb.AppendLine($"<Bold>Ссылка на новую версию файла:</Bold> {fileStruct.URL}&#x0a;&#x0a;");
            return sb;
        }


        public struct UpdateFiles
        {
            public string Name;
            public long Size;
            public string URL;
            public string Comment;

            public UpdateFiles(string name, long size, string urlPath, string comment)
            {
                Name = name;
                Size = size;
                URL = urlPath;
                Comment = comment;
            }
        }

        public string UpdLog { get; set; }

    }
}
