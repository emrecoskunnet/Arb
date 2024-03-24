using System.Runtime.Serialization;

namespace ArbTech.SharedKernel.Exceptions;

[Serializable]
public class DomainObjectNotFoundException: DomainException
{
    public DomainObjectNotFoundException(string propertyName, string notFoundValue)
        : base("Queried object '{0}' was not found with value of '{1}' ",  new List<string>{propertyName, notFoundValue})
    {
        AddMetadata(propertyName, notFoundValue);
    }
    
    public DomainObjectNotFoundException()
        : base("Queried object was not found", new List<string>())
    {
    }
}
