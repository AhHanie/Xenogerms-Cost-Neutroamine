using System.Runtime.CompilerServices;
using Verse;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public class ModSettings: Verse.ModSettings
    {
        public static StrongBox<int> neutroaminePerComplexity = new StrongBox<int>(10);

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref neutroaminePerComplexity.Value, "neutroaminePerComplexity", 10);
        }
    }
}
