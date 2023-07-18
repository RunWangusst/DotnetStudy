namespace webapidemo.Models
{
    public class TokenParameter
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public int AccessExpiration { get; set; }
        public int RefreshExpiration { get; set; }

    }
}
