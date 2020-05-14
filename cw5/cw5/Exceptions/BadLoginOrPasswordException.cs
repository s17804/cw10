#nullable enable
using System;

namespace cw5.Exceptions
{
    public class BadLoginOrPasswordException : Exception
    {
        public BadLoginOrPasswordException(string? message) : base(message)
        {
        }
    }
}