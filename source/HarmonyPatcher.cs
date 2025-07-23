using System.Reflection;
using HarmonyLib;
using RimWorld;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public class HarmonyPatcher
    {
        public static Harmony instance;
        public static void PatchVanillaMethods()
        {
            // Patch BiostatsTable.Draw method
            MethodInfo drawMethod = AccessTools.Method(typeof(BiostatsTable), "Draw");
            HarmonyMethod drawPrefixPatch = new HarmonyMethod(typeof(Patches).GetMethod("DrawBioStatsTablePrefix"));
            instance.Patch(drawMethod, drawPrefixPatch);

            // Patch Dialog_CreateXenogerm.CanAccept method
            MethodInfo canAcceptMethod = AccessTools.Method(typeof(Dialog_CreateXenogerm), "CanAccept");
            HarmonyMethod canAcceptPostfixPatch = new HarmonyMethod(typeof(Patches).GetMethod("CanAcceptPostfix"));
            instance.Patch(canAcceptMethod, null, canAcceptPostfixPatch);

            // Patch Building_GeneAssembler.Start method
            MethodInfo startMethod = AccessTools.Method(typeof(Building_GeneAssembler), "Start");
            HarmonyMethod startPostfixPatch = new HarmonyMethod(typeof(Patches).GetMethod("GeneAssemblerStartPostfix"));
            instance.Patch(startMethod, null, startPostfixPatch);

            // Patch Building_GeneAssembler.Finish method
            MethodInfo finishMethod = AccessTools.Method(typeof(Building_GeneAssembler), "Finish");
            HarmonyMethod finishPrefixPatch = new HarmonyMethod(typeof(Patches).GetMethod("GeneAssemblerFinishPrefix"));
            instance.Patch(finishMethod, finishPrefixPatch);

            // Less Destructive Patch For JobOnThing
            // Patch WorkGiver_CreateXenogerm.JobOnThing method
            // MethodInfo jobOnThingMethod = AccessTools.Method(typeof(WorkGiver_CreateXenogerm), "JobOnThing");
            // HarmonyMethod jobOnThingPostfixPatch = new HarmonyMethod(typeof(Patches).GetMethod("WorkGiverJobOnThingPostfix"));
            // instance.Patch(jobOnThingMethod, null, jobOnThingPostfixPatch);

            // Patch WorkGiver_CreateXenogerm.JobOnThing method
            MethodInfo jobOnThingMethod = AccessTools.Method(typeof(WorkGiver_CreateXenogerm), "JobOnThing");
            HarmonyMethod jobOnThingPrefixPatch = new HarmonyMethod(typeof(Patches).GetMethod("WorkGiverJobOnThingPrefix"));
            instance.Patch(jobOnThingMethod, jobOnThingPrefixPatch);

            // Patch WorkGiver_CreateXenogerm.HasJobOnThing method
            MethodInfo hasJobOnThingMethod = AccessTools.Method(typeof(WorkGiver_CreateXenogerm), "HasJobOnThing");
            HarmonyMethod hasJobOnThingPostfixPatch = new HarmonyMethod(typeof(Patches).GetMethod("WorkGiverHasJobOnThingPostfix"));
            instance.Patch(hasJobOnThingMethod, null, hasJobOnThingPostfixPatch);
        }
    }
}
