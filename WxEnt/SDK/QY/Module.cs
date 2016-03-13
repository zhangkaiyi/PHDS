using System;
using System.Collections.Generic;
using System.Web;

namespace PHDS.Weixin.QY
{
    public class Module
    {
        public static string Token { get; set; }
        public static string Corpid { get; set; }
        public static string EncodingAESKey { get; set; }
        public static string Secret { get; set; }
        public static string Msg_Signature { get; set; }
        public static string Timestamp { get; set; }
        public static string Nonce { get; set; }
    }
}