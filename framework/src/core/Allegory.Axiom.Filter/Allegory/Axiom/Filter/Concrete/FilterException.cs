using System;

namespace Allegory.Axiom.Filter.Concrete;

public class FilterException : Exception
{
    public FilterException() {}
    public FilterException(string message) : base(message) {}
}