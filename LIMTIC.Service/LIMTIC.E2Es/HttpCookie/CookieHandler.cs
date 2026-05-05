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

            Console.WriteLine($"=== REQUEST: {request.Method} {request.RequestUri}");
            Console.WriteLine($"=== BASE URI: {baseUri}");

            var allCookies = _cookieContainer.GetCookies(baseUri);
            Console.WriteLine($"=== COOKIES IN CONTAINER: {allCookies.Count}");
            foreach (Cookie c in allCookies)
                Console.WriteLine($"    - {c.Name} = {c.Value}");

            // Add stored cookies to the request
            var cookieHeader = _cookieContainer.GetCookieHeader(baseUri);
            Console.WriteLine($"=== COOKIE HEADER: {cookieHeader}");
            if (!string.IsNullOrEmpty(cookieHeader))
                request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);

            var response = await base.SendAsync(request, cancellationToken);

            // Store cookies from the response
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                Console.WriteLine($"=== SET-COOKIE HEADERS FOUND:");
                foreach (var cookie in cookies)
                {
                    Console.WriteLine($"    - {cookie}");
                    _cookieContainer.SetCookies(baseUri, cookie);
                }
            }
            else
            {
                Console.WriteLine("=== NO SET-COOKIE HEADERS IN RESPONSE");
            }

                return response;
        }
    }
}
