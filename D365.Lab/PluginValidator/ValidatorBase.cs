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

        /// <summary>
        /// Contains number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsContainsNumber(string value)
        {
            if (!value.HasContent())
                return false;

            if (value.Any(c => char.IsDigit(c)))
                return true;
            return false;
        }



        /// <summary>
        /// Count number characters in string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int CountNumberCharacter(string value)
        {
            if (!value.HasContent())
                return 0;

            int countChar = 0;
            foreach (char c in value)
            {
                if (char.IsDigit(c))
                {
                    countChar++;
                }
            }

            return countChar;
        }


    }
}
