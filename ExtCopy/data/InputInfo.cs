using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtCopy.data
{
    /// <summary>
    /// 入力データクラス
    /// </summary>
    public class InputInfo
    {
        /// <summary>
        /// 抽出コピー元ディレクトリ
        /// </summary>
        public DirectoryInfo InputDir { get; set; }

        /// <summary>
        /// 抽出コピー先ディレクトリ
        /// </summary>
        public DirectoryInfo OutputDir { get; set; }

        /// <summary>
        /// コピー対象文字列
        /// </summary>
        public List<string> IncludeList { get; set; }

        /// <summary>
        /// コピー非対称文字列
        /// </summary>
        public List<string> ExcludeList { get; set; }

        /// <summary>
        /// 対象開始日時
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 対象終了日時
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 対象最低ステップ
        /// </summary>
        public long LowStep { get; set; }

        /// <summary>
        /// 対象最高ステップ
        /// </summary>
        public long HighStep { get; set; }

        /// <summary>
        /// コピー最大ステップ
        /// </summary>
        public long MaxTotalStep { get; set; }

        /// <summary>
        /// コピー先分割数
        /// </summary>
        public int DevideNum { get; set; }

        /// <summary>
        /// コピータイプ
        /// </summary>
        public CopyType copyType { get; set; }

        /// <summary>
        /// 強制上書きフラグ
        /// </summary>
        public bool OverWrite { get; set; }

        /// <summary>
        /// コンソール出力フラグ
        /// </summary>
        public bool Output { get; set; }
    }
}
