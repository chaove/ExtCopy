using System.Collections.Generic;

namespace ExtCopy.common
{
    /// <summary>
    /// 定数クラス
    /// </summary>
    public class Const
    {
        /// <summary>
        /// バージョン
        /// </summary>
        public static readonly string VERSION = "2.0";

        /// 引数関連

        /// <summary>
        /// 引数タイプリスト
        /// </summary>
        public static readonly List<string> ARGUMENT_TYPE_LIST = new List<string>() { "/fi", "/fe", "/ds", "/de", "/sl", "/sh", "/mh", "/ow", "/dev", "/c", "/o" };

        /// <summary>
        /// 引数概要
        /// </summary>
        public static readonly Dictionary<string, string> ARGUMENT_OVERVIEW = new Dictionary<string, string>()
        {
            {"/fi", "対象文字列" },
            {"/fe", "対象文字列" },
            {"/ds", "日付" },
            {"/de", "日付" },
            {"/sl", "行数" },
            {"/sh", "行数" },
            {"/mh", "合計行数" },
            {"/ow", "上書き" },
            {"/dev", "ディレクトリ分割" },
            {"/c", "行数カウント方法" },
            {"/o", "詳細情報出力" }
        };

        /// <summary>
        /// 引数説明
        /// </summary>
        public static readonly Dictionary<string, string> ARGUMENT_DESCRIPTION = new Dictionary<string, string>()
        {
            {"/fi", "コピー対象にするファイル名を正規表現で指定します。" },
            {"/fe", "コピー対象外にするファイル名を正規表現で指定します。" },
            {"/ds", "コピー対象にするファイルの更新日(開始)を指定します。" },
            {"/de", "コピー対象にするファイルの更新日(終了)を指定します。" },
            {"/sl", "コピー対象にする個々のファイルの最低行数を指定します。" },
            {"/sh", "コピー対象にする個々のファイルの最高行数を指定します。" },
            {"/mh", "コピー対象にする全ファイルの最高合計行数を指定します。(単位にはk,M,Gが利用可能)" },
            {"/ow", "ファイルの上書きをＯＮに指定します。" },
            {"/dev", "コピー先ディレクトリ分割数を指定します。(行数で分割)" },
            {"/c", "行数カウント方法を指定します。(1:カウントしない, 2:行数カウントのみ, 指定なし:行数カウント+開発者取得)" },
            {"/o", "コピー対象の詳細情報を出力します。" }
        };

        /// <summary>
        /// 行数カウント方法、カウントなし
        /// </summary>
        public static readonly string NO_COUNT = "1";

        /// <summary>
        /// 行数カウント方法、行数のみ
        /// </summary>
        public static readonly string ROW_COUNT = "2";

        /// ファイル名

        /// <summary>
        /// 作成者情報を出力するファイル名
        /// </summary>
        public static readonly string AUTHOR_FILE_NAME = "extcopy_author.txt";

        /// エラーメッセージ

        /// <summary>
        /// 引数エラーメッセージ
        /// </summary>
        public static readonly string ERR_USAGE = "Usage : extcopy コピー元ディレクトリパス コピー先ディレクトリパス [ コピー対象文字列... ]\nオプション詳細はextcopy -helpで確認";
    }
}
