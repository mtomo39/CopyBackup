using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CopyBackup
{
    
    class Program
    {
        static readonly string DateTimeformat = "yyyy-MM-dd_HHmmss";

        /// <summary>
        /// パラメータのファイルパス、または、フォルダパスに対しては、バックアップフォルダにコピーを作成する
        /// </summary>
        /// <param name="args">ファイルパス、または、フォルダパス</param>
        static void Main(string[] args)
        {
            var hasError = false;

            var fileInfos = new List<FileInfo>();

            // パラメータのうち、ファイルパス
            var targetFiles = args.Where(x => File.Exists(x));

            fileInfos.AddRange(targetFiles.Select(x => new FileInfo(x)));

            // パラメータのうち、フォルダパス
            var targetFolders = args.Where(x => Directory.Exists(x));

            foreach (var folder in targetFolders)
            {
                fileInfos.AddRange(new DirectoryInfo(folder).GetFiles("*", SearchOption.AllDirectories));
            }

            // パラメータにファイルパスもフォルダパスの1つもない場合は処理しない
            if (fileInfos.Count == 0)
            {
                return;
            }


            // バックアップフォルダを作る
            var timeStamp = string.Format("{0:" + Program.DateTimeformat + " }", fileInfos.OrderByDescending(x => x.LastWriteTime).Select(x => x.LastWriteTime).FirstOrDefault());

            var targets = targetFiles.Count() == 0 ? targetFolders : targetFiles;
            var parent = Directory.GetParent(targets.First()).FullName;
            var destination = Path.Combine(Path.Combine(parent, "OLD"), timeStamp);

            Directory.CreateDirectory(destination);

            // コピー
            try
            {
                foreach (var file in targetFiles)
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(file, Path.Combine(destination, Path.GetFileName(file)), overwrite: true);
                }

                foreach (var folder in targetFolders)
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(folder, Path.Combine(destination, new DirectoryInfo(folder).Name), overwrite: true);
                }
            }catch(Exception ex)
            {
                hasError = true;
                Console.WriteLine(ex.ToString());
            }

            if (hasError)
            {
                Console.ReadKey();
            }

        }
    }
}
