using ExtCopy.common;
using ExtCopy.data;
using ExtCopy.io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExtCopy.util
{
    class CheckUtil
    {
        /// <summary>
        /// 出力クラス
        /// </summary>
        private Output output;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="o"></param>
        public CheckUtil(Output o)
        {
            output = o;
        }

        /// <summary>
        /// 引数が正しいかチェックします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public InputInfo CheckArgs(string[] args)
        {
            InputInfo inputDirs = new InputInfo();

            // 引数が2つ未満のときエラー
            if (args.Count() < 2)
            {
                if (0 < args.Count())
                {
                    if (IsHelp(args[0]))
                    {
                        output.Help();
                    }
                    else if (IsVersion(args[0]))
                    {
                        output.Version();
                    }
                }
                throw new common.ApplicationException(Const.ERR_USAGE);
            }

            // 抽出元ディレクトリ
            string inputDir = args[0];
            // 抽出先ディレクトリ
            string outputDir = args[1];

            // 抽出元ディレクトリの絶対パスを取得
            string inputAbsolutePath = string.Empty;
            try
            {
                // 絶対パスの場合はそのままセット
                if (Path.IsPathRooted(inputDir))
                {
                    inputAbsolutePath = inputDir;
                }
                // 相対パスの場合は絶対パスを取得しセット
                else
                {
                    inputAbsolutePath = (new Uri(new Uri(GetCurrentDirSlash()), inputDir)).AbsolutePath;
                }
            }
            catch (PathTooLongException)
            {
                throw new common.ApplicationException("エラー : 対象ディレクトリのパスが長すぎます。");
            }
            catch (Exception)
            {
                throw new common.ApplicationException("エラー : 対象ディレクトリパスに誤りがあります。");
            }

            // 抽出元ディレクトリが存在しないときエラー
            if (!Directory.Exists(inputAbsolutePath))
            {
                throw new common.ApplicationException("エラー : 抽出元ディレクトリが存在しません。");
            }
            // 抽出先ディレクトリの絶対パスを取得
            string outputAbsolutePath = string.Empty;
            try
            {
                // 絶対パスの場合はそのままセット
                if (Path.IsPathRooted(outputDir))
                {
                    outputAbsolutePath = outputDir;
                }
                // 相対パスの場合は絶対パスを取得しセット
                else
                {
                    outputAbsolutePath = (new Uri(new Uri(GetCurrentDirSlash()), outputDir)).AbsolutePath;
                }
            }
            catch (PathTooLongException)
            {
                throw new common.ApplicationException("エラー : 抽出先ディレクトリのパスが長すぎます。");
            }
            catch (Exception)
            {
                throw new common.ApplicationException("エラー : 抽出先ディレクトリパスに誤りがあります。");
            }

            // 抽出元ディレクトリ情報を作成
            try
            {
                inputDirs.InputDir = new DirectoryInfo(inputAbsolutePath);
            }
            catch (Exception e)
            {
                throw new common.ApplicationException("抽出元ディレクトリの取得に失敗しました。");
            }

            // 抽出先ディレクトリ情報を作成
            inputDirs.OutputDir = new DirectoryInfo(outputAbsolutePath);

            // 検索条件を取得
            inputDirs = GetSearchCriteria(inputDirs, args);

            return inputDirs;
        }

        /// <summary>
        /// 検索条件をチェックし、格納
        /// </summary>
        /// <param name="info"></param>
        /// <param name="commands"></param>
        InputInfo GetSearchCriteria(InputInfo info, string[] commands)
        {
            // 初期化
            info.InclusionFileName = new List<string>();
            info.ExcludeFilename = new List<string>(); ;
            info.OverWrite = false;
            // 検索条件
            string nowArgsType = string.Empty;
            // 検索条件指定時
            for (int i = 2; i < commands.Count(); i++)
            {
                // 文字列を取得
                string arg = commands[i];

                // 引数の種類の場合
                if (Const.ARGUMENT_TYPE_LIST.Contains(arg.ToLower()))
                {
                    switch (arg.ToLower())
                    {
                        case "/ow":
                            info.OverWrite = true;
                            nowArgsType = string.Empty;
                            break;
                        case "/o":
                            info.OutputDetail = true;
                            nowArgsType = string.Empty;
                            break;
                        default:
                            nowArgsType = arg;
                            break;
                    }
                }
                // 通常の引数の場合
                else
                {
                    // 引数タイプによって分岐
                    switch (nowArgsType)
                    {
                        // 包含ファイル名
                        case "/fi":
                            info.InclusionFileName.Add(arg);
                            break;
                        // 除外ファイル名
                        case "/fe":
                            info.ExcludeFilename.Add(arg);
                            break;
                        // 更新日付（開始）
                        case "/ds":
                            arg = ConvertStr2DateTimeFormat(arg);
                            try
                            {
                                DateTime.Parse(arg);
                                info.StartDateTime = arg;
                            }
                            catch (Exception e)
                            {
                                throw new common.ApplicationException("更新日付（開始）に誤りがあります。");
                            }
                            nowArgsType = string.Empty;
                            break;
                        // 更新日付（終了）
                        case "/de":
                            arg = ConvertStr2DateTimeFormat(arg);
                            try
                            {
                                DateTime.Parse(arg);
                                info.EndDateTime = arg;
                            }
                            catch (Exception e)
                            {
                                throw new common.ApplicationException("更新日付（開始）に誤りがあります。");
                            }
                            nowArgsType = string.Empty;
                            break;
                        // 最小行数
                        case "/sl":
                            try
                            {
                                int.Parse(arg);
                                info.MinStep = arg;
                            }
                            catch (Exception e)
                            {
                                throw new common.ApplicationException("最小行数に誤りがありますす。");
                            }
                            nowArgsType = string.Empty;
                            break;
                        // 最大行数
                        case "/sh":
                            try
                            {
                                int.Parse(arg);
                                info.MaxStep = arg;
                            }
                            catch (Exception e)
                            {
                                throw new common.ApplicationException("最大行数に誤りがあります。");
                            }
                            nowArgsType = string.Empty;
                            break;
                        // 合計最大行数
                        case "/mh":
                            // 引数チェック用正規表現
                            Regex regex = new Regex(@"\d+([kmg]?|\.\d+[kmg])");
                            arg = arg.ToLower();
                            // 引数の形式が正しい場合
                            if (regex.IsMatch(arg))
                            {
                                int num = 0;
                                // kmgが入っている場合、numを変更
                                switch (arg[arg.Length - 1])
                                {
                                    case 'k':
                                        num = 3;
                                        break;
                                    case 'm':
                                        num = 6;
                                        break;
                                    case 'g':
                                        num = 9;
                                        break;
                                }

                                // kmgが入っている場合、削除
                                if (num != 0)
                                {
                                    arg = arg.Substring(0, arg.Length - 1);
                                }

                                // まずdoubleに変換
                                double stepNum = double.Parse(arg);
                                // kmg分の計算をする
                                for (int j = 0; j < num; j++)
                                {
                                    stepNum *= 10;
                                }

                                // 最終的に小数の場合、エラー
                                long temp = 0;
                                if (!long.TryParse(stepNum.ToString(), out temp))
                                {
                                    throw new common.ApplicationException("合計最大行数には小数は指定できません。");
                                }

                                info.TotalMaxStep = stepNum.ToString();
                            }
                            else
                            {
                                throw new common.ApplicationException("合計最大行数に誤りがあります。");
                            }
                            nowArgsType = string.Empty;
                            break;
                        case "/dev":
                            try
                            {
                                info.DevideNum = int.Parse(arg);
                                if (info.DevideNum < 1)
                                {
                                    throw new common.ApplicationException("コピー先ディレクトリ分割数に誤りがあります。");
                                }
                            }
                            catch (Exception e)
                            {
                                throw new common.ApplicationException("コピー先ディレクトリ分割数に誤りがあります。");
                            }
                            nowArgsType = string.Empty;
                            break;
                        case "/c":
                            switch (arg)
                            {
                                case "1":
                                    info.CountType = CountType.COPY_ONLY;
                                    break;
                                case "2":
                                    info.CountType = CountType.CHECK_AUTHOR;
                                    break;
                                default:
                                    throw new common.ApplicationException("カウントタイプに誤りがあります。");
                            }
                            nowArgsType = string.Empty;
                            break;
                        // その他
                        default:
                            throw new common.ApplicationException("引数に誤りがあります。");
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// 文字列を日付形式に変換できる形に変更します。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        string ConvertStr2DateTimeFormat(string target)
        {
            Regex regex = new Regex("\\d{" + target.Length + "}");
            if (!target.Contains("/") && regex.IsMatch(target))
            {
                switch (target.Length)
                {
                    case 6:
                        target = string.Format("{0}/0{1}/0{2}", target.Substring(0, 4), target[4].ToString(), target[5].ToString());
                        break;
                    case 7:
                        string tar = target.Substring(4, 3);
                        int first = int.Parse(tar[0].ToString());
                        if ((2 <= first && first <= 9) || tar[1] == '3' || tar[2] == '0')
                        {
                            target = string.Format("{0}/0{1}/{2}", target.Substring(0, 4), target[4].ToString(), target.Substring(5, 2));
                        }
                        else if (tar[1] == '0')
                        {
                            target = string.Format("{0}/{1}/0{2}", target.Substring(0, 4), target.Substring(4, 2), target[6].ToString());
                        }
                        break;
                    case 8:
                        target = string.Format("{0}/{1}/{2}", target.Substring(0, 4), target.Substring(4, 2), target.Substring(6, 2));
                        break;
                }
            }

            return target;
        }

        /// <summary>
        /// ヘルプか判定します。
        /// </summary>
        /// <returns></returns>
        private bool IsHelp(string target)
        {
            target = target.ToLower();
            if (target == "--help" || target == "-help" || target == "/h")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// バージョンか判定します。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool IsVersion(string target)
        {
            target = target.ToLower();
            if (target == "--version" || target == "-version")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 末尾に区切り文字付きのカレントディレクトリパス取得
        /// </summary>
        /// <returns></returns>
        private string GetCurrentDirSlash()
        {
            return System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar;
        }
    }
}
