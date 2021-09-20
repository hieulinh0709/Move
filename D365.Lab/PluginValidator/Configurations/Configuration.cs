namespace PluginValidator.Configurations
{
    public class Configuration
    {
        public bool NullOrEmptyAllowed { get; set; } = false;
        public bool NumberAllowed { get; set; } = false;
        public bool WhiteSpaceAllowed { get; set; } = false;
        public bool VietNameseSyllableAllowed { get; set; } = true;
        public bool StandardVietNamese { get; set; } = true;
        // No more than 2 spaces between names
        public string MaxNumWhiteSpaceBetweenName { get; set; } = "";
        public string NoMoreThanQuantitySpacesBetweenCharacters { get; set; } = "";
        // No more than 2 spaces between character
        public bool MustHasOneWhiteSpaceBetweenTwoWords { get; set; } = false;

        // case1: @"!@#$%^&*()_+\/'-=;{}[]<>|";
        // case2: except "-"
        // case3: except "/"
        public string BannedSymbols { get; set; } = ""; 
        // No banned symbol after @
        public string BannedSymbolAfterAt { get; set; } = "";
        // No banned symbol before @
        public string BannedSymbolCheckBeforeAt { get; set; } = "";
        public string EmailSuffix { get; set; } = "";
        public int[] NumberPhoneLength { get; set; } = new int[] { };
        public bool PhoneNumberProvinceCode { get; set; } = false;
        public bool PhoneNumberProviderPrefixes { get; set; } = false;
        public bool Capitalized { get; set; } = false;
        public string NationalIdentityCardLength { get; set; } = "";
        public string CitizenIdentityCardLength { get; set; } = "";
        public string PassportLength { get; set; } = "";
        public string ContainQuantityNumberCharacter { get; set; } = "";
        // eample: Symbol ""-"" is 11th character
        public string PositionDashCharacter { get; set; } = "";
        public bool MustBeBeforeSysDateAndCreatedDate { get; set; } = false;
        public bool MustBeBeforeSysDateAndCreatedDateAndThanIdIssueDate { get; set; } = false;
        public bool NoStringWithoutSpaces { get; set; } = false;
        public bool NoOnlyNumber { get; set; } = false;
        public string NoEndingWithCharacter { get; set; } = "";
        public bool ContainsNumberCharacter { get; set; } = false;
        public string BirthCertificateLength { get; set; } = "";
        public string CopyOfIDcardLenth { get; set; } = "";
        public bool NoOnlySpace { get; set; }
        public string DateTimeFormat { get; set; } = "";
        public bool NoOnlyLetters { get; set; }
        public bool WarningAgentMobileUsedForCustomers { get; set; }
        public bool WarningSameMobileNumbersForOtherPOCustomers { get; set; }
        public bool SplitsFullnameToFirstNameLastName { get; set; }
        public bool OneWordHasNoSpaces { get; set; }
        


    }
}
