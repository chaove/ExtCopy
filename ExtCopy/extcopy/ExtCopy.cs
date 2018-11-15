using ExtCopy.data;
using ExtCopy.io;
using ExtCopy.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExtCopy.extcopy
{
    /// <summary>
    /// メインクラス
    /// </summary>
    public class ExtCopy
    {
        /// <summary>
        /// 入力情報
        /// </summary>
        private InputInfo info;

        /// <summary>
        /// 出力クラス
        /// </summary>
        private Output output;

        /// <summary>
        /// メインメソッド
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                ExtCopy o = new ExtCopy
                {
                    output = new Output()
                };
                // 引数をチェック
                CheckUtil check = new CheckUtil(o.output);
                o.info = check.CheckArgs(args);
                // ファイル抽出実行
                o.execute();
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("システムエラー発生\n" + e.Message);
            }
        }

        /// <summary>
        /// ファイル抽出を実行します。
        /// </summary>
        private void execute()
        {
            FileUtil fileUtil = new FileUtil();

            output.Println("ファイル抽出開始");
            // 抽出ファイルリスト
            List<FileInfo> targetFileList = null;
            // 引数を検索条件に、対象のファイルリストを取得
            if (info.InclusionFileName == null || info.InclusionFileName.Count() == 0)
            {
                targetFileList = fileUtil.GetFiles(info.InputDir, "*", info.ExcludeFilename);
            }
            else
            {
                targetFileList = fileUtil.GetFiles(info.InputDir, info.InclusionFileName, info.ExcludeFilename);
            }

            // 開始日が設定されている場合、対象以外を除外
            if (!string.IsNullOrEmpty(info.StartDateTime))
            {
                targetFileList.RemoveAll(f => (DateTime.Compare(f.LastWriteTime.Date, DateTime.Parse(info.StartDateTime)) < 0));
            }

            // 終了日が設定されている場合、対象以外を除外
            if (!string.IsNullOrEmpty(info.EndDateTime))
            {
                targetFileList.RemoveAll(f => (DateTime.Compare(f.LastWriteTime.Date, DateTime.Parse(info.EndDateTime)) > 0));
            }

            // 対象ファイルの拡張情報を取得
            List<ExtraFileInfo> extraFileList = MakeExtraFileInfo(targetFileList);

            if (!string.IsNullOrEmpty(info.MinStep) && !string.IsNullOrEmpty(info.MaxStep))
            {
                int minStep = int.Parse(info.MinStep);
                int maxStep = int.Parse(info.MaxStep);
                extraFileList.RemoveAll(f => f.Step < minStep || maxStep < f.Step);
            }
            else
            {
                // 最小行数による検索
                if (!string.IsNullOrEmpty(info.MinStep))
                {
                    int minStep = int.Parse(info.MinStep);
                    extraFileList.RemoveAll(f => f.Step < minStep);
                }

                // 最大行数による検索
                if (!string.IsNullOrEmpty(info.MaxStep))
                {
                    int maxStep = int.Parse(info.MaxStep);
                    extraFileList.RemoveAll(f => maxStep < f.Step);
                }
            }

            List<ExtraFileInfo> refineFileList = new List<ExtraFileInfo>();
            // 最大合計行数による絞り込み
            if (!string.IsNullOrEmpty(info.TotalMaxStep))
            {
                long tempTotalStep = 0;
                long totalMaxStep = long.Parse(info.TotalMaxStep);
                foreach (ExtraFileInfo fileInfo in extraFileList)
                {
                    tempTotalStep += fileInfo.Step;
                    if (totalMaxStep < tempTotalStep)
                    {
                        break;
                    }
                    refineFileList.Add(fileInfo);
                }
            }
            else
            {
                refineFileList = extraFileList;
            }

            // 対象ファイル数が0の場合、エラー
            if (refineFileList.Count() == 0)
            {
                throw new ApplicationException("対象ファイルが存在しません。");
            }

            // 対象ファイルの合計行数カウント
            long totalStep = 0;
            if (info.DevideNum != 0)
            {
                refineFileList.ForEach(f => totalStep += f.Step);
                info.TotalStep = totalStep;
            }

            output.Println("抽出終了　対象ファイル数：" + refineFileList.Count());
            // テキストファイルに情報を出力
            output.Println("コピー開始");
            output.Copy(refineFileList, info);
            output.Println("コピー終了");
            if (info.CountType != CountType.COPY_ONLY)
            {
                output.Println("ファイル情報出力開始");
                if (info.DevideNum <= 1)
                {
                    output.Output2Txt(refineFileList, info);
                }
                output.Println("ファイル情報出力終了");
            }
        }

        /// <summary>
        /// 拡張ファイル情報リストを作成します。
        /// </summary>
        /// <param name="infoList"></param>
        /// <returns></returns>
        private List<ExtraFileInfo> MakeExtraFileInfo(List<FileInfo> infoList)
        {
            // 結果格納用リスト
            List<ExtraFileInfo> extraInfoList = new List<ExtraFileInfo>();
            CountType countType = info.CountType;
            if (info.CountType == CountType.COPY_ONLY && info.DevideNum >= 2)
            {
                countType = CountType.COUNT_STEP;
            }

            switch (countType)
            {
                // カウントなし、単純コピー
                case CountType.COPY_ONLY:
                    foreach (FileInfo info in infoList)
                    {
                        ExtraFileInfo temp = new ExtraFileInfo();
                        temp.FileInfo = info;
                        temp.Author = string.Empty;
                        extraInfoList.Add(temp);
                    }
                    break;
                // カウントのみ
                case CountType.COUNT_STEP:
                    // 各対象ファイルに対して行数取得を行い、格納
                    foreach (FileInfo fileInfo in infoList)
                    {
                        try
                        {
                            Encoding encoding = null;
                            // ファイルの文字コードを判定
                            using (Hnx8.ReadJEnc.FileReader reader = new Hnx8.ReadJEnc.FileReader(fileInfo))
                            {
                                Hnx8.ReadJEnc.CharCode c = reader.Read(fileInfo);
                                encoding = c.GetEncoding();
                            }
                            if (encoding == null)
                            {
                                output.Println("対応されていない文字コードが利用されています。(" + fileInfo.FullName + ")");
                                continue;
                            }
                            // ファイルを読み込み、情報を取得
                            ExtraFileInfo extraInfo = GetExtraFileInfo(fileInfo, encoding);
                            extraInfoList.Add(extraInfo);
                            info.TotalStep += extraInfo.Step;
                        }
                        catch (Exception e)
                        {
                            output.Println("文字コード取得でエラーが発生しました。(" + fileInfo.Name + ")\n" + e);
                        }
                    }
                    break;
                // カウント＋開発者取得
                default:
                    // 各対象ファイルに対して作成者と行数取得を行い、格納
                    foreach (FileInfo fileInfo in infoList)
                    {
                        try
                        {
                            Encoding encoding = null;
                            // ファイルの文字コードを判定
                            using (Hnx8.ReadJEnc.FileReader reader = new Hnx8.ReadJEnc.FileReader(fileInfo))
                            {
                                Hnx8.ReadJEnc.CharCode c = reader.Read(fileInfo);
                                encoding = c.GetEncoding();
                            }
                            if (encoding == null)
                            {
                                output.Println("対応されていない文字コードが利用されています。(" + fileInfo.FullName + ")");
                                continue;
                            }
                            // ファイルを読み込み、情報を取得
                            ExtraFileInfo extraInfo = GetExtraFileInfoWithAuthor(fileInfo, encoding);
                            extraInfoList.Add(extraInfo);
                            info.TotalStep += extraInfo.Step;
                        }
                        catch (Exception e)
                        {
                            output.Println("文字コード取得でエラーが発生しました。(" + fileInfo.Name + ")\n" + e);
                        }
                    }
                    break;
            }

            return extraInfoList;
        }

        /// <summary>
        /// ファイルをバイト型で読み込む
        /// </summary>
        /// <returns></returns>
        private byte[] ReadFileByte(string filePath)
        {
            byte[] bs = null;
            //ファイルを開く
            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //ファイルを読み込むバイト型配列を作成する
                bs = new byte[fs.Length];
                //ファイルの内容をすべて読み込む
                fs.Read(bs, 0, bs.Length);
            }
            return bs;
        }

        /// <summary>
        /// ファイルの詳細情報を取得します。
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private ExtraFileInfo GetExtraFileInfo(FileInfo fileInfo, Encoding encoding)
        {
            ExtraFileInfo extraInfo = new ExtraFileInfo();

            // ファイルの読み込み
            using (StreamReader reader = new StreamReader(fileInfo.FullName, encoding))
            {
                int cnt = 0;
                while (reader.Peek() >= 0)
                {
                    reader.ReadLine();
                    cnt++;
                }

                extraInfo.FileInfo = fileInfo;
                extraInfo.Step = cnt;
            }

            return extraInfo;
        }

        /// <summary>
        /// ファイルの詳細情報を取得します。
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private ExtraFileInfo GetExtraFileInfoWithAuthor(FileInfo fileInfo, Encoding encoding)
        {
            ExtraFileInfo extraInfo = new ExtraFileInfo();

            // ファイルの読み込み
            using (StreamReader reader = new StreamReader(fileInfo.FullName, encoding))
            {
                int cnt = 0;
                string author = string.Empty;
                while (reader.Peek() >= 0)
                {
                    cnt++;
                    string line = reader.ReadLine();
                    // @authorが含まれる場合、その後の文字列を作成者に設定
                    if (string.IsNullOrEmpty(author))
                    {
                        author = GetAuthor(line, author);
                    }
                }
                if (info.OutputDetail && !string.IsNullOrEmpty(author))
                {
                    output.Println(encoding.EncodingName + " : " + author);
                }

                extraInfo.FileInfo = fileInfo;
                extraInfo.Step = cnt;
                extraInfo.Author = author;
            }

            return extraInfo;
        }

        /// <summary>
        /// ファイルの作成者を取得します。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        private string GetAuthor(string line, string author)
        {
            // 行に@authorが含まれるとき
            if (line.ToLower().Contains("@author"))
            {
                // 空白で行を分割
                string[] devideLine = line.Split(' ');
                // 既に作成者が登録されている場合は追加
                if (!string.IsNullOrEmpty(author))
                {
                    author += ",";
                }
                // @author以降を取得
                for (int i = 0; i < devideLine.Count(); i++)
                {
                    if (devideLine[i].ToLower().Contains("@author"))
                    {
                        for (int j = i + 1; j < devideLine.Count(); j++)
                        {
                            author += devideLine[j];
                        }
                    }
                }
            }

            return author;
        }
    }
}
