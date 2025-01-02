using System;

namespace IFC5Tekla.Engine.Exceptions;
public class MappingException : Exception
{
    public MappingException(string propertyName) : base($"Could not map property {propertyName}")
    {
    }
}
