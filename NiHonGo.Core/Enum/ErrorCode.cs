﻿namespace NiHonGo.Core.Enum
{
    public enum ErrorCode
    {
        UserNotFound,
        BlogNotFound,
        VideoNotFound,
        WordNotFound,
        GrammarNotFound,
        EmailIsNull,
        PasswordIsNull,
        NameIsNull,
        OldPasswordIsNull,
        NewPasswordIsNull,
        YoutubeUrlIsNull,
        JapaneseTitleIsNull,
        JapaneseContentIsNull,
        ChineseTitleIsNull,
        ChineseContentIsNull,
        TitleIsNull,
        DescriptionIsNull,
        BlogTitleIsNull,
        BlogDetailIsNull,
        EmailOrPasswordWrong,
        OldPasswordWrong,
        EmailFormatIsWrong,
        ChangePasswordFailed,
        AlreadyHadThisEmail,
        AlreadyHadThisVideo,
        AlreadyHadThisWord,
        AlreadyHadThisGrammar,
        NoAuthentication,
        InternalError
    }
}