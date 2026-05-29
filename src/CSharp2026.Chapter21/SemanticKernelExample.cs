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

namespace CSharp2026.Chapter21;

#pragma warning disable
// Chapter21/SemanticKernelExample.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Chapter21/SemanticKernelExample.cs
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion("gpt-4o", endpoint, apiKey)
    .Build();

// Define a plugin from a class
kernel.Plugins.AddFromType<OrderPlugin>("Orders");

// Invoke a function directly
var result = await kernel.InvokeAsync("Orders", "GetStatus",
    new KernelArguments { ["orderId"] = "ORD-1234" });
Console.WriteLine(result);

// Use the chat service with auto function invocation
var chatService = kernel.GetRequiredService<IChatCompletionService>();
var settings = new OpenAIPromptExecutionSettings
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
};
var chat = new ChatHistory("You are an order assistant.");
chat.AddUserMessage("What is the status of order ORD-1234?");
var response = await chatService.GetChatMessageContentAsync(chat, settings, kernel);
Console.WriteLine(response.Content);

// Chapter21/OrderPlugin.cs
public class OrderPlugin
{
    private readonly IOrderRepository _repo;
    public OrderPlugin(IOrderRepository repo) => _repo = repo;

    [KernelFunction, Description("Gets the status of a specific order")]
    public async Task<string> GetStatus([Description("The order ID")] string orderId)
    {
        var order = await _repo.GetByIdAsync(orderId);
        return order is null ? "Order not found" : $"Status: {order.Status}";
    }
}

#pragma warning restore
