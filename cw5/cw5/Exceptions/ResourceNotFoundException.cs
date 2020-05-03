#nullable enable
using System;

namespace cw5.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string? message) : base(message)
        {
        }
    }
}