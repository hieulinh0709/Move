using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using PluginValidator;
using PluginValidator.Configurations;
using PluginValidator.FileHandler;
using PluginValidator.Interface;
using System;
using System.IO;

namespace ValidatorTest
{
    [TestFixture]
    public class ValidatorTest
    {
        private IValidator _validator;

        private PhoneConfiguration _phoneConfiguration = new PhoneConfiguration();
        private static Configuration Config = new Configuration
        {
            //name
            NullOrEmptyAllowed = false,
            NoOnlySpace = true,
            NoStringWithoutSpaces = true,
            BannedSymbols = @"!@#$%^&*()_+\/'-=;{}[]<>|",
            MaxNumWhiteSpaceBetweenName = "2",
            MustHasOneWhiteSpaceBetweenTwoWords = true,
            VietNameseSyllableAllowed = true,
            NumberAllowed = false,
            Capitalized = true,

            //business code
            //NullOrEmptyAllowed = false,
            //NoOnlySpace = true,
            //BannedSymbols = @"!@#$%^&*()_+\/'=;{}[]<>|",
            ContainQuantityNumberCharacter = "13",
            PositionDashCharacter = "11",

            // date
            //NumberAllowed = true,
            DateTimeFormat = "MM/dd/yyyy",
            MustBeBeforeSysDateAndCreatedDate = true,
            MustBeBeforeSysDateAndCreatedDateAndThanIdIssueDate = true,

            //address
            //NullOrEmptyAllowed = false,
            //NoOnlySpace = true,
            //VietNameseSyllableAllowed = true,
            //MaxNumWhiteSpaceBetweenName = "2",
            //BannedSymbols = @"!@#$%^&*()_+\'-=;{}[]<>|",

            // telephone1
            //NullOrEmptyAllowed = false,
            //NumberAllowed = true,
            //BannedSymbols = @"!@#$%^&*()_+\/'=;{}[]<>|",
            NumberPhoneLength = new int[] { 100000010, 100000011, 100000012 },
            //VietNameseSyllableAllowed = true,
            //Capitalized = false,
            //ContainQuantityNumberCharacter = "13",

            NationalIdentityCardLength = "9",
            CitizenIdentityCardLength = "12",
            BannedSymbolAfterAt = @"!#$%^&*()_+\/'-=;{}[]<>|",
            EmailSuffix = "@gmail.com, @aia.com,  @yahoo.com",
            NoEndingWithCharacter = ". ,",
            PassportLength = "8",
            PhoneNumberProvinceCode = false,
            PhoneNumberProviderPrefixes = false,
            NoOnlyLetters = false,

        };

        [SetUp]
        public void SetUp()
        {

            string fileName = @"\PhoneConfiguration.json";
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + fileName;
            ReadOnlyJson readOnlyJson = new ReadOnlyJson();

            //configurationRules = JsonHelper.Deserialize<ConfigurationRules>(unsecureString);
            //JsonSerializer.Deserialize<ConfigurationRules>(unsecureString);
            //JsonConvert.DeserializeObject<ConfigurationRules>(secureString);

            _phoneConfiguration = readOnlyJson.ReadJson(path);

            _validator = new Validator(Config, _phoneConfiguration);
        }

        [Test]
        //name
        //[TestCase(null, "name", "LÂM HIẾU LINH")]
        //[TestCase(null, "name", "LÂM HIẾU")]
        //business code
        //[TestCase(null, "name", "1234567890-123")]
        //data createddate, sysdate
        //[TestCase(null, "name", "09/18/2021")]
        //address
        //[TestCase(null, "name", "/")]
        //[TestCase(null, "name", "311/89 NVC, Ninh Kieu Can Tho")]
        public void Validation_ValidValidation_ShouldReturnTrue(Entity entity, string key, string value)
        {
            bool result = _validator.Validation(entity, key, value);

            Assert.That(result, Is.True);
        }

        [Test]
        #region name rules invalid
        //[TestCase(null, "name", "FGHG FGHG GFHG FGHFG")]
        //[TestCase(null, "name", null)]
        //[TestCase(null, "name", "              ")]
        //[TestCase(null, "name", "FHFGHGHG")]
        //[TestCase(null, "name", "@#$^%^")]
        //[TestCase(null, "name", "FGHFGHGF 123")]
        //[TestCase(null, "name", "FGHFGHGF  123")]
        #endregion
        #region code rules invalid
        //[TestCase(null, "name", "12345678901-23")]
        //[TestCase(null, "name", null)]
        //[TestCase(null, "name", "fsdfd")]
        //[TestCase(null, "name", "0962 157 876")]
        //[TestCase(null, "name", "0962157876")]
        //[TestCase(null, "name", @"!@#$02005")]
        #endregion
        //data createddate, sysdate
        //[TestCase(null, "name", "01/20/2021")] //<  idissue date
        //[TestCase(null, "name", "09/19/2021")] //==created date
        //[TestCase(null, "name", "09/20/2021")] //>created date
        // address
        //[TestCase(null, "name", null)]
        //[TestCase(null, "name", "!@#!!$~")]
        //[TestCase(null, "name", "311/89 NVC, Ninh Kieu      Can Tho")]
        public void Validation_InvalidValidation_ShouldReturnFalse(Entity entity, string key, string value)
        {
            bool result = _validator.Validation(entity, key, value);
            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("fghfhfghgfh")]
        public void ContainsOnlyLetter_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.ContainsOnlyLetter();

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("dgdgfd123123")]
        [TestCase("12321323")]
        public void ContainsOnlyLetter_Valid_ShouldReturnFalse(string input)
        {
            bool result = input.ContainsOnlyLetter();

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("09/25/2021", "MM/dd/yyyy")]
        public void IsDateTimeFormat_Valid_ShouldReturnTrue(string input, string format)
        {
            bool result = input.IsDateTimeFormat(format);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("09/25/2021", "MM-dd-yyyy")]
        [TestCase("09/25/2021", "dd/MM/yyyy")]
        [TestCase("09/25/2021", "xxxxxxxxx")]
        public void IsDateTimeFormat_Valid_ShouldReturnFalse(string input, string format)
        {
            bool result = input.IsDateTimeFormat(format);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("09/18/2021", "09/19/2021")]
        public void IsMustBeforeSysDateCreatedDate_Valid_ShouldReturnTrue(string input, string createdDate)
        {
            bool result = _validator.IsMustBeforeSysDateCreatedDate(input, createdDate);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("01/20/2022", "09/19/2021")]
        [TestCase("09/19/2021", "09/19/2021")]
        [TestCase("09/20/2021", "09/19/2021")]
        public void IsMustBeforeSysDateCreatedDate_Inalid_ShouldReturnFalse(string input, string createdDate)
        {
            bool result = _validator.IsMustBeforeSysDateCreatedDate(input, createdDate);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("18/09/2021 12:00:00 AM", "09/19/2021", "09/17/2021")]
        [TestCase("09/17/2021", "09/19/2021", "09/16/2021")]
        [TestCase("09/18/2021", "09/19/2021", "09/17/2021")]
        public void IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate_Valid_ShouldReturnTrue(string input, string createdDate, string idIssueDate)
        {
            bool result = _validator.IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate(input, createdDate, idIssueDate);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("09/16/2021", "09/19/2021", "09/16/2021")]
        [TestCase("01/20/2021", "09/19/2021", "08/19/2021")]
        [TestCase("09/19/2021", "09/19/2021", "08/19/2021")]
        [TestCase("09/20/2021", "09/19/2021", "08/19/2021")]
        public void IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate_Inalid_ShouldReturnFalse(string input, string createdDate, string idIssueDate)
        {
            bool result = _validator.IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate(input, createdDate, idIssueDate);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase(@"0351234567")]
        [TestCase(@"0361234567")]
        public void IsProviderNetworkPrefix_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.IsProviderNetworkPrefix(input, _phoneConfiguration.Mobile);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(@"9001234567")]
        [TestCase(@"8001234567")]
        public void IsProviderNetworkPrefix_Inalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.IsProviderNetworkPrefix(input, _phoneConfiguration.Mobile);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase(@"2691234567")]
        [TestCase(@"2541234567")]
        public void IsProvinceCode_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.IsProvinceCode(input, _phoneConfiguration.Phone);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(@"9001234567")]
        [TestCase(@"8001234567")]
        public void IsProvinceCode_Inalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.IsProvinceCode(input, _phoneConfiguration.Phone);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase(@"á ô ồ")]
        public void IsStandardVietnamese_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.IsStandardVietnamese();

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(@"A566544A")]
        [TestCase(@"A566544a")]
        public void IsPassport_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.IsPassport(Config.PassportLength);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(@"a566544A")]
        [TestCase(@"SF45665445")]
        [TestCase(@"SF4445DFGD")]
        public void IsPassport_Valid_ShouldReturnFalse(string input)
        {
            bool result = input.IsPassport(Config.PassportLength);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase(@"日本語")]
        [TestCase(@"ខេមរភាសា")]
        public void IsStandardVietnamese_Valid_ShouldReturnFalse(string input)
        {
            bool result = input.IsStandardVietnamese();

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("á ă â ố ồ ô")]
        public void ContainsSyllableVietNamese_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.ContainsSyllableVietNamese();

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("CTYTNHH~!@#$%^MTV")]
        public void ContainsSyllableVietNamese_Ivalid_ShouldReturnFalse(string input)
        {
            bool result = input.ContainsSyllableVietNamese();

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("CTYTNHH~!@#$%^MTV")]
        public void IsContainsSymbol_InvalidSymbolEcpextSlash_ShouldReturnTrue(string input)
        {
            Configuration Config = new Configuration
            {
                NullOrEmptyAllowed = false,
                NumberAllowed = false,
                BannedSymbols = @"!@#$%^&*()_+\'-=;{}[]<>|"
            };

            Validator validatorSymbol = new Validator(Config, _phoneConfiguration);
            bool result = input.IsContainsBannedSymbol(Config.BannedSymbols);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("300/22")]
        public void IsContainsSymbol_ValidSymbolEcpextSlash_ShouldReturnFalse(string input)
        {
            Configuration Config = new Configuration
            {
                NullOrEmptyAllowed = false,
                NumberAllowed = false,
                BannedSymbols = @"!@#$%^&*()_+\'-=;{}[]<>|"
            };

            Validator validatorSymbol = new Validator(Config, _phoneConfiguration);
            bool result = input.IsContainsBannedSymbol(Config.BannedSymbols);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("CTYTNHH~!@#$%^MTV")]
        public void IsContainsSymbol_InvalidSymbolEcpextDash_ShouldReturnTrue(string input)
        {
            Configuration Config = new Configuration
            {
                NullOrEmptyAllowed = false,
                NumberAllowed = false,
                BannedSymbols = @"!@#$%^&*()_+\/'=;{}[]<>|"
            };

            Validator validatorSymbol = new Validator(Config, _phoneConfiguration);
            bool result = input.IsContainsBannedSymbol(Config.BannedSymbols);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("CTY-TNHH")]
        public void IsContainsSymbol_ValidSymbolEcpextDash_ShouldReturnFalse(string input)
        {
            Configuration Config = new Configuration
            {
                NullOrEmptyAllowed = false,
                NumberAllowed = false,
                BannedSymbols = @"!@#$%^&*()_+\/'=;{}[]<>|"
            };

            Validator validatorSymbol = new Validator(Config, _phoneConfiguration);
            bool result = input.IsContainsBannedSymbol(Config.BannedSymbols);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("CTYTNHH~!@#$%^MTV")]
        public void IsContainsBannedSymbol_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.IsContainsBannedSymbol(Config.BannedSymbols);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("CTYTNHHcbcvbvcb")]
        public void IsContainsBannedSymbol_Invalid_ShouldReturnFalse(string input)
        {
            bool result = input.IsContainsBannedSymbol(Config.BannedSymbols);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("hieulinh@GMAIL.com")]
        [TestCase("hieulinh@aia.com")]
        [TestCase("hieulinh@yahoo.com")]
        public void IsRuleEmail_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.IsRuleEmail(input, Config.EmailSuffix);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("hieulinh@GMAlL.com")]
        [TestCase("hieulinh@@$@%com")]
        [TestCase("hieulinh@ aIa.com")]
        [TestCase("hieulinh@yaho0.com")]
        public void IsRuleEmail_Invalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.IsRuleEmail(input, Config.EmailSuffix);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("366190728123")]
        public void IsLengthCCCD_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.IsLengthCitizenIdentityCard(input, Config.CitizenIdentityCardLength);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("366190728")]
        public void IsLengthCCCD_Invalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.IsLengthCitizenIdentityCard(input, Config.CitizenIdentityCardLength);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("366190728")]
        public void IsLengthNationalIdentityCardD_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.IsLengthNationalIdentityCard(input, Config.NationalIdentityCardLength);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("3661907")]
        [TestCase("asdsad")]
        [TestCase("366190728123")]
        public void IsLengthNationalIdentityCard_Invalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.IsLengthNationalIdentityCard(input, Config.NationalIdentityCardLength);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("1234567890-123")]
        public void IsHaveQuantityNumCharAndPositionDash_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.IsHaveQuantityNumCharAndPositionDash(input, Config.ContainQuantityNumberCharacter);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("1234567890-12")]
        public void IsHaveQuantityNumCharAndPositionDash_Invalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.IsHaveQuantityNumCharAndPositionDash(input, Config.ContainQuantityNumberCharacter);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("1234567890-123")]
        public void PositionDashCharacter_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.PositionDashCharacter(input, Config.PositionDashCharacter);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("1234567890123")]
        [TestCase("123456789-0123")]
        public void PositionDashCharacter_Invalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.PositionDashCharacter(input, Config.PositionDashCharacter);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("09/10/2021")]
        public void IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate_Valid_ShouldReturnTrue(DateTime input)
        {
            string anomymousDate = input.ToString("MM/dd/yyyy");
            string createdDate = "09/11/2021";
            string idIssueDate = "09/08/2021";
            bool result = _validator.IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate(anomymousDate, createdDate, idIssueDate);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("09/30/2021")]
        [TestCase("09/14/2021")]
        public void IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate_Invalid_ShouldReturnFalse(DateTime input)
        {
            string anomymousDate = input.ToString("MM/dd/yyyy");
            string createdDate = "09/11/2021";
            string idIssueDate = "09/11/2021";
            bool result = _validator.IsMustBeforeSysDateCreatedDateAndThanIdIssueDdate(anomymousDate, createdDate, idIssueDate);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("09/10/2021")]
        public void IsMustBeforeSysDateCreatedDate_Valid_ShouldReturnTrue(DateTime input)
        {
            string anomymousDate = input.ToString("MM/dd/yyyy");
            string createdDate = "09/11/2021";
            bool result = _validator.IsMustBeforeSysDateCreatedDate(anomymousDate, createdDate);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("09/30/2021")]
        [TestCase("09/14/2021")]
        public void IsMustBeforeSysDateCreatedDate_Invalid_ShouldReturnFalse(DateTime input)
        {
            string anomymousDate = input.ToString("MM/dd/yyyy");
            string createdDate = "09/11/2021";
            bool result = _validator.IsMustBeforeSysDateCreatedDate(anomymousDate, createdDate);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("hieulinh512@gmail.com")]
        public void IsNoSymbolAfterAt_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.IsNoSymbolAfterAt(input, Config.BannedSymbolAfterAt);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("hieulinh512@#$%^gmail.com")]
        [TestCase("hieulinh512@123^gmail.com")]
        public void IsNoSymbolAfterAt_Invalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.IsNoSymbolAfterAt(input, Config.BannedSymbolAfterAt);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("100000000000")]
        public void IsOnlyNumber_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.IsOnlyNumber();

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("Lam Hieu Linh")]
        [TestCase("Lam Hie12321321")]
        public void IsOnlyNumber_Valid_ShouldReturnFalse(string input)
        {
            bool result = input.IsOnlyNumber();

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("Lam Hieu Linh")]
        public void SplitFullNameToFirstname_Valid_ShouldReturnEqual(string input)
        {
            string result = _validator.SplitFullNameToLastname(input);

            Assert.AreEqual("Lam Hieu", result);
        }

        [Test]
        [TestCase("Lam Hieu Linh")]
        public void SplitFullNameToFirstname_Invalid_ShouldReturnEqual(string input)
        {
            string result = _validator.SplitFullNameToFirstname(input);

            Assert.AreEqual("LINH", result);
        }

        [Test]
        [TestCase("UPPER123123")]
        [TestCase("UPPER DFGF DFGFD")]
        [TestCase(@"UPPER DFGF`~!@#$%^&*()_+-=[/\]{},./<>")]
        public void IsCapitalized_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.IsCapitalized();

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("lower")]
        [TestCase("loweR")]
        public void IsCapitalized_Invalid_ShouldReturnFalse(string input)
        {
            bool result = input.IsCapitalized();

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("a b c")]
        public void IsNoMoreThanTwoWhiteSpaceBetweenNames_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.IsNoMoreThanTwoWhiteSpaceBetweenNames(Config.MaxNumWhiteSpaceBetweenName);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("a b c d")]
        [TestCase("   ")]
        public void IsNoMoreThanTwoWhiteSpaceBetweenNames_Invalid_ShouldReturnFalse(string input)
        {
            bool result = input.IsNoMoreThanTwoWhiteSpaceBetweenNames(Config.MaxNumWhiteSpaceBetweenName);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("asdasdsa asdsa")]
        public void MustBeContainsWhiteSpace_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.MustBeContainsWhiteSpace();

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("adsdsa")]
        [TestCase("hieulinh@gmail.com")]
        public void MustBeContainsWhiteSpace_InValid_ShouldReturnFalse(string input)
        {
            bool result = input.MustBeContainsWhiteSpace();

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("asdf123456790")]
        [TestCase("as")]
        public void IsHaveQuantityNumberPhoneConfig_Invalid_ShouldReturnFalse(string input)
        {
            bool result = _validator.IsHaveQuantityNumberPhoneConfig(input, Config.NumberPhoneLength);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("12345690000")]
        [TestCase("1234569000")]
        public void IsHaveQuantityNumberPhoneConfig_Valid_ShouldReturnTrue(string input)
        {
            bool result = _validator.IsHaveQuantityNumberPhoneConfig(input, Config.NumberPhoneLength);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("nodo.t", ". ,")]
        [TestCase(".nodot", ". ,")]
        [TestCase(",nodot", ". ,")]
        public void NoEndingWithCharacter_InValid_ShouldReturnFalse(string input, string noEndingWithCharacter)
        {
            bool result = _validator.NoEndingWithCharacter(input, noEndingWithCharacter);

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase(".", ". ,")]
        [TestCase(",", ". ,")]
        [TestCase("abc.", ". ,")]
        [TestCase("abc,", ". ,")]
        public void IsEndingCommaAndDot_Valid_ShouldReturnTrue(string input, string noEndingWithCharacter)
        {
            bool result = _validator.NoEndingWithCharacter(input, noEndingWithCharacter);

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("nodot1")]
        [TestCase("123456790")]
        public void IsContainsNumber_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.IsContainsNumber();

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("nodot")]
        [TestCase("@#$%")]
        public void IsContainsNumber_Invalid_ShouldReturnFalse(string input)
        {
            bool result = input.IsContainsNumber();

            Assert.That(result, Is.False);
        }

        [Test]
        [TestCase("abc 1")]
        [TestCase(" asdasd")]
        public void HasContent_Valid_ShouldReturnTrue(string input)
        {
            bool result = input.HasContent();

            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void HasContent_InValid_ShouldReturnFalse(string input)
        {
            bool result = input.HasContent();

            Assert.That(result, Is.False);
        }


        [TearDown]
        public void TearDown()
        {
        }
    }
}
