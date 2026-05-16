using System;

namespace HtmlToPdf.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class DataNotFoundException : DomainException
    {
        public DataNotFoundException(string message) : base(message) { }
    }
}
