using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{

    public class OperationResult<T>
    {
        public OperationResult(bool succeed, T data)
        {
            Succeeded = succeed;
            Data = data;
        }

        public OperationResult(bool succeed)
        {
            Succeeded = succeed;
        }

        public bool Succeeded { get; set; } = true;
        public T Data { get; set; }
        public string Message { get; set; }
    }

    public class OperationResult : OperationResult<bool>
    { 
        public OperationResult(bool succeed) : base(succeed)
        {

        }
    }
}