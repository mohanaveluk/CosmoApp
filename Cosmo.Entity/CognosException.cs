using System;

namespace Cosmo.Entity
{
    public class CognosException : Exception
    {
        public CognosException()
        {

        }
        public CognosException(string message)
            : base(message)
        {
        }
    }
}