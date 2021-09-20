using Microsoft.Xrm.Sdk;
using PluginValidator.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PluginValidator
{
    public class ValidatorBase : IValidatorBase
    {
        public virtual bool Validation(Entity entity, string attrKey, string attrValue)
        {
            return true;
        }


    }
}
