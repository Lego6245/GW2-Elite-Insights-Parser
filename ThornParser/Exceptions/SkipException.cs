using System;

namespace ThornParser.Exceptions
{
    public class SkipException : Exception
    {
        public SkipException() : base("Option enabled: Failed logs are skipped")
        {
        }

    }
}
