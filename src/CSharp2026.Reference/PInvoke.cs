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

namespace CSharp2026.Reference;

#pragma warning disable
// Reference/PInvoke.cs
// From: C# 2026: Enterprise Mastery by Victor Mihailov
// https://github.com/MrMeHighLove/CSharp2026-Enterprise-Mastery
// Validated against the .NET 10 SDK.

// Reference/PInvoke.cs
// Modern P/Invoke with LibraryImport (source-generated, AOT-compatible)
// Replaces DllImport for .NET 7+

[LibraryImport("libsodium", EntryPoint = "crypto_secretbox_easy")]
[UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
private static partial int CryptoSecretboxEasy(
    Span<byte> ciphertext,
    ReadOnlySpan<byte> message,
    long messageLen,
    ReadOnlySpan<byte> nonce,
    ReadOnlySpan<byte> key);

// Safer wrapper with proper error handling
public static byte[] Encrypt(ReadOnlySpan<byte> message, ReadOnlySpan<byte> key)
{
    Span<byte> nonce = stackalloc byte[24];
    RandomNumberGenerator.Fill(nonce);

    var ciphertext = new byte[message.Length + 16 + nonce.Length];
    nonce.CopyTo(ciphertext);

    int result = CryptoSecretboxEasy(
        ciphertext.AsSpan(nonce.Length),
        message, message.Length, nonce, key);

    if (result != 0) throw new CryptographicException("Encryption failed");
    return ciphertext;
}

// For high-performance interop: use unsafe code + fixed to avoid marshalling
public static unsafe void FastMemCopy(
    ReadOnlySpan<byte> source, Span<byte> destination)
{
    fixed (byte* src = source)
    fixed (byte* dst = destination)
    {
        Buffer.MemoryCopy(src, dst, destination.Length, source.Length);
    }
}

#pragma warning restore
