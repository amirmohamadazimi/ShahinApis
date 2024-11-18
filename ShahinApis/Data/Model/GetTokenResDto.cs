namespace ShahinApis.Data.Model;

public class GetTokenResDto
{
    public string Access_Token { get; init; }
    public string token_type { get; init; }
    public int expires_in { get; init; }
    public List<string> scope { get; init; }
    public string bank { get; init; }
    public List<string> accounts { get; init; }
    public int amount { get; init; }
    public string jti { get; init; }
    public string user_name { get; init; }
    public long iat { get; init; }
    public long nbf { get; init; }
    public string iss { get; init; }
}
