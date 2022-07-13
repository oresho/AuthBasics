using System;
namespace Server
{
    public static class Constants
    {
        public const string Audience = "http://localhost:48432";
        public const string Issuer = Audience;
        public const string Secret = "not_too_short_secret_otherwise_it_might_error";

    }
}
