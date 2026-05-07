namespace LIMTIC.Application.Abstractions.Email
{
    public interface ITemplateRenderer
    {
        string Render<TModel>(string templateName, TModel model) where TModel : class;
    }
}
