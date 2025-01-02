using System;

namespace IFC5.Reader.Exceptions;
public class MappingException : Exception
{
    public MappingException(string propertyName) : base($"Could not map property {propertyName}")
    {
    }
}
