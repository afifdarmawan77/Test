using Azure;
using System.Security.Claims;

namespace DOT_Test.Libraries
{
    public interface IHelper
    {
        Task<string> GetRoleUser(ClaimsPrincipal User);
        ApplicationUser GetUser(ClaimsPrincipal User);
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal User);
        string QueryProtect(string QueryString);
        string QueryUnprotect(string QueryString);
        string QueryEncode(string QueryString);
        string QueryDecode(string EncodedQueryString);
        string SerializeJson(dynamic data);
        dynamic DeserializeJson(string stringJson);
        string DatetimeConverter(DateTime date);
        string Nl2Br(string src);


        
    }
}
