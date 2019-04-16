﻿using System;

namespace ThornParser.Exceptions
{
    public class CancellationException : Exception
    {
        public GridRow Row { get; }

        public CancellationException(GridRow row)
        {
            Row = row;
        }

        public CancellationException(GridRow row, Exception inner) : base("", inner)
        {
            Row = row;
        }
    }
}
