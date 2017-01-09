using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Models
{
    public class Result
    {
        public Result()
        {

        }
        public Result(string first, string second)
        {
            this.first = first;
            this.second = second;
        }
        public string first { get; set; }
        public string second { get; set; }
    }
}