using ExtCopy.data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ExtCopy.extcopy
{
    /// <summary>
    /// 抽出コピープログラム
    /// </summary>
    public class ExtCopy
    {
        /// <summary>
        /// 入力情報
        /// </summary>
        private InputInfo inputInfo;

        /// <summary>
        /// メインメソッド
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                ExtCopy o = new ExtCopy();
                o.Execute(args);
            }
            catch (ApplicationException e)
            {

                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("システムエラー発生\n" + e);
            }
        }

        /// <summary>
        /// 抽出コピー実行メソッド
        /// </summary>
        /// <param name="args"></param>
        private void Execute(string[] args)
        {
            CheckArgs(args);
        }

        /// <summary>
        /// 入力情報チェック
        /// </summary>
        /// <param name="args"></param>
        private void CheckArgs(string[] args)
        {
            CheckArgs check = new CheckArgs();
            inputInfo = check.Check(args);

            // 抽出元ディレクトリまたは抽出先ディレクトリが未指定の場合、エラー
            if (inputInfo.InputDir == null || inputInfo.OutputDir == null)
            {
                throw new ApplicationException("入力情報が不足しています");
            }
        }
    }
}
