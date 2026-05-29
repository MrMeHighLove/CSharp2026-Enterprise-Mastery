// AUTO-WRAPPED for compilation. Original snippet content follows the
// namespace declaration. Snippets are illustrative and may reference
// types that need to be supplied by the chapter or by the reader.
// See ISSUES.md for the catalog of known gaps.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSharp2026.Common.Domain;
using CSharp2026.Common.Events;
using CSharp2026.Common.Results;
using CSharp2026.Common.ValueObjects;

namespace CSharp2026.Supplement;

#pragma warning disable
// Supplement/SecretsManagement.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Supplement/SecretsManagement.cs
// 1. Local development: use dotnet user-secrets
// dotnet user-secrets set "Database:Password" "devpassword"

// 2. Production: Azure Key Vault with Managed Identity
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net/"),
    new DefaultAzureCredential()); // Uses Managed Identity in Azure, dev credentials locally

// 3. Kubernetes: mount secrets as environment variables, never ConfigMaps
// kubectl create secret generic db-creds --from-literal=password=secret
// In deployment.yaml:
// env:
// - name: DATABASE__PASSWORD
//   valueFrom:
//     secretKeyRef:
//       name: db-creds
//       key: password

// 4. Connection string builder — construct from separate secret pieces
public class DatabaseConnectionFactory
{
    private readonly IConfiguration _config;

    public string BuildConnectionString() =>
        new NpgsqlConnectionStringBuilder
        {
            Host = _config["Database:Host"],
            Database = _config["Database:Name"],
            Username = _config["Database:Username"],
            Password = _config["Database:Password"], // From Key Vault or secret
            SslMode = SslMode.Require,
            TrustServerCertificate = false
        }.ConnectionString;
}

#pragma warning restore
