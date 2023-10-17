﻿namespace MRS.LibraryMessage
{
    public partial class Message
    {
        public LanguageEnum Language;
        public string message;
        public Enum EnumBC;

        private static string defaultViMessage = "[].";
        private static string defaultEnMessage = "[].";

        public Message(LanguageEnum language, Enum en)
        {
            Language = language;
            EnumBC = en;
            message = GetMessage(en);
        }

        public enum LanguageEnum
        {
            Vietnamese,
            English,
        }

        internal class LanguageName
        {
            internal const string VI = "VietNamese";
            internal const string EN = "English";
        }

        public static string GetLanguageName(LanguageEnum type)
        {
            string result = LanguageName.VI;
            switch (type)
            {
                case LanguageEnum.Vietnamese:
                    result = LanguageName.VI;
                    break;
                case LanguageEnum.English:
                    result = LanguageName.EN;
                    break;
                default:
                    result = LanguageName.VI;
                    break;
            }
            return result;
        }

        public static LanguageEnum GetLanguageEnum(string languageName)
        {
            LanguageEnum result = LanguageEnum.Vietnamese;
            switch (languageName)
            {
                case LanguageName.VI:
                    result = LanguageEnum.Vietnamese;
                    break;
                case LanguageName.EN:
                    result = LanguageEnum.English;
                    break;
                default:
                    result = LanguageEnum.Vietnamese;
                    break;
            }
            return result;
        }
    }
}
