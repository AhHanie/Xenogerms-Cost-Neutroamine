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
        public static StrongBox<string> hardmodeLabelRef = new StrongBox<string>("SK.XCN.ModSettingsHardmode".Translate());
        public static StrongBox<string> architeMultiplierLabelRef = new StrongBox<string>("SK.XCN.ModSettingsArchiteGenesMultiplier".Translate());
        public static void Draw(Rect parent)
        {
            Canvas canvas = new Canvas(parent);
            FillGrid grid = new FillGrid(2, 3)
            {
                Padding = 20f
            };
            Text.Font = GameFont.Medium;

            Label neutroaminePerComplexityLabel = new Label(neutroaminePerComplexityLabelRef);
            neutroaminePerComplexityLabel.Alignment = Align.MiddleLeft;

            grid.AddChild(neutroaminePerComplexityLabel);

            LabeledSlider neutroaminePerComplexitySlider = new LabeledSlider(ModSettings.neutroaminePerComplexity.Value.ToString(), ModSettings.neutroaminePerComplexity.Value, 0f, 200f, SizeMode.Fill,(val) => ModSettings.neutroaminePerComplexity.Value = (int)val);
            neutroaminePerComplexitySlider.Alignment = Align.MiddleLeft;

            grid.AddChild(neutroaminePerComplexitySlider);

            Label hardmodeLabel = new Label(hardmodeLabelRef)
            {
                Tooltip = "ModSettingsHardmodeTooltip".Translate()
            };
            hardmodeLabel.Alignment = Align.MiddleLeft;

            grid.AddChild(hardmodeLabel);

            Checkbox hardmodeCheckbox = new Checkbox(ModSettings.hardmode);
            hardmodeCheckbox.Alignment = Align.MiddleLeft;

            grid.AddChild(hardmodeCheckbox);

            if (ModSettings.hardmode.Value)
            {
                Label architeMultiplierLabel = new Label(architeMultiplierLabelRef);
                architeMultiplierLabel.Alignment = Align.MiddleLeft;

                grid.AddChild(architeMultiplierLabel);

                LabeledSlider architeMultiplierSlider = new LabeledSlider(ModSettings.architeGenesMultiplier.Value.ToString(), ModSettings.architeGenesMultiplier.Value, 2f, 10f, SizeMode.Fill, (val) => ModSettings.architeGenesMultiplier.Value = (int)val);
                architeMultiplierSlider.RoundTo = 1f;
                architeMultiplierSlider.Alignment = Align.MiddleLeft;


                grid.AddChild(architeMultiplierSlider);
            }

            canvas.AddChild(grid);
            canvas.Render();
        }
    }
}
