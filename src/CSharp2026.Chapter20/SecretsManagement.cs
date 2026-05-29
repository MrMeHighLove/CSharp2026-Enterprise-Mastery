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

namespace CSharp2026.Chapter20;

#pragma warning disable
// Chapter20/SecretsManagement.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// AVOID: NEVER: secrets in source code or appsettings.json
var connStr = "Server=proddb01;Password=MyS3cr3tP@ssword!;";

// GOOD: Development: .NET User Secrets (never committed to source control)
// dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;..."
builder.Configuration.AddUserSecrets<Program>();

// GOOD: Production: Azure Key Vault (or AWS Secrets Manager, HashiCorp Vault)
if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
}

// GOOD: Container environments: environment variables injected by Kubernetes secrets
// The appsettings.json approach is a last resort with encrypted values only.

// Data protection: encrypt sensitive data at rest
builder.Services.AddDataProtection()
    .PersistKeysToAzureBlobStorage(blobUri)   // keys stored externally, not in process
    .ProtectKeysWithAzureKeyVault(keyIdentifier, new DefaultAzureCredential())
    .SetApplicationName("order-service")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

#pragma warning restore
