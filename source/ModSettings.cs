using System.Runtime.CompilerServices;
using Verse;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public class ModSettings: Verse.ModSettings
    {
        public static StrongBox<int> neutroaminePerComplexity = new StrongBox<int>(10);
        public static StrongBox<bool> hardmode = new StrongBox<bool>(false);
        public static StrongBox<int> architeGenesMultiplier = new StrongBox<int>(2);
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref neutroaminePerComplexity.Value, "neutroaminePerComplexity", 10);
            Scribe_Values.Look(ref hardmode.Value, "hardmode", false);
            Scribe_Values.Look(ref architeGenesMultiplier.Value, "architeGenesMultiplier", 2);
        }
    }
}
