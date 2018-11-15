using System.IO;

namespace ExtCopy.data
{
    /// <summary>
    /// 拡張ファイル情報
    /// </summary>
    public class ExtraFileInfo
    {
        /// <summary>
        /// ファイル情報
        /// </summary>
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// ステップ数
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// 開発者
        /// </summary>
        public string Author { get; set; }
    }
}
