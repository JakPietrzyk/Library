using System;

namespace BooksLibrary.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string message): base(message)
        {

        }
    }
}