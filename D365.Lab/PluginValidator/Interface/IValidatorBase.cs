using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace PluginValidator.Interface
{
    public interface IValidatorBase
    {
        bool Validation(Entity entity, string attrKey, string attrValue);

    }
}
