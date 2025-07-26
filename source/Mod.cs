using HarmonyLib;
using UnityEngine;
using Verse;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public class XCNMod: Mod
    {
        public static ThingDef neutroamineDef;
        public static JobDef haulMultipleThingsToCotainnerDef;
        public XCNMod(ModContentPack content) : base(content)
        {
            Harmony instance = new Harmony("rimworld.sk.xenogermscostneutroamine");
            HarmonyPatcher.instance = instance;

             LongEventHandler.QueueLongEvent(InitAsync, "SK.Xenogerms_Cost_Neutroamine.InitAsync", true, null);
            LongEventHandler.QueueLongEvent(InitSync, "SK.Xenogerms_Cost_Neutroamine.InitSync", false, null);
        }

        public override string SettingsCategory()
        {
            return "Xenogerms Cost Neutroamine";
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            ModSettingsWindow.Draw(rect);
            base.DoSettingsWindowContents(rect);
        }

        public void InitAsync()
        {
            GetSettings<ModSettings>();
            ThingDef geneAssemblerDef = DefDatabase<ThingDef>.AllDefsListForReading.Find(def => def.defName == "GeneAssembler");
            CompProperties compProperties = new CompProperties { compClass = typeof(NeutroamineRequiredComp) };
            geneAssemblerDef.comps.Add(compProperties);
            neutroamineDef = DefDatabase<ThingDef>.AllDefsListForReading.Find(def => def.defName == "Neutroamine");
            haulMultipleThingsToCotainnerDef = DefDatabase<JobDef>.AllDefsListForReading.Find(def => def.defName == "SK_XCN_HaulMultipleToContainer");
            HarmonyPatcher.PatchVanillaMethodsAsync();
        }

        public void InitSync()
        {
            HarmonyPatcher.PatchVanillaMethodsSync();   
        }
    }
}
