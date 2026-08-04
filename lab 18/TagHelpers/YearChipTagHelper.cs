using Microsoft.AspNetCore.Razor.TagHelpers;

namespace lab_18.TagHelpers
{
    [HtmlTargetElement("year-chip", TagStructure = TagStructure.WithoutEndTag)]
    public class YearChipTagHelper : TagHelper
    {
        private const int ChipYear = 4;        // (27 mod 4) + 1, Lab ID 27
        private const string ChipLabel = "Final";

        public int For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            string cssClass;
            string text;

            if (For == ChipYear)
            {
                cssClass = "bg-warning text-dark";
                text = ChipLabel;
            }
            else
            {
                cssClass = "bg-light text-dark";
                text = $"Year {For}";
            }

            output.Attributes.SetAttribute("class", $"badge {cssClass}");
            output.Attributes.SetAttribute("title", "rendered by muhamad Assem");
            output.Content.SetContent(text);
        }
    }
}