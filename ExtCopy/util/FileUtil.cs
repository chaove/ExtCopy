using System;
using System.Collections.Generic;
using System.IO;

namespace ExtCopy.util
{
    /// <summary>
    /// 対象ディレクトリからファイルを取得するクラスです。
    /// </summary>
    class FileUtil
    {
        /// <summary>
        /// 対象ディレクトリから指定パターンのファイルを取得します。
        /// </summary>
        /// <param name="dir">対象ディレクトリ絶対パス</param>
        /// <param name="pattern">指定パターン</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> GetFiles(string dir, string pattern)
        {
            // 結果ファイルリスト
            List<FileInfo> files = new List<FileInfo>();

            // 対象ディレクトリ直下のファイルを取得
            IEnumerable<string> topFiles = Directory.EnumerateFiles(dir, pattern);
            foreach (string topFile in topFiles)
            {
                files.Add(new FileInfo(topFile));
            }

            // 対象ディレクトリ直下のディレクトリ配下のファイルをすべて取得
            foreach (var di in Directory.EnumerateDirectories(dir))
            {
                try
                {
                    foreach (var fi in Directory.EnumerateFiles(di, pattern, SearchOption.AllDirectories))
                    {
                        try
                        {
                            files.Add(new FileInfo(fi));
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // アクセスを拒否された場合は何もしない
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    // アクセスを拒否された場合は何もしない
                }
            }

            return files;
        }

        /// <summary>
        /// 対象ディレクトリから指定パターンのファイルを取得します。
        /// </summary>
        /// <param name="dir">対象ディレクトリ</param>
        /// <param name="pattern">指定パターン</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> GetFiles(DirectoryInfo dir, string pattern)
        {
            return GetFiles(dir.FullName, pattern);
        }

        /// <summary>
        /// 対象ディレクトリから指定パターンのファイルを取得します。
        /// </summary>
        /// <param name="dir">対象ディレクトリ絶対パス</param>
        /// <param name="patterns">指定パターンリスト</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> GetFiles(string dir, List<string> patterns)
        {
            List<FileInfo> files = new List<FileInfo>();
            foreach (string pattern in patterns)
            {
                List<FileInfo> tempList = GetFiles(dir, pattern);
                foreach (FileInfo file in tempList)
                {
                    FileInfo tempFile = files.Find(f => f.FullName == file.FullName);
                    if (tempFile == null)
                    {
                        files.Add(file);
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// 対象ディレクトリから指定パターンのファイルを取得します。
        /// </summary>
        /// <param name="dir">対象ディレクトリ</param>
        /// <param name="patterns">指定パターンリスト</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> GetFiles(DirectoryInfo dir, List<string> patterns)
        {
            return GetFiles(dir.FullName, patterns);
        }

        /// <summary>
        /// 対象ディレクトリから指定パターンのファイルを取得します。
        /// </summary>
        /// <param name="dir">対象ディレクトリ絶対パス</param>
        /// <param name="includePatterns">包含パターンリスト</param>
        /// <param name="excludePatterns">除外パターンリスト</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> GetFiles(string dir, List<string> includePatterns, List<string> excludePatterns)
        {
            List<FileInfo> includeFiles = GetFiles(dir, includePatterns);
            List<FileInfo> excludeFiles = GetFiles(dir, excludePatterns);
            List<FileInfo> files = new List<FileInfo>();

            foreach (FileInfo file in includeFiles)
            {
                FileInfo tempFile = excludeFiles.Find(f => f.FullName == file.FullName);
                if (tempFile == null)
                {
                    files.Add(file);
                }
            }

            return files;
        }

        /// <summary>
        /// 対象ディレクトリから指定パターンのファイルを取得します。
        /// </summary>
        /// <param name="dir">対象ディレクトリ</param>
        /// <param name="includePatterns">包含パターンリスト</param>
        /// <param name="excludePatterns">除外パターンリスト</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> GetFiles(DirectoryInfo dir, List<string> includePatterns, List<string> excludePatterns)
        {
            return GetFiles(dir.FullName, includePatterns, excludePatterns);
        }

        /// <summary>
        /// 対象ディレクトリから指定パターンのファイルを取得します。
        /// </summary>
        /// <param name="dir">対象ディレクトリ</param>
        /// <param name="includePattern">包含パターン</param>
        /// <param name="excludePatterns">除外パターンリスト</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> GetFiles(string dir, string includePattern, List<string> excludePatterns)
        {
            return GetFiles(dir, new List<string>() { includePattern }, excludePatterns);
        }

        /// <summary>
        /// 対象ディレクトリから指定パターンのファイルを取得します。
        /// </summary>
        /// <param name="dir">対象ディレクトリ</param>
        /// <param name="includePattern">包含パターン</param>
        /// <param name="excludePatterns">除外パターンリスト</param>
        /// <returns>ファイルリスト</returns>
        public List<FileInfo> GetFiles(DirectoryInfo dir, string includePattern, List<string> excludePatterns)
        {
            return GetFiles(dir.FullName, new List<string>() { includePattern }, excludePatterns);
        }
    }
}
