using System.Runtime.Serialization;

namespace ArbTech.SharedKernel.Exceptions;

[Serializable]
public class DomainObjectAlreadyExistsException : DomainException
{
    public DomainObjectAlreadyExistsException(string propertyName, string alreadyExistsValue)
        : base("'{0}' already exists", new List<string> { alreadyExistsValue })
    {
        AddMetadata(propertyName, alreadyExistsValue);
    }
}
