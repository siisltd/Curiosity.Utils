using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Curiosity.Tools.Web.TagHelpers
{
    [HtmlTargetElement("input", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class InvariantDecimalTagHelper : InputTagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string InvariantAttributeName = "asp-is-invariant";

        [HtmlAttributeName(InvariantAttributeName)]
        public bool IsInvariant { set; get; } = false;

        public InvariantDecimalTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (IsInvariant && output.TagName == "input" && For.Model != null && For.Model is decimal value)
            {
                var invariantValue = value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                output.Attributes.SetAttribute(new TagHelperAttribute("value", invariantValue));                
            }
        }
    }
}