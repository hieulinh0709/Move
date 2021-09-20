using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using PluginValidator.Configurations;
using PluginValidator.FileHandler;
using PluginValidator.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PluginValidator
{
    public class EntityConfiguration
    {
        private Configuration configuration = new Configuration();
        private PhoneConfiguration _phoneConfiguration = new PhoneConfiguration();
        private ReadOnlyJson readOnlyJson;

        /// <summary>
        /// Get entity configurations base on request
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        public void GetEntityConfig(IOrganizationService service, Microsoft.Xrm.Sdk.Entity entity, string unsecureString)
        {
            EntityCollection entityConfigs = null;
            readOnlyJson = new ReadOnlyJson();

            if (entity == null && string.IsNullOrEmpty(entity.LogicalName))
            {
                return;
            }

            string fileName = @"\PhoneConfiguration.json";
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + fileName;

            //_phoneConfiguration = JsonHelper.Deserialize<PhoneConfiguration>(unsecureString);

            //JsonSerializer.Deserialize<ConfigurationRules>(unsecureString);
            //JsonConvert.DeserializeObject<ConfigurationRules>(secureString);

            _phoneConfiguration = readOnlyJson.ReadJson(path);

            // get config from entity taget
            try
            {
                // Retrieve all accounts owned by the user with read access rights to the accounts and 
                // where the last name of the user is not Cannon. 
                string fetch2 = $@"
                    <fetch>
                        <entity name='one_validationconfigrole'>
                            <all-attributes/>
                            <filter>
                                <condition attribute='one_entityname' operator='eq' value='{entity.LogicalName}' />
                            </filter>
                        </entity>
                    </fetch>";

                entityConfigs = service.RetrieveMultiple(new FetchExpression(fetch2)); 
            }
            catch (Exception)
            {
                throw new InvalidPluginExecutionException("Cannot fetch data");
            }

            var listConfigOfEntitys = entityConfigs.Entities.ToList();

            // get attribute from user input
            foreach (var attr in entity.Attributes)
            {
                // get attribute configuration
                var attrConfig = listConfigOfEntitys.Where(x => x.Attributes.Any(a => a.Value.Equals(attr.Key))).FirstOrDefault();

                if (attrConfig != null)
                {
                    var config = GetAttrEntityConfig(service, entity, attr, attrConfig);

                    if (config != null)
                    {
                        string attrValue = string.Empty;
                        string attrKey = attr.Value.ToString();
                        attrValue = attr.Value.ToString();
                        var validator = new Validator(config, _phoneConfiguration).Validation(entity, attrKey, attrValue);
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Get the validation rule convert to configuration object
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <param name="attr"></param>
        /// <param name="entityConfig"></param>
        public Configuration GetAttrEntityConfig(IOrganizationService service, Entity entity, KeyValuePair<string, object> attr, Entity entityConfig)
        {

            if (entityConfig != null)
            {
                try
                {
                    configuration.NullOrEmptyAllowed = entityConfig.GetAttributeValue<bool>("one_isnull");
                    configuration.NumberAllowed = entityConfig.GetAttributeValue<bool>("one_numberallow");
                    configuration.NoOnlySpace = entityConfig.GetAttributeValue<bool>("one_noonlyspace");
                    configuration.BannedSymbols = entityConfig.GetAttributeValue<string>("one_bannedsymbols");
                    configuration.MustHasOneWhiteSpaceBetweenTwoWords = entityConfig.GetAttributeValue<bool>("one_musthasonewhitespacebetweentwowords");
                    configuration.MaxNumWhiteSpaceBetweenName = entityConfig.GetAttributeValue<string>("one_maxnumberwhitespacebetweenname");
                    configuration.StandardVietNamese = entityConfig.GetAttributeValue<bool>("one_standardvietnamese");
                    configuration.VietNameseSyllableAllowed = entityConfig.GetAttributeValue<bool>("one_syllablevietnamese");
                    configuration.Capitalized = entityConfig.GetAttributeValue<bool>("one_capitalized");
                    configuration.ContainQuantityNumberCharacter = entityConfig.GetAttributeValue<string>("one_containquantitynumbercharacter");
                    configuration.PositionDashCharacter = entityConfig.GetAttributeValue<string>("one_positiondashcharacter");

                    var length = entityConfig.GetAttributeValue<OptionSetValueCollection>("one_numberphonelength")?.Any();

                    if (length != null)
                    {
                        configuration.NumberPhoneLength = entityConfig.GetAttributeValue<OptionSetValueCollection>("one_numberphonelength").Select(i => i.Value).ToArray();

                    }

                    configuration.NationalIdentityCardLength = entityConfig.GetAttributeValue<string>("one_nationalidentitycardlength");
                    configuration.CitizenIdentityCardLength = entityConfig.GetAttributeValue<string>("one_citizenidentitycardlength");
                    configuration.BannedSymbolCheckAfterAt = entityConfig.GetAttributeValue<string>("one_bannedsymbolcheckafterat");
                    configuration.EmailSuffix = entityConfig.GetAttributeValue<string>("one_emailsuffix");
                    configuration.NoEndingWithCharacter = entityConfig.GetAttributeValue<string>("one_noendingwithcharacter");
                    configuration.PassportLength = entityConfig.GetAttributeValue<string>("one_passportlength");
                    configuration.NoOnlyNumber = entityConfig.GetAttributeValue<bool>("one_noonlynumber");
                    configuration.NoStringWithoutSpaces = entityConfig.GetAttributeValue<bool>("one_nostringwithoutspace");
                    configuration.MustBeBeforeSysDateAndCreatedDate = entityConfig.GetAttributeValue<bool>("one_mustbebeforesystemandcreateddate");
                    configuration.PhoneNumberProvinceCode = entityConfig.GetAttributeValue<bool>("one_phonenumberprovincecode");
                    configuration.PhoneNumberProviderPrefixes = entityConfig.GetAttributeValue<bool>("one_phonenumberproviderprefixes");
                    configuration.NoOnlyLetters = entityConfig.GetAttributeValue<bool>("one_noonlyletters");
                    configuration.SplitsFullnameToFirstNameLastName = entityConfig.GetAttributeValue<bool>("one_splitsfullnametofirstnameandlastname");
                    configuration.OneWordHasNoSpaces = entityConfig.GetAttributeValue<bool>("one_onewordhasnospaces");

                    // chưa viết hàm xử lý
                    configuration.WarningAgentMobileUsedForCustomers = entityConfig.GetAttributeValue<bool>("one_warningagentsmobiletobeusedforcustomers");
                    configuration.WarningSameMobileNumbersForOtherPOCustomers = entityConfig.GetAttributeValue<bool>("one_warningsamemobilenumberforotherpocustomer");


                }
                catch (Exception ex)
                {

                    throw new InvalidPluginExecutionException("Have a problem while get rules configurations" + ex);
                }

            }
            return configuration;

        }
        
    }
}
