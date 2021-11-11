using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_ts_support.Models
{
    public class ApiResponseModel
    {
        public bool success { get; init; }
        public object result { get; init; }

        public ApiResponseModel(bool success, object result)
        {
            this.success = success;
            this.result = result;
        }
    }
}
