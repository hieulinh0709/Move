using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PluginValidator
{
    public static class StringExtensions
    {
        /// <summary>
        /// Only number characters
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsOnlyNumber(this string value)
        {
            if (!value.HasContent())
                return false;

            return value.All(c => char.IsNumber(c));
        }
        /// <summary>
        /// Is Passport
        /// </summary>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static bool IsPassport(this string attrValue, string passportLength)
        {
            if (!attrValue.HasContent())
            {
                return false;
            }
            if (attrValue.Length != passportLength.ToInt())
            {
                return false;
            }
            if (!char.IsLetter(attrValue.First()) || !char.IsUpper(attrValue.First()))
            {
                return false;
            }
            if (attrValue.ContainsOnlyLetter() || attrValue.ContainsOnlyNumber())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Chưa dùng được
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsStandardVietnamese(this string value)
        {
            if (!value.HasContent())
                return false;

            string standardVI = @"^[aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆfFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTuUùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ\s]+$";

            return Regex.IsMatch(value, standardVI);
        }
        /// <summary>
        /// Capitalized names
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsCapitalized(this string value)
        {
            Regex reg = new Regex(@"[`~!@#$%^&*()_+-=[/\]{},./\\<>123456790]");
            string format = reg.Replace(value, string.Empty);

            return format.RemoveAllWhiteSpace().All(c => char.IsUpper(c));
        }
        /// <summary>
        /// Contains syllable in Vietnamese
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsSyllableVietNamese(this string value)
        {
            string syllableVietNameese = @"^[àÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬđĐèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆìÌỉỈĩĨíÍịỊòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰỳỲỷỶỹỸýÝỵỴ\s]+$";
            return Regex.IsMatch(value, syllableVietNameese);
        }

        /// <summary>
        /// No strange symbol example: !@#$%^&*()_+\/-=;{}[]<>| 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsContainsBannedSymbol(this string value, string bannedSymbol)
        {
            if (!value.HasContent())
                return false;

            return value.Any(c => bannedSymbol.Contains(c));
        }

        /// <summary>
        /// No more than 2 spaces between characters
        /// </summary>
        /// <param name="value"></param>x
        /// <returns></returns>
        public static bool IsNoMoreThanQuatityWhiteSpaceBetweenCharacters(this string value, string quatityWhiteSpace)
        {
            if (!value.HasContent())
                return false;

            int countSpace = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i].ToString().Equals(" "))
                {
                    countSpace++;
                }
                else
                {
                    countSpace = 0;
                    continue;
                }

                if (countSpace > quatityWhiteSpace.ToInt())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// No more than 2 spaces between names
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNoMoreThanTwoWhiteSpaceBetweenNames(this string value, string maxNumWhiteSpaceBetweenName)
        {
            if (!value.HasContent())
                return false;

            int countSpace = value.Trim().CountWhiteSpace();

            if (countSpace > maxNumWhiteSpaceBetweenName.ToInt())
                return false;

            return true;
        }

        /// <summary>
        /// No string without space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool MustBeContainsWhiteSpace(this string value)
        {
            if (!value.HasContent())
                return false;

            int countSpace = value.Trim().CountWhiteSpace();

            if (countSpace <= 0)
                return false;
            return true;
        }

        /// <summary>
        /// No string without space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNoStringWithoutWhiteSpace(this string value)
        {
            if (!value.HasContent())
                return false;

            int countWhiteSpace = CountWhiteSpace(value);

            if (countWhiteSpace < 1)
                return false;

            return true;
        }

        /// <summary>
        /// Count space in string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CountWhiteSpace(this string value)
        {
            if (!value.HasContent())
                return 0;

            int spaceCount = value.Count(c => char.IsWhiteSpace(c));
            return spaceCount;
        }

        public static bool IsDateTimeFormat(this string input, string format)
        {
            DateTime dateTimeParsed;
            if (!input.HasContent() || !format.HasContent())
                return false;

            return DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeParsed);
        }
        /// <summary>
        /// remove all white space
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveAllWhiteSpace(this string input)
        {
            if (!input.HasContent())
                return string.Empty;
            return String.Concat(input.Where(c => !Char.IsWhiteSpace(c)));
        }

        /// <summary>
        /// Convert string to int
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int ToInt(this string input)
        {
            if (!input.HasContent())
                return 0;

            int returnInt = 0;

            try
            {
                returnInt = Int32.Parse(input);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnInt;
        }
        /// <summary>
        /// Has Content
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasContent(this string input)
        {
            return !string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// Contains Any Banned Symbol
        /// </summary>
        /// <param name="input"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static bool ContainsAnyBannedSymbol(this string input, string symbols)
        {
            return ContainsAnyBannedSymbol(input, symbols.ToCharArray());
        }

        /// <summary>
        /// Contains Any Banned Symbol
        /// </summary>
        /// <param name="input"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static bool ContainsAnyBannedSymbol(this string input, char[] symbols)
        {
            return input.HasContent() && input.Any(c => symbols.Contains(c));
        }

        /// <summary>
        /// Contains Any Banned Symbol After Character
        /// </summary>
        /// <param name="input"></param>
        /// <param name="symbols"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static bool ContainsAnyBannedSymbolAfter(this string input, string symbols, char seperator)
        {
            return input.ContainsAnyBannedSymbolAfter(symbols.ToCharArray(), seperator);
        }

        /// <summary>
        /// Contains Any Banned Symbol After
        /// </summary>
        /// <param name="input"></param>
        /// <param name="symbols"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static bool ContainsAnyBannedSymbolAfter(this string input, char[] symbols, char seperator)
        {
            if (!input.HasContent())
            {
                return false;
            }
            if (input.Count(c => c == seperator) != 1)
            {
                throw new InvalidDataException("there are 0 or more than 1 seperator found");
            }

            var arr = input.Split(seperator);
            var content = arr[1];
            return content.Any(c => symbols.Contains(c));
        }

        /// <summary>
        /// Contains Any Banned Symbol Before
        /// </summary>
        /// <param name="input"></param>
        /// <param name="symbols"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static bool ContainsAnyBannedSymbolBefore(this string input, string symbols, char seperator)
        {
            return input.ContainsAnyBannedSymbolBefore(symbols.ToCharArray(), seperator);
        }

        /// <summary>
        /// Contains Any Banned Symbol Before
        /// </summary>
        /// <param name="input"></param>
        /// <param name="symbols"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static bool ContainsAnyBannedSymbolBefore(this string input, char[] symbols, char seperator)
        {
            if (!input.HasContent())
            {
                return false;
            }
            if (input.Count(c => c == seperator) != 1)
            {
                throw new InvalidDataException("there are 0 or more than 1 seperator found");
            }
            var arr = input.Split(seperator);
            var content = arr[0];
            return content.Any(c => symbols.Contains(c));
        }

        /// <summary>
        /// White Space Count
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WhiteSpaceCount(this string input)
        {
            return input != null ? input.Count(c => char.IsWhiteSpace(c)) : 0;
        }

        /// <summary>
        /// Word Count
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int WordCount(this string input)
        {
            return input.HasContent() ? input.Split(' ').Count(element => element.HasContent()) : 0;
        }

        /// <summary>
        /// Number Count
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int NumberCount(this string input)
        {
            return input.HasContent() ? input.Count(c => char.IsDigit(c)) : 0;
        }

        /// <summary>
        /// Contains Any Number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsAnyNumber(this string input)
        {
            return input.HasContent() && input.Any(c => char.IsDigit(c));
        }

        /// <summary>
        /// Contains Any Letter
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsAnyLetter(this string input)
        {
            return input.HasContent() && input.Any(c => char.IsLetter(c));
        }

        /// <summary>
        /// Contains Only Number
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsOnlyNumber(this string input)
        {
            return input.HasContent() && input.All(c => char.IsDigit(c));
        }

        /// <summary>
        /// Contains Only Letter
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ContainsOnlyLetter(this string input)
        {
            return input.HasContent() && input.All(c => char.IsLetter(c));
        }

        /// <summary>
        /// Has Only One White Space Between Two Words
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasOnlyOneWhiteSpaceBetweenTwoWords(this string input)
        {
            if (!input.HasContent())
            {
                return false;
            }
            var arr = input.Split(' ');
            // if array contains any white space => more than one 1 space between words
            foreach (var element in arr)
            {
                if (!element.HasContent())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Has First Letter Of Each Word Capitalized
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasFirstLetterOfEachWordCapitalized(this string input)
        {
            if (!input.HasContent())
            {
                return false;
            }
            var arr = input.Split(' ');
            foreach (var word in arr)
            {
                if (word.HasContent())
                {
                    var first = word.First();
                    if (!char.IsLetter(first) || !char.IsUpper(first))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// not yet execute
        /// </summary>
        /// <param name="input"></param>
        /// <param name="countryPrefix"></param>
        /// <param name="providerPrefix"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(this string input, string countryPrefix, string providerPrefix, int minLength, int maxLength)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// not yet execute
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(this string input, int minLength, int maxLength)
        {
            throw new NotImplementedException();
        }

    }
}
