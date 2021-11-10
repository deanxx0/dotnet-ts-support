using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_ts_support.Models
{
    public class ApiResponseModel
    {
        public bool success { get; init; }
        public string messge { get; init; }
        public object result { get; init; }

        public ApiResponseModel() { }
    }
}
