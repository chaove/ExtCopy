using System.Collections.Generic;
using System.IO;

namespace ExtCopy.data
{
    /// <summary>
    /// ユーザ入力情報クラス
    /// </summary>
    public class InputInfo
    {
        /// <summary>
        /// 抽出元ディレクトリ
        /// </summary>
        public DirectoryInfo InputDir { get; set; }

        /// <summary>
        /// 抽出先ディレクトリ
        /// </summary>
        public DirectoryInfo OutputDir { get; set; }

        /// <summary>
        /// 選択ファイルパターン
        /// </summary>
        public List<string> InclusionFileName { get; set; }

        /// <summary>
        /// 除外ファイルパターン
        /// </summary>
        public List<string> ExcludeFilename { get; set; }

        /// <summary>
        /// 更新日時（開始）
        /// </summary>
        public string StartDateTime { get; set; }

        /// <summary>
        /// 更新日時（終了）
        /// </summary>
        public string EndDateTime { get; set; }

        /// <summary>
        /// 最低行数
        /// </summary>
        public string MinStep { get; set; }

        /// <summary>
        /// 最大行数
        /// </summary>
        public string MaxStep { get; set; }

        /// <summary>
        /// 合計最大行数
        /// </summary>
        public string TotalMaxStep { get; set; }

        /// <summary>
        /// 上書き
        /// </summary>
        public bool OverWrite { get; set; }

        /// <summary>
        /// コピー先ディレクトリ分割数
        /// </summary>
        public int DevideNum { get; set; }

        /// <summary>
        /// 行数カウント方法
        /// </summary>
        public CountType CountType { get; set; }

        /// <summary>
        /// コピー先ディレクトリ分割用合計行数
        /// </summary>
        public long TotalStep { get; set; }

        /// <summary>
        /// 詳細情報出力フラグ
        /// </summary>
        public bool OutputDetail { get; set; }

        /// <summary>
        /// オブジェクト複製
        /// </summary>
        /// <returns></returns>
        public InputInfo Clone()
        {
            return (InputInfo)MemberwiseClone();
        }
    }
}
