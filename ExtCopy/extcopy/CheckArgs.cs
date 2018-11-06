using ExtCopy.common;
using ExtCopy.data;
using System.IO;
using System.Linq;
using System.Security;

namespace ExtCopy.extcopy
{
    public class CheckArgs
    {
        /// <summary>
        /// コマンドライン引数が正しいかチェックします。
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public InputInfo Check(string[] args)
        {
            InputInfo inputInfo = new InputInfo();

            string argType = null;
            foreach (string arg in args)
            {
                string lowerArg = arg.ToLower();
                if (Const.ARG_TYPE.Contains(lowerArg))
                {
                    switch (lowerArg)
                    {
                        case "/o":
                            inputInfo.OverWrite = true;
                            break;
                        default:
                            argType = lowerArg;
                            break;
                    }
                }
                else
                {
                    switch (argType)
                    {
                        case "/fi":
                            try
                            {
                                DirectoryInfo info = new DirectoryInfo(arg);
                                if (!info.Exists)
                                {
                                    throw new ApplicationException("抽出元ディレクトリは存在しません");
                                }
                                inputInfo.InputDir = info;
                            }
                            catch (SecurityException)
                            {
                                throw new ApplicationException("抽出元ディレクトリにアクセス権限がありません");
                            }
                            catch (System.ArgumentException)
                            {
                                throw new ApplicationException("抽出元ディレクトリに無効な文字が含まれています");
                            }
                            catch (PathTooLongException)
                            {
                                throw new ApplicationException("抽出元ディレクトリのパスが長すぎます");
                            }
                            break;
                        case "/fo":
                            try
                            {
                                DirectoryInfo info = new DirectoryInfo(arg);
                                if (!info.Exists)
                                {
                                    info.Create();
                                }
                                inputInfo.OutputDir = info;
                            }
                            catch (SecurityException)
                            {
                                throw new ApplicationException("抽出先ディレクトリにアクセス権限がありません");
                            }
                            catch (System.ArgumentException)
                            {
                                throw new ApplicationException("抽出先ディレクトリに無効な文字が含まれています");
                            }
                            catch (PathTooLongException)
                            {
                                throw new ApplicationException("抽出先ディレクトリのパスが長すぎます");
                            }
                            catch (IOException)
                            {
                                throw new ApplicationException("抽出先ディレクトリの生成に失敗しました");
                            }
                            break;
                        case "/ds":
                            // TODO 開始日付引数チェック
                            break;
                        case "/de":
                            // TODO 終了日付引数チェック
                            break;
                        case "/sl":
                            // TODO 最低ステップ引数チェック
                            break;
                        case "/sh":
                            // TODO 最高ステップ引数チェック
                            break;
                        case "/mh":
                            // TODO 合計ステップ引数チェック
                            break;
                        case "/dv":
                            // TODO 分割数引数チェック
                            break;
                        case "/c":
                            // TODO カウント方式引数チェック
                            break;
                        default:
                            throw new ApplicationException("引数に誤りがあります。");
                    }
                }
            }

            return inputInfo;
        }
    }
}