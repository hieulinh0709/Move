using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using PluginValidator.Configurations;
using PluginValidator.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PluginValidator
{
    public class Validator : ValidatorBase, IValidator
    {
        private readonly Configuration _configuration;
        private readonly PhoneConfiguration _phoneConfiguration;

        public Validator(
            Configuration configuration,
            PhoneConfiguration phoneConfiguration)
        {
            _configuration = configuration;
            _phoneConfiguration = phoneConfiguration;
        }

        public override bool Validation(Entity entity, string attrKey, string attrValue)
        {

            if (!_configuration.NullOrEmptyAllowed)
            {
                if (!attrValue.HasContent())
                    throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: No null"));
            }

            if (_configuration.NoStringWithoutSpaces)
            {
                bool contains = attrValue.MustBeContainsWhiteSpace();

                if (!contains) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: No string without spaces)"));
            }

            if (_configuration.MaxNumWhiteSpaceBetweenName.HasContent())
            {
                if (!attrValue.IsNoMoreThanTwoWhiteSpaceBetweenNames(_configuration.MaxNumWhiteSpaceBetweenName))throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: More than two white spaces between names"));
            }

            if (_configuration.NoMoreThanQuantitySpacesBetweenCharacters.HasContent())
            {
                if (!attrValue.IsNoMoreThanQuatityWhiteSpaceBetweenCharacters(_configuration.NoMoreThanQuantitySpacesBetweenCharacters)) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: More than two white spaces between characters"));
            }

            if (_configuration.BannedSymbols.HasContent())
            {
                bool isContainsSymbol = attrValue.IsContainsBannedSymbol(_configuration.BannedSymbols);

                if (isContainsSymbol)
                {
                    throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: Contains banned symbols"));
                }
            }

            //No syllable in VN
            if (!_configuration.VietNameseSyllableAllowed)
            {
                bool containsSyllableVI = attrValue.ContainsSyllableVietNamese();

                if (containsSyllableVI) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: contain syllable vietnamese"));
            }

            if (!_configuration.NumberAllowed)
            {
                bool containNumber = attrValue.ContainsAnyNumber(); 

                if (containNumber) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: Contains number"));
            }

            if (_configuration.Capitalized)
            {
                bool isCapitalized = attrValue.IsCapitalized();

                if (!isCapitalized) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: Must be captializd"));
            }

            if (_configuration.DateTimeFormat.HasContent())
            {
                bool isValid = attrValue.IsDateTimeFormat(_configuration.DateTimeFormat.RemoveAllWhiteSpace());

                if (!isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1} {2}", attrKey, "invalid: Not match format", _configuration.DateTimeFormat));
            }

            if (_configuration.MustBeBeforeSysDateAndCreatedDate)
            {
                string createdDate = entity.GetAttributeValue<DateTime>("createdon").ToString("MM/dd/yyyy");//"09/19/2021";//

                if (!attrValue.HasContent())
                    throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: No null"));
                bool result = IsMustBeforeSysDateCreatedDate(attrValue, createdDate);

                if (!result)
                    throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: No null"));
            }

            if (_configuration.MustBeBeforeSysDateAndCreatedDateAndThanIdIssueDate)
            {
                string createdDate = "09/19/2021"; //entity.GetAttributeValue<DateTime>("createdon").ToString("MM/dd/yyyy");
                string idIssueDate = "08/19/2021";//entity.GetAttributeValue<DateTime>("idissuedate").ToString("MM/dd/yyyy");

                if (!attrValue.HasContent())
                    throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: No null"));

                bool result = IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate(attrValue, createdDate, idIssueDate);
                if (!result)
                    return false;
            }

            if (_configuration.ContainQuantityNumberCharacter.HasContent())
            {
                bool isHaveQtyNumChar = IsHaveQuantityNumCharAndPositionDash(attrValue, _configuration.ContainQuantityNumberCharacter);

                if (!isHaveQtyNumChar) throw new InvalidPluginExecutionException(string.Format("{0} {1} {2}", attrKey, "invalid: Must have"+ _configuration.ContainQuantityNumberCharacter +"number characters and symbol '-'."));

                bool dashIsCharacterDigitth = PositionDashCharacter(attrValue, _configuration.PositionDashCharacter);

                if (!dashIsCharacterDigitth) throw new InvalidPluginExecutionException(string.Format("{0} {1} {2}", attrKey, "invalid: Symbol '-' is" + _configuration.PositionDashCharacter +"th character"));
            }

            //_configuration.MustBeBeforeSysDateAndCreatedDate

            if (_configuration.EmailSuffix.HasContent())
            {
                bool isEmailValid = IsRuleEmail(attrValue, _configuration.EmailSuffix);

                if (!isEmailValid) throw new InvalidPluginExecutionException(string.Format("{0} {1} {2}", attrKey, "invalid: There is rule for email ending is"+ _configuration.EmailSuffix));
            }

            if (_configuration.BannedSymbolAfterAt.HasContent())
            {
                bool isValid = IsNoSymbolAfterAt(attrValue, _configuration.BannedSymbolAfterAt);

                if (!isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1} {2}", attrKey, "invalid: No strange symbol" + _configuration.BannedSymbolAfterAt));
            }

            if (_configuration.NoOnlyNumber)
            {
                bool isValid = attrValue.IsOnlyNumber();

                if (isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: No only numbe" ));
            }

            if (_configuration.NoEndingWithCharacter.HasContent())
            {
                bool isValid = NoEndingWithCharacter(attrValue, _configuration.NoEndingWithCharacter);

                if (isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: No ending with", _configuration.NoEndingWithCharacter));
            }

            if (_configuration.ContainsNumberCharacter)
            {
                bool isValid = attrValue.ContainsAnyNumber();

                if (!isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1} {2}", attrKey, "invalid: There are" + _configuration.ContainsNumberCharacter + "number character"));
            }

            if (_configuration.NumberPhoneLength.Any())
            {
                bool isValid = IsHaveQuantityNumberPhoneConfig(attrValue, _configuration.NumberPhoneLength);

                if (!isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1} {2}", attrKey, "invalid: There are" + _configuration.NumberPhoneLength.Aggregate(string.Empty, (s, i) => s + i.ToString()) + "number character"));
            }

            if (_configuration.PassportLength.HasContent())
            {
                bool isValid = attrValue.IsPassport(_configuration.PassportLength);

                if (!isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: Passport" + _configuration.PassportLength + "characters"));
            }

            if (_configuration.PhoneNumberProvinceCode)
            {
                bool isValid = IsProvinceCode(attrValue, _phoneConfiguration.Phone); 

                if (!isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: Number phone not match with province code"));
            }

            if (_configuration.PhoneNumberProviderPrefixes)
            {
                bool isValid = IsProviderNetworkPrefix(attrValue, _phoneConfiguration.Mobile);

                if (!isValid) throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: Number phone not match with provider prefix"));
            }


            if (_configuration.NoOnlyLetters)
            {
                bool isValid = attrValue.ContainsOnlyLetter();

                if (isValid)
                    throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: No only letter"));
            }

            if (_configuration.OneWordHasNoSpaces)
            {
                int countSpace = attrValue.CountWhiteSpace();

                if (countSpace > 0)
                    throw new InvalidPluginExecutionException(string.Format("{0} {1}", attrKey, "invalid: One word has no spaces"));

            } 

            if (_configuration.SplitsFullnameToFirstNameLastName)
            {
                if (!attrValue.HasContent())
                    throw new InvalidDataException("Fullname is invalid");

                SplitFullNameToFirstname(attrValue);
            }

            return true;
        }

        /// <summary>
        /// No strange synbol ' as the starting and ending letter.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsStartingAndEndingSymbolApostrophe(string value)
        {
            if (!value.HasContent())
                return false;

            return value.First().Equals("'") || value.Last().Equals("'");
        }

        /// <summary>
        /// splits fullname to first name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SplitFullNameToFirstname(string fullname)
        {
            int indexStartFirstName = fullname.LastIndexOf(" ");
            string firstname = fullname.Substring(indexStartFirstName + 1).ToUpper();

            return firstname;
        }

        /// <summary>
        /// splits fullname to last name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SplitFullNameToLastname(string fullname)
        {
            if (!fullname.HasContent())
                throw new InvalidDataException("Fullname is invalid");

            int indexStartLastName = fullname.LastIndexOf(" ");
            string lastname = fullname.Substring(0, indexStartLastName);

            return lastname;
        }

        /// <summary>
        /// Full name syntax = Last Name + First name
        /// </summary>
        /// <param name="lastname"></param>
        /// <param name="firstname"></param>
        /// <returns></returns>
        public string FullName(string lastname, string firstname)
        {
            if (!lastname.HasContent() || !firstname.HasContent())
                throw new InvalidDataException("Lastname or Firstname is invalid");

            string fullname = string.Format("{0} {1}", lastname, firstname);

            return fullname;
        }

        /// <summary>
        /// No ending with ""."", "","""
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool NoEndingWithCharacter(string value, string noEndingWithCharacter)
        {
            if (!value.HasContent())
                return false;

            string bannedCharacter = noEndingWithCharacter;

            string[] character = bannedCharacter.Split(' ');
            bool endingIsDot = value.Last().ToString().Equals(".");
            bool endingIsComma = value.Last().ToString().Equals(",");

            string endingOfString = value.Last().ToString();

            var isContains = Array.Exists(character, e => e.Equals(endingOfString));

            if (isContains) return true;

            return false;
        }

        /// <summary>
        /// There are 10-11 number characters 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsHaveQuantityNumberPhoneConfig(string value, int[] number)
        {
            int valueDefaultOfD365 = 100000000;
            if (!value.HasContent())
                return false;

            if (value.ContainsAnyLetter())
                return false;

            int countattrValue = value.RemoveAllWhiteSpace().NumberCount();

            if (number.Any(c => c.Equals(valueDefaultOfD365 + countattrValue)))
                return true;

            return false;
        }

        /// <summary>
        /// There are quantity number characters and symbol ""-"". 
        /// Symbol ""-"" is position[th] character
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsHaveQuantityNumCharAndPositionDash(string value, string quantityNumberCharacters)
        {
            if (!value.HasContent())
                return false;

            int countChar = value.RemoveAllWhiteSpace().CountNumberCharacter();

            if (countChar == quantityNumberCharacters.ToInt())
                return true;

            return false;
        }

        /// <summary>
        /// Symbol ""-"" is [Digit]th character
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool PositionDashCharacter(string value, string positionDashCharacter)
        {
            if (!value.HasContent())
                return false;

            int indexDashIntoString = value.IndexOf("-") + 1;
            int indexDashIntoConfig = positionDashCharacter.ToInt();

            return indexDashIntoString == indexDashIntoConfig;
        }

        /// <summary>
        /// For CMND/CCCD: 9 or 12 numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsLengthNationalIdentityCard(string value, string nationalIdentityCardLength)
        {
            if (!value.HasContent())
                return false;

            bool onlyNumber = value.ContainsOnlyNumber();
            bool isLenghNationalIdentityCard = value.Length == nationalIdentityCardLength.ToInt();

            return onlyNumber && isLenghNationalIdentityCard;
        }

        /// <summary>
        /// For CMND/CCCD: 9 or 12 numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsLengthCitizenIdentityCard(string value, string citizenIdentityCardLength)
        {
            if (!value.HasContent())
                return false;

            bool onlyNumber = value.ContainsOnlyNumber();
            bool matchLength = value.Length == citizenIdentityCardLength.ToInt();

            return onlyNumber && matchLength;
        }


        /// <summary>
        /// No symnol after @
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsNoSymbolAfterAt(string value, string bannedSymbolCheckAfterAt)
        {
            bool containsBannedSymbol = true;

            if (!value.HasContent())
                return false;

            string[] split = value.Split('@');

            if (split.Length > 0)
            {
                containsBannedSymbol = split[1].Any(c => bannedSymbolCheckAfterAt.Contains(c));
            }

            if (containsBannedSymbol)
                return false;

            return true;
        }

        /// <summary>
        /// There is rule for email ending. 
        /// Ex: correct ending is @gmail.com, @aia.com, yahoo.com
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsRuleEmail(string value, string emailSuffix)
        {
            if (!value.HasContent())
                return false;

            string[] emailSuffixArray = emailSuffix.RemoveAllWhiteSpace().Split(',');

            value = value.ToLower();

            string email = value.Substring(value.IndexOf("@"));

            foreach (var rule in emailSuffixArray)
            {
                if (Regex.IsMatch(email, rule.ToLower()))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Must be before system date and created date
        /// </summary>
        /// <param name="anonymousDate"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        public bool IsMustBeforeSysDateCreatedDate(string anonymousDate, string createdDate)
        {
            if (!anonymousDate.HasContent() || !createdDate.HasContent())
                return false;

            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime anonymousDateFormat;
            if (anonymousDate.IsDateTimeFormat("MM/dd/yyyy"))
            {
                anonymousDateFormat = DateTime.ParseExact(anonymousDate, "MM/dd/yyyy", provider);
            } else
            {
                anonymousDateFormat = DateTime.Parse(anonymousDate);
            }

            DateTime createdDateFormat = DateTime.ParseExact(createdDate, "MM/dd/yyyy", provider);
            DateTime systemDate = DateTime.ParseExact(DateTime.Now.ToString("MM/dd/yyyy"), "MM/dd/yyyy", provider);
            bool anonymousDateThanCreatedDate = DateTime.Compare(anonymousDateFormat, createdDateFormat) >= 0;
            bool anonymousDateThanSystemDate = DateTime.Compare(anonymousDateFormat, systemDate) >= 0;
            if (anonymousDateThanCreatedDate || anonymousDateThanSystemDate)
                return false;

            return true;
        }

        /// <summary>
        /// Must be before system date and created date
        /// </summary>
        /// <param name="anonymousDate"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        public bool IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate(string anonymousDate, string createdDate, string idIssueDate)
        {
            if (!anonymousDate.HasContent() || !createdDate.HasContent() || !idIssueDate.HasContent())
                return false;

            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime anonymousDateFormat;
            if (anonymousDate.IsDateTimeFormat("MM/dd/yyyy"))
            {
                anonymousDateFormat = DateTime.ParseExact(anonymousDate, "MM/dd/yyyy", provider);
            }
            else
            {
                anonymousDateFormat = DateTime.Parse(anonymousDate);
            }

            DateTime createdDateFormat = DateTime.ParseExact(createdDate, "MM/dd/yyyy", provider);
            DateTime idIssueDateFormat = DateTime.ParseExact(idIssueDate, "MM/dd/yyyy", provider);
            DateTime systemDate = DateTime.ParseExact(DateTime.Now.ToString("MM/dd/yyyy"), "MM/dd/yyyy", provider);
            bool anonymousDateThanCreatedDate = anonymousDateFormat.CompareTo(createdDateFormat) >= 0;
            bool anonymousDateThanSystemDate = anonymousDateFormat.CompareTo(systemDate) >= 0;
            bool anonymousDateThanIdIssueDate = anonymousDateFormat.CompareTo(idIssueDateFormat) > 0;
            if (anonymousDateThanCreatedDate || anonymousDateThanSystemDate || !anonymousDateThanIdIssueDate)
                return false;

            return true;
        }

        /// <summary>
        /// Is province code
        /// </summary>
        /// <param name="numberPhone"></param>
        /// <returns></returns>
        public bool IsProvinceCode(string numberPhone, List<Phone> phones)
        {
            if (!numberPhone.HasContent())
                return false;

            if (!numberPhone.ContainsOnlyNumber())
                return false;

            foreach (var phone in phones)
            {
                string provinceCode = phone.ProvinceCode.RemoveAllWhiteSpace();
                string strimedValue = numberPhone.RemoveAllWhiteSpace();

                if (strimedValue.StartsWith(provinceCode))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerPrefix"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsProviderNetworkPrefix(string numberPhone, List<Mobile> mobiles)
        {
            if (!numberPhone.HasContent())
                return false;

            if (!numberPhone.ContainsOnlyNumber())
                return false;

            foreach (var mobile in mobiles)
            {
                string prefix = mobile.Prefix.RemoveAllWhiteSpace();
                string strimedValue = numberPhone.RemoveAllWhiteSpace();

                if (strimedValue.StartsWith(prefix))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsBirthCertificateLength(string value)
        {
            if (!value.HasContent())
                return false;
            string trimed = value.RemoveAllWhiteSpace();
            if (value.NumberCount() != Int32.Parse(trimed))
                return false;

            return true;
        }
    }
}
