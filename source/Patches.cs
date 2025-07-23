using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public class Patches
    {
        private static BiostatData[] cachedExtendedBiostats;

        private static BiostatData[] GetExtendedBiostats(BiostatData[] originalBiostats)
        {
            if (cachedExtendedBiostats == null)
            {
                cachedExtendedBiostats = new BiostatData[originalBiostats.Length + 1];

                // Complexity
                cachedExtendedBiostats[0] = originalBiostats[0];
                // Metabolism
                cachedExtendedBiostats[1] = originalBiostats[1];
                // Neutroamine
                cachedExtendedBiostats[2] = new BiostatData(
                    "SK.XCN.Neutroamine.label",
                    "SK.XCN.Neutroamine.description",
                    XCNMod.neutroamineDef.uiIcon,
                    false
                );
                // Archite Capsules
                cachedExtendedBiostats[3] = originalBiostats[2];
            }

            return cachedExtendedBiostats;
        }

        public static bool DrawBioStatsTablePrefix(Rect rect, int gcx, int met, int arc, bool drawMax, bool ignoreLimits, BiostatData[] ___Biostats, Dictionary<string, string> ___truncateCache, ref float ___cachedWidth, int maxGCX = -1)
        {
            BiostatData[] extendedBiostats = GetExtendedBiostats(___Biostats);

            DrawExtendedBioStatsTable(rect, gcx, met, arc, gcx * ModSettings.neutroaminePerComplexity.Value, drawMax, ignoreLimits, extendedBiostats, ___truncateCache, ref ___cachedWidth, maxGCX);

            return false;
        }

        private static void DrawExtendedBioStatsTable(Rect rect, int gcx, int met, int arc, int neutroamine, bool drawMax, bool ignoreLimits, BiostatData[] biostats, Dictionary<string, string> truncateCache, ref float cachedWidth, int maxGCX = -1)
        {
            int num = ((arc > 0) ? biostats.Length : (biostats.Length - 1));
            Log.Message($"Drawing Table: {num}");
            float num2 = Utils.MaxLabelWidth(arc, biostats);
            float num3 = rect.height / (float)num;

            GUI.BeginGroup(rect);

            for (int i = 0; i < num; i++)
            {
                Rect position = new Rect(0f, (float)i * num3 + (num3 - 22f) / 2f, 22f, 22f);
                Rect rect2 = new Rect(position.xMax + 4f, (float)i * num3, num2, num3);
                Rect rect3 = new Rect(0f, rect2.y, rect.width, rect2.height);

                if (i % 2 == 1)
                {
                    Widgets.DrawLightHighlight(rect3);
                }
                Widgets.DrawHighlightIfMouseover(rect3);
                rect3.xMax = rect2.xMax + 4f + 90f;

                TaggedString taggedString = biostats[i].descKey.Translate();
                if (maxGCX >= 0 && biostats[i].displayMaxGCXInfo)
                {
                    taggedString += "\n\n" + "MaxComplexityDesc".Translate();
                }
                TooltipHandler.TipRegion(rect3, taggedString);
                GUI.DrawTexture(position, biostats[i].icon);

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(rect2, biostats[i].labelKey.Translate().CapitalizeFirst());
                Text.Anchor = TextAnchor.UpperLeft;
            }

            float num4 = num2 + 4f + 22f + 4f;

            string complexityText = gcx.ToString();
            string metabolismText = met.ToStringWithSign();
            string neutroamineText = neutroamine.ToString();

            if (drawMax && !ignoreLimits)
            {
                if (maxGCX >= 0)
                {
                    if (gcx > maxGCX)
                    {
                        complexityText = complexityText.Colorize(ColorLibrary.RedReadable);
                    }
                    complexityText = complexityText + " / " + maxGCX;
                }
                if (met < GeneTuning.BiostatRange.TrueMin)
                {
                    metabolismText = string.Concat(metabolismText, " (" + "min".Translate() + " ", GeneTuning.BiostatRange.TrueMin.ToString(), ")");
                    metabolismText = metabolismText.Colorize(ColorLibrary.RedReadable);
                }
            }

            Text.Anchor = TextAnchor.MiddleCenter;

            int currentRow = 0;

            // Complexity
            Widgets.Label(new Rect(num4, currentRow * num3, 90f, num3), complexityText);
            currentRow++;

            // Metabolism
            Widgets.Label(new Rect(num4, currentRow * num3, 90f, num3), metabolismText);
            currentRow++;

            // Neutroamine
            Widgets.Label(new Rect(num4, currentRow * num3, 90f, num3), neutroamineText);
            currentRow++;

            // Archites (only if > 0)
            if (arc > 0)
            {
                Widgets.Label(new Rect(num4, currentRow * num3, 90f, num3), arc.ToString());
                
            }
            
            Text.Anchor = TextAnchor.MiddleLeft;

            float width = rect.width - num2 - 90f - 22f - 4f;
            Rect rect4 = new Rect(num4 + 90f + 4f, num3, width, num3);
            if (rect4.width != cachedWidth)
            {
                cachedWidth = rect4.width;
                truncateCache.Clear();
            }
            string text3 = Utils.MetabolismDescAt(met);
            Widgets.Label(rect4, text3.Truncate(rect4.width, truncateCache));
            if (Mouse.IsOver(rect4) && !text3.NullOrEmpty())
            {
                TooltipHandler.TipRegion(rect4, text3);
            }

            Text.Anchor = TextAnchor.UpperLeft;
            GUI.EndGroup();
        }

        public static void CanAcceptPostfix(ref bool __result, int ___gcx, Building_GeneAssembler ___geneAssembler)
        {
           if (!Utils.ColonyHasEnoughNeutroamine(___gcx * ModSettings.neutroaminePerComplexity.Value, ___geneAssembler))
           {
                Messages.Message("SK.XCN.NotEnoughNeutroamine".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
                __result = false;
           }
        }

        public static void GeneAssemblerStartPostfix(Building_GeneAssembler __instance, List<Genepack> ___genepacksToRecombine)
        {
            NeutroamineRequiredComp comp = __instance.TryGetComp<NeutroamineRequiredComp>();
            comp.NeutroamineRequired = Utils.CalculateComplexity(___genepacksToRecombine) * ModSettings.neutroaminePerComplexity.Value;
        }

        public static void GeneAssemblerFinishPrefix(Building_GeneAssembler __instance)
        {
            NeutroamineRequiredComp comp = __instance.TryGetComp<NeutroamineRequiredComp>();
            for (int num = __instance.innerContainer.Count - 1; num >= 0; num--)
            {
                if (__instance.innerContainer[num].def == XCNMod.neutroamineDef)
                {
                    Thing thing = __instance.innerContainer[num].SplitOff(Mathf.Min(__instance.innerContainer[num].stackCount, comp.NeutroamineRequired));
                    comp.NeutroamineRequired -= thing.stackCount;
                    thing.Destroy();
                    if (comp.NeutroamineRequired <= 0)
                    {
                        break;
                    }
                }
            }
            comp.NeutroamineRequired = 0;
        }

        // Less Destructive Patching
        //public static void WorkGiverJobOnThingPostfix(ref Job __result, Pawn pawn, Thing t)
        //{
        //    if (__result != null && __result.def == JobDefOf.CreateXenogerm)
        //    {
        //        NeutroamineRequiredComp comp = t.TryGetComp<NeutroamineRequiredComp>();
        //        int neutroamineRequiredNow = Utils.NeutroamineRequiredNow(comp.NeutroamineRequired, t as Building_GeneAssembler);
        //        if (neutroamineRequiredNow > 0)
        //        {
        //            Thing thing = Utils.FindNeutroamine(pawn);
        //            if (thing != null)
        //            {
        //                Job job = JobMaker.MakeJob(JobDefOf.HaulToContainer, thing, t);
        //                job.count = Mathf.Min(neutroamineRequiredNow, thing.stackCount);
        //                __result = job;
        //            }
        //        }
        //    }
        //}

        public static void WorkGiverHasJobOnThingPostfix(ref bool __result, Pawn pawn, Thing t)
        {
            if (__result == true)
            {
                NeutroamineRequiredComp comp = t.TryGetComp<NeutroamineRequiredComp>();
                int neutroamineRequiredNow = Utils.NeutroamineRequiredNow(comp.NeutroamineRequired, t as Building_GeneAssembler);
                if (neutroamineRequiredNow < 1)
                {
                    return;
                }
                int totolNeutroamineOnMap = 0;
                List<Thing> neutroamineThings = pawn.Map.listerThings.ThingsOfDef(XCNMod.neutroamineDef);
                for (int i = 0; i < neutroamineThings.Count; i++)
                {
                    totolNeutroamineOnMap += neutroamineThings[i].stackCount;
                }
                if (totolNeutroamineOnMap < neutroamineRequiredNow)
                {
                    __result = false;
                }
                if (!Utils.CanReachEnoughNeutroamine(pawn, neutroamineRequiredNow, neutroamineThings))
                {
                    __result = false;
                }
            }
        }

        public static bool WorkGiverJobOnThingPrefix(Pawn pawn, Thing t, ref Job __result)
        {
            if (!(t is Building_GeneAssembler building_GeneAssembler))
            {
                return false;
            }

            NeutroamineRequiredComp comp = t.TryGetComp<NeutroamineRequiredComp>();
            int neutroamineRequiredNow = Utils.NeutroamineRequiredNow(comp.NeutroamineRequired, t as Building_GeneAssembler);
            List<LocalTargetInfo> targetQueue = new List<LocalTargetInfo>();
            List<int> countQueue = new List<int>();

            if (neutroamineRequiredNow > 0)
            {
                int neutroamineRequired = neutroamineRequiredNow;
                List<Thing> closestNeutroamineThings = Utils.GetClosestNeuotramineThings(pawn, neutroamineRequiredNow);
                if (closestNeutroamineThings == null)
                {
                    __result = null;
                    return false;
                }
                for (int i = 0;i< closestNeutroamineThings.Count;i++)
                {
                    targetQueue.Add(closestNeutroamineThings[i]);
                    countQueue.Add(Mathf.Min(closestNeutroamineThings[i].stackCount, neutroamineRequired));
                    neutroamineRequired -= closestNeutroamineThings[i].stackCount;
                }
            }

            if (building_GeneAssembler.ArchitesRequiredNow > 0)
            {
                Thing thing = Utils.FindArchiteCapsule(pawn);
                if (thing != null)
                {
                    targetQueue.Add(thing);
                    countQueue.Add(Mathf.Min(thing.stackCount, building_GeneAssembler.ArchitesRequiredNow));
                }
            }

            if (targetQueue.Count > 1)
            {
                // Multiple ingredients - use our custom job
                Job job = JobMaker.MakeJob(XCNMod.haulMultipleThingsToCotainnerDef, t);
                job.targetQueueB = targetQueue;
                job.countQueue = countQueue;
                __result = job;
                return false;
            }
            else if (targetQueue.Count == 1)
            {
                // Single ingredient - use vanilla hauling
                Job job = JobMaker.MakeJob(JobDefOf.HaulToContainer, targetQueue[0], t);
                job.count = countQueue[0];
                __result = job;
                return false;
            }

            __result =  JobMaker.MakeJob(JobDefOf.CreateXenogerm, t, 1200, checkOverrideOnExpiry: true);
            return false;
        }
    }
}