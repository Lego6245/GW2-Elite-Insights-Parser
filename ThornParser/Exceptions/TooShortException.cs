﻿using System;

namespace ThornParser.Exceptions
{
    public class TooShortException : Exception
    {
        public TooShortException() : base("Fight is too short, aborted")
        {
        }

    }
}
