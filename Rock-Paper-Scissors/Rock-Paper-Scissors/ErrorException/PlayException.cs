using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Rock_Paper_Scissors.ErrorException
{
    public class PlayException : System.Exception, ISerializable
    {
        public PlayException(string errorMessage)
            : base(errorMessage)
        {

        }
    }
}