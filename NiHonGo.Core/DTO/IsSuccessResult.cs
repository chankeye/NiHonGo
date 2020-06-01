using NiHonGo.Core.Enum;
namespace NiHonGo.Core.DTO
{
    public class IsSuccessResult
    {
        #region Constructors
        /// <summary>
        /// Success
        /// </summary>
        public IsSuccessResult()
        {
            IsSuccess = true;
        }

        /// <summary>
        /// Failed
        /// </summary>
        /// <param name="errorCode">Error code</param>
        public IsSuccessResult(string errorCode)
        {
            IsSuccess = false;
            ErrorCode = errorCode;
        }
        #endregion //Constructors

        /// <summary>
        /// Success or not
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public string ErrorCode { get; set; }
    }

    /// <summary>
    /// Success or not
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    public class IsSuccessResult<T> : IsSuccessResult
    {
        #region Constructors
        /// <summary>
        /// Success
        /// </summary>
        public IsSuccessResult()
            : base()
        { }

        /// <summary>
        /// Failed
        /// </summary>
        /// <param name="errorCode">Error code</param>
        public IsSuccessResult(string errorCode)
            : base(errorCode)
        {
        }
        #endregion //Constructors

        /// <summary>
        /// Return object
        /// </summary>
        public T ReturnObject { get; set; }
    }
}