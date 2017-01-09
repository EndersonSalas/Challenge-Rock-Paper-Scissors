using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WebService.ErrorException
{
    public class PlayException: System.Exception, ISerializable
    {
        public PlayException(string errorMessage)
            : base(errorMessage)
        { 
        
        }
    }
}