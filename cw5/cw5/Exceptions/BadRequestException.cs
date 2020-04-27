using System;

namespace cw5.Exceptions
{
    public class BadRequestException : Exception
    {
        private readonly string _message;

        public BadRequestException(string message) : base(message)
        {
            _message = message;
        }
    }
}