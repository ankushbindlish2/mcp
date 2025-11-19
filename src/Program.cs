using Azure.Identity;
using System.IdentityModel.Tokens.Jwt;

if(args.Length != 1)
{
    Console.WriteLine("Usage: dotnet run <scope>");
    Console.WriteLine("Example scopes:");
    Console.WriteLine("  https://vault.azure.net/.default");
    Console.WriteLine("  env:MY_ENV_VAR");
    return 1;
}

string scope = args[0];
string token;

if (scope.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
{
    var envVarName = scope.Substring(4);
    token = Environment.GetEnvironmentVariable(envVarName) ?? throw new InvalidOperationException($"{envVarName} environment variable is not set.");
}
else
{
    var credential = new DefaultAzureCredential();
    var tokenRequestContext = new Azure.Core.TokenRequestContext(new[] { scope });
    var response = await credential.GetTokenAsync(tokenRequestContext);
    token = response.Token;
}

var handler = new JwtSecurityTokenHandler();
JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

var now = DateTimeOffset.UtcNow;
var start = jwtToken.IssuedAt > jwtToken.ValidFrom ? jwtToken.IssuedAt : jwtToken.ValidFrom;
var end = jwtToken.ValidTo;

var tokenAge = now - start;
var remainingLife = end - now;
var lifespan = end - start;
Console.WriteLine($"Issued At: {jwtToken.IssuedAt}");
Console.WriteLine($"Valid From: {jwtToken.ValidFrom}");
Console.WriteLine($"Valid To: {jwtToken.ValidTo}");
Console.WriteLine();
Console.WriteLine($"Token Lifespan: {lifespan}");
Console.WriteLine($"Token Age: {tokenAge}");
Console.WriteLine($"Remaining Life: {remainingLife}");

return 0;
