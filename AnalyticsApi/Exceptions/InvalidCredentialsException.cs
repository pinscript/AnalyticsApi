using System;

namespace AnalyticsApi
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message)
            : base(message)
        {
            
        }
    }
}