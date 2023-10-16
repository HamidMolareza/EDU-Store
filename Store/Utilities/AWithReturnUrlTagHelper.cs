using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Store.Utilities;

[HtmlTargetElement("a-return-url")]
public class AWithReturnUrlTagHelper : AnchorTagHelper {
    public AWithReturnUrlTagHelper(IHtmlGenerator generator) : base(generator) {
    }

    public PathString? RequestPath { get; set; }
    public QueryString? RequestQuery { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        output.TagName = "a";
        base.Process(context, output);

        var href = (string)output.Attributes.FirstOrDefault(attribute => attribute.Name == "href")?.Value!;
        href = href.Trim();

        if (string.IsNullOrEmpty(href) || href == "#") return;

        var returnUrl = Uri.EscapeDataString($"{RequestPath}{RequestQuery}");
        returnUrl = $"returnUrl={returnUrl}";

        href = href.Contains("?")
                   ? $"{href}&{returnUrl}"
                   : $"{href}?{returnUrl}";

        output.Attributes.SetAttribute("href", href);
    }
}