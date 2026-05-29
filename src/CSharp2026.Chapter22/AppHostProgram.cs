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

namespace CSharp2026.Chapter22;

#pragma warning disable
// Chapter22/AppHostProgram.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter22/AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure resources — Aspire provisions these in containers locally
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var redis = builder.AddRedis("redis")
    .WithRedisInsight();

var rabbit = builder.AddRabbitMQ("messaging");

// Application services with dependencies wired automatically
var catalog = builder.AddProject<Projects.CatalogService>("catalog")
    .WithReference(postgres)
    .WithReference(redis);

var orders = builder.AddProject<Projects.OrderService>("orders")
    .WithReference(postgres)
    .WithReference(rabbit)
    .WithReference(catalog);

builder.AddProject<Projects.ApiGateway>("gateway")
    .WithReference(catalog)
    .WithReference(orders)
    .WithExternalHttpEndpoints();

builder.Build().Run();

#pragma warning restore
