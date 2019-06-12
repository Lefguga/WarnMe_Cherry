using System;

namespace WarnMe_Cherry.Datenbank
{
    class DatenbankBaseException : Exception
    {
        public DatenbankBaseException() { }
        public DatenbankBaseException(string message) : base(message) { }
        public DatenbankBaseException(string message, Exception inner) : base(message, inner) { }
    }

    class KeyNotFoundException : DatenbankBaseException
    {
        public KeyNotFoundException() { }
        public KeyNotFoundException(string message) : base(message) { }
        public KeyNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    class CanNotConvertException : DatenbankBaseException
    {
        public CanNotConvertException() { }
        public CanNotConvertException(string message) : base(message) { }
        public CanNotConvertException(string message, Exception inner) : base(message, inner) { }
    }
}
