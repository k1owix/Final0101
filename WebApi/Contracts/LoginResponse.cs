namespace WebApi.Contracts;
 
public sealed record LoginResponse(int UserId, string FullName, string RoleName);
