using System.Net;

namespace LIMTIC.E2Es.HttpCookie
{
    public class CookieHandler : DelegatingHandler
    {
        private readonly CookieContainer _cookieContainer;

        public CookieHandler(CookieContainer cookieContainer)
        {
            _cookieContainer = cookieContainer;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var baseUri = new Uri(request.RequestUri!.GetLeftPart(UriPartial.Authority));

            // Add stored cookies to the request
            var cookieHeader = _cookieContainer.GetCookieHeader(baseUri);
            if (!string.IsNullOrEmpty(cookieHeader))
                request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);

            var response = await base.SendAsync(request, cancellationToken);

            // Store cookies from the response
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                    _cookieContainer.SetCookies(baseUri, cookie);
            }
            
            return response;
        }
    }
}
