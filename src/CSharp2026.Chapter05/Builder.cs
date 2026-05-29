// -----------------------------------------------------------------------
//   Builder.cs — Chapter 5 / Builder Pattern
//
//   Builds an immutable HttpRequestOptions value. The Headers property is
//   exposed as IReadOnlyDictionary backed by a ReadOnlyDictionary wrapper
//   — Dictionary<TKey,TValue> has no AsReadOnly() and no .Empty static, so
//   we construct the empty wrapper explicitly (the book's compile-bug
//   fix).
// -----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace CSharp2026.Chapter05.Builder;

public sealed class HttpRequestOptions
{
    private HttpRequestOptions() { }

    private static readonly IReadOnlyDictionary<string, string> EmptyHeaders =
        new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

    public required Uri      BaseUri        { get; init; }
    public TimeSpan          Timeout        { get; init; } = TimeSpan.FromSeconds(30);
    public int               MaxRetries     { get; init; } = 3;
    public bool              FollowRedirects{ get; init; } = true;
    public IReadOnlyDictionary<string, string> Headers { get; init; } = EmptyHeaders;

    public sealed class HttpRequestOptionsBuilder
    {
        private Uri?     _baseUri;
        private TimeSpan _timeout         = TimeSpan.FromSeconds(30);
        private int      _maxRetries      = 3;
        private bool     _followRedirects = true;
        private readonly Dictionary<string, string> _headers = new(StringComparer.OrdinalIgnoreCase);

        public HttpRequestOptionsBuilder WithBaseUri(Uri baseUri)
        {
            ArgumentNullException.ThrowIfNull(baseUri);
            _baseUri = baseUri;
            return this;
        }

        public HttpRequestOptionsBuilder WithTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            return this;
        }

        public HttpRequestOptionsBuilder WithMaxRetries(int retries)
        {
            if (retries < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retries), retries, "Retries must be non-negative.");
            }
            _maxRetries = retries;
            return this;
        }

        public HttpRequestOptionsBuilder WithFollowRedirects(bool follow)
        {
            _followRedirects = follow;
            return this;
        }

        public HttpRequestOptionsBuilder WithHeader(string key, string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            _headers[key] = value;
            return this;
        }

        public HttpRequestOptions Build()
        {
            if (_baseUri is null)
            {
                throw new InvalidOperationException("BaseUri is required before Build().");
            }
            return new HttpRequestOptions
            {
                BaseUri         = _baseUri,
                Timeout         = _timeout,
                MaxRetries      = _maxRetries,
                FollowRedirects = _followRedirects,
                // Dictionary<,> has no AsReadOnly() — wrap explicitly.
                Headers         = new ReadOnlyDictionary<string, string>(
                                     new Dictionary<string, string>(_headers, StringComparer.OrdinalIgnoreCase)),
            };
        }
    }

    public static HttpRequestOptionsBuilder Builder() => new();
}
