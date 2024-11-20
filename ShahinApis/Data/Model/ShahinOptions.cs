namespace ShahinApis.Data.Model
{
    public class ShahinOptions
    {
        public const string SectionName = "ShahinOptions";
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string GrantType { get; set; }
        public required string Bank { get; set; }
        public required string ShahinUriService { get; set; }
        public required string ShahinUriToken { get; set; }
        public required string RequestSignature { get; set; }
    }
}
