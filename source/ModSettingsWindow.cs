using UnityEngine;
using Verse;
// LessUI is a UI library under development and will be released at a later date
using LessUI;
using System.Runtime.CompilerServices;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public static class ModSettingsWindow
    {
        public static StrongBox<string> neutroaminePerComplexityLabelRef = new StrongBox<string>("SK.XCN.ModSettingsNeutroaminePerComplexity".Translate());
        public static void Draw(Rect parent)
        {
            Canvas canvas = new Canvas(parent);
            FillGrid grid = new FillGrid(2, 1)
            {
                Padding = 20f
            };
            Text.Font = GameFont.Medium;

            Label neutroaminePerComplexityLabel = new Label(neutroaminePerComplexityLabelRef);

            grid.AddChild(neutroaminePerComplexityLabel);

            LabeledSlider neutroaminePerComplexitySlider = new LabeledSlider(ModSettings.neutroaminePerComplexity.Value.ToString(), ModSettings.neutroaminePerComplexity.Value, 0f, 200f, SizeMode.Fill,(val) => ModSettings.neutroaminePerComplexity.Value = (int)val);

            grid.AddChild(neutroaminePerComplexitySlider);

            canvas.AddChild(grid);
            canvas.Render();
        }
    }
}
