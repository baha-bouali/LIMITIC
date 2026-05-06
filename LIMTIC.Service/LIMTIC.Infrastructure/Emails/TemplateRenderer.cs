using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using LIMTIC.Application.Abstractions.Email;

namespace LIMTIC.Infrastructure.Emails
{
    public class TemplateRenderer : ITemplateRenderer
    {
        private readonly string _templatesPath;
        private readonly ConcurrentDictionary<string, string> _cache = new();

        public TemplateRenderer()
        {
            _templatesPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Emails", "Templates"
            );
        }

        public string Render<TModel>(string templateName, TModel model) where TModel : class
        {
            // Load from cache or disk
            var html = _cache.GetOrAdd(templateName, name =>
            {
                var path = Path.Combine(_templatesPath, $"{name}.html");

                if (!File.Exists(path))
                    throw new FileNotFoundException();

                return File.ReadAllText(path);
            });

            // Replace {{PropertyName}} placeholders
            foreach (var prop in typeof(TModel).GetProperties())
            {
                var placeholder = $"{{{{{prop.Name}}}}}";
                var value = prop.GetValue(model)?.ToString() ?? string.Empty;
                html = html.Replace(placeholder, value);
            }

            // Guard: catch any leftover unresolved placeholders
            var unresolved = Regex.Matches(html, @"\{\{(.+?)\}\}");
            if (unresolved.Count > 0)
            {
                var names = string.Join(", ", unresolved
                    .Select(m => m.Groups[1].Value)
                    .Distinct());

                throw new InvalidOperationException(
                    $"Template '{templateName}' has unresolved placeholders: {names}. " +
                    $"Make sure your model exposes matching properties.");
            }

            return html;
        }
    }
}