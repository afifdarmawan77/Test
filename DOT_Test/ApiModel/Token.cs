namespace DOT_Test.ApiModel
{
    public class TokenResponse
    {
        public int ErrCode { get; set; }
        public string ErrMessage { get; set; }
        public Token Data { get; set; }
    }

    public class Token
    {
        public string Value { get; set; }
        public ApplicationUser User { get; set; }
        
    }
}
