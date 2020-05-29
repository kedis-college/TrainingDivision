using System;
using System.Linq.Expressions;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System.Linq;

namespace TrainingDivisionKedis.Extensions.Helpers
{
    public static class MyHTMLHelpers
    {
        public static IHtmlContent RequiredLabelFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var modelExplorer = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = modelExplorer.Metadata.DisplayName ?? modelExplorer.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            if (modelExplorer.Metadata.IsRequired)
                labelText += "<span style=\"color: red\">&nbsp;*</span>";

            if (string.IsNullOrEmpty(labelText))
                return HtmlString.Empty;

            var label = new TagBuilder("label");
            label.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName));
            label.InnerHtml.AppendHtml(labelText);
            
            return label;
        }
    }
}
