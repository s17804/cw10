#nullable enable
using System;

namespace cw5.Exceptions
{
    public class ObjectAlreadyInDatabaseException : Exception
    {
        public ObjectAlreadyInDatabaseException(string? message) : base(message)
        {
        }
    }
}