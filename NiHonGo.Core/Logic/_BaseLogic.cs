using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NiHonGo.Core.DTO;
using NiHonGo.Data.Models;

namespace NiHonGo.Core.Logic
{
    public abstract class _BaseLogic
    {
        /// <summary>
        /// Base Logic，all Logic are inherit it
        /// </summary>
        public _BaseLogic()
        {
        }

        /// <summary>
        /// NiHonGoContext
        /// </summary>
        protected NiHonGoContext NiHonGoContext
        {
            get { return _NiHonGoContext.Value; }
        }
        Lazy<NiHonGoContext> _NiHonGoContext = new Lazy<NiHonGoContext>();

        /// <summary>
        /// Get a new Logger object
        /// </summary>
        protected Logger GetLogger()
        {
            var method = new StackFrame(1).GetMethod();
            var fullMethodName =
                string.Format(
                    "{0}.{1}",
                    method.DeclaringType.FullName,
                    method.Name
                );

            return LogManager.GetLogger(fullMethodName);
        }

        /// <summary>get all exception's inner exception type</summary>
        protected IEnumerable<Type> GetAllExceptionType(Exception ex)
        {
            //只要他所有的type就好. 並不需要exception本身
            List<Type> list = new List<Type>();
            //assign the current exception as first object and then loop through its
            //inner exceptions till they are null
            for (Exception eCurrent = ex; eCurrent != null; eCurrent = eCurrent.InnerException)
            {
                list.Add(eCurrent.GetType());
            }
            return list;
        }
    }
}