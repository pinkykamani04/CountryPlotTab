// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Components.LineWidget
{
    public partial class LineWidget<TItem>
    {
        [Parameter] public string Title { get; set; } = "Line Chart";

        [Parameter] public IEnumerable<TItem> Data { get; set; }

        [Parameter] public string Category { get; set; } = "Category";

        [Parameter] public string Value { get; set; } = "Value";

        [Parameter] public string Format { get; set; } = "{0}";

        [Parameter] public bool ShowLegend { get; set; } = false;

        private List<LegendItem> LegendItems =>
      (Data ?? Enumerable.Empty<TItem>())
          .Select((item, index) => new LegendItem
          {
              Label = item?
                  .GetType()
                  .GetProperty(Category)?
                  .GetValue(item)?
                  .ToString() ?? string.Empty,
              Color = GetColor(index)
          })
          .ToList();

        private static string GetColor(int index) => index switch
        {
            0 => "#0B74DE",
            1 => "#6AD4C8",
            2 => "#FF7A45",
            3 => "#7B61FF",
            _ => "#9CA3AF",
        };

        private sealed class LegendItem
        {
            public string Label { get; set; } = "";

            public string Color { get; set; } = "";
        }
    }
}
