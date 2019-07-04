using System;
using System.Collections.Generic;
using System.Text;

namespace Bootstrap.Infrastructures.Models.Constants
{
    public class DefaultFieldLengths
    {
        public const int MinAccount = 8;
        public const int MaxAccount = 16;
        public const int MaxPassword = 32;
        public const int MinPassword = 8;
        public const int MaxPasswordCipherText = 256;
        public const int MinOrderNo = 16;
        public const int MaxOrderNo = 64;
        public const int MaxSerialNo = 128;
        public const int MaxTitle = 64;
        public const int MaxHumanName = 64;
        public const int MaxUnitName = 16;
        public const int MaxUrl = 1024;
        public const int MaxProductName = 128;
        public const int MaxMobile = 32;
        public const int MaxTag = 32;
        public const int MaxEmail = 254;
        public const int MaxWeixinOpenId = 64;
        public const int MaxWeixinUnionId = 64;
        public const int MaxWeixinTemplateMessageId = 32;
        public const int MaxCaptcha = 6;
        public const int MaxDetailAddress = 256;
        /// <summary>
        /// Text version for price.
        /// </summary>
        public const int PriceText = 32;
    }
}
