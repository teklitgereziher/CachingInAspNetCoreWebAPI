using System.Data.Common;
using Azure.Core;
using Azure.Identity;
using Caching.DataContract.ConfigOptions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace Caching.DataContract.DbaseContext
{
  public class DbConnInterceptor : DbConnectionInterceptor
  {
    private readonly PostgresSettings postgresSettings;
    private readonly ClientSecretCredential clientSecretCredential;
    public DbConnInterceptor(
      IOptions<PostgresSettings> postgresSettings,
      ClientSecretCredential clientSecretCredential)
    {
      this.postgresSettings = postgresSettings.Value;
      this.clientSecretCredential = clientSecretCredential;
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
      DbConnection connection,
      ConnectionEventData eventData,
      InterceptionResult result,
      CancellationToken cancellationToken = default)
    {
      AccessToken accessToken = await GetAccessTokenAsync();
      connection.ConnectionString = postgresSettings.ConnectionString;
      var npgsqlConnection = (Npgsql.NpgsqlConnection)connection;
      npgsqlConnection.ConnectionString += accessToken.Token;

      return result;
    }

    private async Task<AccessToken> GetAccessTokenAsync()
    {
      var accessToken = await clientSecretCredential.GetTokenAsync(
        new TokenRequestContext(postgresSettings.Scopes),
        CancellationToken.None);

      return accessToken;
    }
  }
}
