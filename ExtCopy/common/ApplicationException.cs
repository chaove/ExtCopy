using System;

namespace ExtCopy.common
{
    /// <summary>
    /// 業務エラークラス
    /// </summary>
    class ApplicationException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ApplicationException() : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message"></param>
        public ApplicationException(string message) : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ApplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
