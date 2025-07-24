using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public class Utils
    {
        public static float MaxLabelWidth(int arc, BiostatData[] biostats)
        {
            float num = 0f;
            int num2 = ((arc > 0) ? biostats.Length : (biostats.Length - 1));
            for (int i = 0; i < num2; i++)
            {
                num = Mathf.Max(num, Text.CalcSize(biostats[i].labelKey.Translate().CapitalizeFirst()).x);
            }
            return num;
        }

        public static string MetabolismDescAt(int met)
        {
            if (met == 0)
            {
                return string.Empty;
            }
            return "HungerRate".Translate() + " x" + GeneTuning.MetabolismToFoodConsumptionFactorCurve.Evaluate(met).ToStringPercent();
        }

        public static bool ColonyHasEnoughNeutroamine(int neutroamineRequiredAmount, Thing geneAssembler)
        {
            if (geneAssembler.MapHeld == null)
            {
                return true;
            }
            List<Thing> list = geneAssembler.MapHeld.listerThings.ThingsOfDef(XCNMod.neutroamineDef);
            int num = 0;
            foreach (Thing item in list)
            {
                if (!item.Position.Fogged(geneAssembler.MapHeld))
                {
                    num += item.stackCount;
                    if (num >= neutroamineRequiredAmount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int CalculateComplexity(List<Genepack> genepacksToRecombine)
        {
            int complexity = 0;
            List<GeneDefWithType> list = new List<GeneDefWithType>();
            for (int i = 0; i < genepacksToRecombine.Count; i++)
            {
                if (genepacksToRecombine[i].GeneSet != null)
                {
                    for (int j = 0; j < genepacksToRecombine[i].GeneSet.GenesListForReading.Count; j++)
                    {
                        list.Add(new GeneDefWithType(genepacksToRecombine[i].GeneSet.GenesListForReading[j], xenogene: true));
                    }
                }
            }
            List<GeneDef> list2 = list.NonOverriddenGenes();
            for (int k = 0; k < list2.Count; k++)
            {
                complexity += list2[k].biostatCpx;
            }
            return complexity;
        }

        //public static Thing FindNeutroamine(Pawn pawn)
        //{
        //    return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(XCNMod.neutroamineDef), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x));
        //}

        public static int NeutroamineRequiredNow(int neutroamineRequired, Building_GeneAssembler geneAssembler)
        {
            int num = 0;
            for (int i = 0; i < geneAssembler.innerContainer.Count; i++)
            {
                if (geneAssembler.innerContainer[i].def == XCNMod.neutroamineDef)
                {
                    num += geneAssembler.innerContainer[i].stackCount;
                }
            }
            return neutroamineRequired - num;
        }

        public static bool CanReachEnoughNeutroamine(Pawn pawn, int requiredAmount, List<Thing> neutroamineThings)
        {
            if (pawn?.Map == null) return false;

            int reachableAmount = 0;

            foreach (Thing neutroamine in neutroamineThings)
            {
                if (!neutroamine.IsForbidden(pawn) &&
                    pawn.CanReach(neutroamine, PathEndMode.ClosestTouch, Danger.Deadly) &&
                    pawn.CanReserve(neutroamine))
                {
                    reachableAmount += neutroamine.stackCount;

                    if (reachableAmount >= requiredAmount)
                        return true;
                }
            }

            return false;
        }

        public static List<Thing> GetClosestNeuotramineThings(Pawn pawn, int requiredAmount)
        {
            if (pawn?.Map == null) return null;

            List<Thing> neutroamineThings = pawn.Map.listerThings.ThingsOfDef(XCNMod.neutroamineDef);
            List<Thing> closest = new List<Thing>();
            int reachableAmount = 0;

            // Sort by distance for efficiency (closest first)
            neutroamineThings.Sort((a, b) =>
                a.Position.DistanceToSquared(pawn.Position).CompareTo(
                b.Position.DistanceToSquared(pawn.Position)));

            foreach (Thing neutroamine in neutroamineThings)
            {
                if (!neutroamine.IsForbidden(pawn) &&
                    pawn.CanReach(neutroamine, PathEndMode.ClosestTouch, Danger.Deadly) &&
                    pawn.CanReserve(neutroamine))
                {
                    reachableAmount += neutroamine.stackCount;
                    closest.Add(neutroamine);   
                    if (reachableAmount >= requiredAmount)
                        return closest;
                }
            }

            return null;
        }


        public static Thing FindArchiteCapsule(Pawn pawn)
        {
            return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.ArchiteCapsule), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x));
        }

        public static int CalculcateNeutroamineRequired(int complexity, int architesRequired)
        {
            if (ModSettings.hardmode.Value && architesRequired > 0)
            {
                return (complexity * ModSettings.neutroaminePerComplexity.Value) * ModSettings.architeGenesMultiplier.Value * architesRequired;
            }
            return complexity * ModSettings.neutroaminePerComplexity.Value;
        }
    }
}
