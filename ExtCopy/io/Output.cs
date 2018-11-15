using ExtCopy.common;
using ExtCopy.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExtCopy.io
{
    /// <summary>
    /// 出力クラス
    /// </summary>
    public class Output
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Output()
        {
        }

        /// <summary>
        /// コンソールに出力(改行あり)
        /// </summary>
        /// <param name="content"></param>
        public void Println(string content)
        {
            Console.WriteLine(content);
        }

        /// <summary>
        /// コンソールに出力(改行なし)
        /// </summary>
        /// <param name="content"></param>
        public void Print(string content)
        {
            Console.Write(content);
        }

        /// <summary>
        /// ヘルプを出力します。
        /// </summary>
        public void Help()
        {
            Println("extcopy コピー元ディレクトリパス コピー先ディレクトリパス");
            foreach (string arg in Const.ARGUMENT_TYPE_LIST)
            {
                if (Const.ARGUMENT_OVERVIEW.ContainsKey(arg) && Const.ARGUMENT_DESCRIPTION.ContainsKey(arg))
                {
                    Println("\t" + arg + " " + Const.ARGUMENT_OVERVIEW[arg]);
                    Println("\t\t" + Const.ARGUMENT_DESCRIPTION[arg]);
                }
            }
        }

        /// <summary>
        /// バージョンを出力します。
        /// </summary>
        public void Version()
        {
            Println("version : " + Const.VERSION);
        }

        /// <summary>
        /// ファイルをコピーします。
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="info"></param>
        public void Copy(List<ExtraFileInfo> fileList, InputInfo info)
        {
            string inputPath = info.InputDir.FullName;
            string outputPath = info.OutputDir.FullName;
            bool allNo = false;
            int copyCnt = 0;
            long devideCnt = 0;
            int devideNum = 1;
            long devideStep = 0;
            string outputTempPath;
            List<ExtraFileInfo> tempFileList = new List<ExtraFileInfo>();

            if (info.DevideNum >= 2)
            {
                DirectoryInfo tempDir = new DirectoryInfo(outputPath);
                outputTempPath = GetPathWithEndSeparator(Path.Combine(tempDir.Parent.FullName, tempDir.Name + "_" + devideNum));
                devideStep = info.TotalStep / info.DevideNum;
            }
            else
            {
                outputTempPath = GetPathWithEndSeparator(outputPath);
            }

            foreach (ExtraFileInfo fileInfo in fileList)
            {
                // 相対パスを作成
                string relative = (new Uri(inputPath)).MakeRelativeUri(new Uri(fileInfo.FileInfo.FullName)).ToString();
                // 抽出先パスを作成
                string newPath = (new Uri(new Uri(outputTempPath), relative)).AbsolutePath;
                DirectoryInfo outputDir = new DirectoryInfo(new FileInfo(newPath).DirectoryName);
                // ディレクトリが存在しない場合、生成
                if (!outputDir.Exists)
                {
                    outputDir.Create();
                    File.Copy(fileInfo.FileInfo.FullName, newPath);
                    copyCnt++;
                    devideCnt += fileInfo.Step;
                }
                // 上書きモードの場合、強制上書きコピー
                else if (info.OverWrite)
                {
                    outputDir.Create();
                    File.Copy(fileInfo.FileInfo.FullName, newPath, true);
                    copyCnt++;
                    devideCnt += fileInfo.Step;
                }
                else
                {
                    // ファイル既に存在しているかチェック
                    FileInfo oldFile = (outputDir.GetFiles("*", SearchOption.TopDirectoryOnly).ToList()).Find(f => f.Name == fileInfo.FileInfo.Name);
                    // 存在する場合
                    if (oldFile != null)
                    {
                        if (allNo)
                        {
                            continue;
                        }
                        Println("同名のファイルが既に存在しています。");
                        Println("抽出元ファイル : " + Path.Combine(fileInfo.FileInfo.DirectoryName, fileInfo.FileInfo.Name));
                        Println("抽出先ファイル : " + Path.Combine(newPath, fileInfo.FileInfo.Name));
                        while (true)
                        {
                            Print("上書きしますか？( ally / alln / y / n )");
                            string answer = Console.ReadLine().ToLower();
                            switch (answer)
                            {
                                case "ally":
                                    File.Copy(fileInfo.FileInfo.FullName, newPath, true);
                                    copyCnt++;
                                    devideCnt += fileInfo.Step;
                                    info.OverWrite = true;
                                    goto OUT;
                                case "alln":
                                    allNo = true;
                                    goto OUT;
                                case "y":
                                    File.Copy(fileInfo.FileInfo.FullName, newPath, true);
                                    copyCnt++;
                                    devideCnt += fileInfo.Step;
                                    goto OUT;
                                case "n":
                                    goto OUT;
                            }
                        }
                        OUT:;
                    }
                    // 存在しない場合、単純コピー
                    else
                    {
                        File.Copy(fileInfo.FileInfo.FullName, newPath);
                        copyCnt++;
                        devideCnt += fileInfo.Step;
                    }
                }

                if (info.DevideNum >= 2)
                {
                    tempFileList.Add(fileInfo);
                }

                if (devideStep != 0 && devideStep <= devideCnt)
                {
                    if (info.CountType != CountType.COPY_ONLY)
                    {
                        InputInfo tempInfo = info.Clone();
                        tempInfo.OutputDir = new DirectoryInfo(outputTempPath);
                        Output2Txt(tempFileList, tempInfo);
                    }

                    devideNum++;
                    DirectoryInfo tempDir = new DirectoryInfo(outputPath);
                    outputTempPath = GetPathWithEndSeparator(Path.Combine(tempDir.Parent.FullName, tempDir.Name + "_" + devideNum));
                    devideCnt = 0;
                }
            }

            if (tempFileList.Count != 0)
            {
                InputInfo tempInfo = info.Clone();
                tempInfo.OutputDir = new DirectoryInfo(outputTempPath);
                Output2Txt(tempFileList, tempInfo);
            }

            Println(copyCnt + "個のファイルをコピーしました。");
            if (info.TotalStep != 0)
            {
                Println("合計行数 : " + info.TotalStep);
            }
        }

        /// <summary>
        /// ファイル情報をテキストファイルに出力します。
        /// </summary>
        /// <param name="extraFileList"></param>
        /// <param name="info"></param>
        public void Output2Txt(List<ExtraFileInfo> extraFileList, InputInfo info)
        {
            // 結果表示時のインデント整理用
            int maxDirNameSize = 0;
            int maxFileNameSize = 0;
            int maxStepSize = 4;
            long totalStep = 0;
            // 対象ファイル内の最大ファイル名、最大行数を取得
            foreach (ExtraFileInfo fileInfo in extraFileList)
            {
                int tempSize = fileInfo.FileInfo.Directory.FullName.Length;
                if (maxDirNameSize < tempSize)
                {
                    maxDirNameSize = tempSize;
                }
                tempSize = fileInfo.FileInfo.Name.Length;
                if (maxFileNameSize < tempSize)
                {
                    maxFileNameSize = tempSize;
                }
                tempSize = fileInfo.Step.ToString().Length;
                if (maxStepSize < tempSize)
                {
                    maxStepSize = tempSize;
                }
                totalStep += fileInfo.Step;
            }

            string path = System.IO.Path.Combine(info.OutputDir.FullName, Const.AUTHOR_FILE_NAME);
            string inputPath = info.InputDir.FullName;
            string outputPath = info.OutputDir.FullName;
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    string content = string.Empty;
                    writer.WriteLine(GetHeader(maxDirNameSize, maxFileNameSize, maxStepSize));
                    foreach (ExtraFileInfo fileInfo in extraFileList)
                    {
                        // 出力するデータを作成
                        int dirRightSpace = maxDirNameSize + 3 - fileInfo.FileInfo.Directory.FullName.Length;
                        int fileRightSpace = maxFileNameSize + 3 - fileInfo.FileInfo.Name.Length;
                        int sizeLeftSpace = maxStepSize - fileInfo.Step.ToString().Length;
                        content = GetSpaceString(fileInfo.FileInfo.Directory.FullName, 0, dirRightSpace) + GetSpaceString(fileInfo.FileInfo.Name, 0, fileRightSpace) + GetSpaceString(fileInfo.Step.ToString(), sizeLeftSpace, 3) + fileInfo.Author;
                        // テキストファイルへ出力
                        writer.WriteLine(content);
                    }
                }
            }
            catch (Exception e)
            {
                Println(e.Message);
            }
        }

        /// <summary>
        /// テキストに表示するヘッダーを作成
        /// </summary>
        /// <param name="maxDirNameSize"></param>
        /// <param name="maxFileNameSize"></param>
        /// <param name="maxStepSize"></param>
        /// <returns></returns>
        string GetHeader(int maxDirNameSize, int maxFileNameSize, int maxStepSize)
        {
            string headDir = "directory_path";
            string headFile = "file_name";
            string headSize = "step";
            return GetSpaceString(headDir, 0, maxDirNameSize + 3 - headDir.Length) + GetSpaceString(headFile, 0, maxFileNameSize - headFile.Length) + GetSpaceString(headSize, maxStepSize + 3 - headSize.Length, 3) + "author";
        }

        /// <summary>
        /// 文字列の指定方向に指定数空白を入れる
        /// </summary>
        /// <param name="target"></param>
        /// <param name="num"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        string GetSpaceString(string target, int left, int right)
        {
            // 空白文字列の作成
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < left; i++)
            {
                builder.Append(" ");
            }
            string spaceLeft = builder.ToString();
            builder.Clear();
            for (int i = 0; i < right; i++)
            {
                builder.Append(" ");
            }
            string spaceRight = builder.ToString();

            return spaceLeft + target + spaceRight;
        }

        /// <summary>
        /// 末尾にセパレータのついているディレクトリパスを取得します。
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        string GetPathWithEndSeparator(string dirPath)
        {
            if (!dirPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                dirPath += Path.DirectorySeparatorChar;
            }

            return dirPath;
        }
    }
}
