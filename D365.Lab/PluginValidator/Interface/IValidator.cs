using PluginValidator.Configurations;
using System.Collections.Generic;

namespace PluginValidator.Interface
{
    public interface IValidator : IValidatorBase
    {
        /// <summary>
        /// FullName
        /// </summary>
        /// <param name="lastname"></param>
        /// <param name="firstname"></param>
        /// <returns></returns>
        string FullName(string lastname, string firstname);

        /// <summary>
        /// ending with ""."", "","""
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool NoEndingWithCharacter(string value, string noEndingCommaAndDot);

        /// <summary>
        /// There are 10-11 number characters 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsHaveQuantityNumberPhoneConfig(string value, int[] number);

        /// <summary>
        /// splits fullname to first name
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        string SplitFullNameToFirstname(string fullname);

        /// <summary>
        /// splits fullname to last name
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        string SplitFullNameToLastname(string fullname);

        /// <summary>
        /// No symnol after @
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsNoSymbolAfterAt(string value, string bannedSymbolCheckAfterAt);

        /// <summary>
        /// Must be before system date and created date
        /// </summary>
        /// <param name="anonymousDate"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        bool IsMustBeforeSysDateCreatedDate(string anonymousDate, string createdDate);

        /// <summary>
        /// Symbol ""-"" is position character
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool PositionDashCharacter(string value, string positionDashCharacter);

        /// <summary>
        /// There are 13 number characters and symbol ""-"". 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsHaveQuantityNumCharAndPositionDash(string value, string containQuantityNumberCharacter);

        /// <summary>
        /// For CMND/CCCD: 9 or 12 numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsLengthNationalIdentityCard(string value, string nationalIdentityCardLength);

        /// <summary>
        /// For CMND/CCCD: 9 or 12 numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsLengthCitizenIdentityCard(string value, string citizenIdentityCardLength);

        /// <summary>
        /// There is rule for email ending. 
        /// Ex: correct ending is @gmail.com, @aia.com, yahoo.com
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsRuleEmail(string value, string emailSuffix);

        /// <summary>
        /// Match with provice code config
        /// </summary>
        /// <param name="numberPhone"></param>
        /// <param name="phones"></param>
        /// <returns></returns>
        bool IsProvinceCode(string numberPhone, List<Phone> phones);

        /// <summary>
        /// Match with provider network config
        /// </summary>
        /// <param name="numberPhone"></param>
        /// <param name="mobiles"></param>
        /// <returns></returns>
        bool IsProviderNetworkPrefix(string numberPhone, List<Mobile> mobiles);

        /// <summary>
        /// Match with Birth Certificate Length config
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsBirthCertificateLength(string value);

        /// <summary>
        /// Match with condition: befor system, created and Id issue date
        /// </summary>
        /// <param name="anonymousDate"></param>
        /// <param name="createdDate"></param>
        /// <param name="idIssueDate"></param>
        /// <returns></returns>
        bool IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate(string anonymousDate, string createdDate, string idIssueDate);
    }
}