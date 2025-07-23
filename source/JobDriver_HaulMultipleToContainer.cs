using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public class JobDriver_HaulMultipleToContainer : JobDriver
    {
        private const TargetIndex ContainerInd = TargetIndex.A;
        private const TargetIndex IngredientInd = TargetIndex.B;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            // Reserve the container
            if (!pawn.Reserve(job.GetTarget(ContainerInd), job, 1, -1, null, errorOnFailed))
            {
                return false;
            }

            // Reserve as many ingredients as possible from the queue
            pawn.ReserveAsManyAsPossible(job.GetTargetQueue(IngredientInd), job);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(ContainerInd);
            this.FailOn(() => {
                ThingWithComps container = job.GetTarget(ContainerInd).Thing as ThingWithComps;
                if (container == null) return true;

                ThingOwner innerContainer = container.TryGetInnerInteractableThingOwner();
                return innerContainer == null;
            });

            foreach (Toil toil in CollectIngredientsToils(IngredientInd, ContainerInd))
            {
                yield return toil;
            }
        }

        private IEnumerable<Toil> CollectIngredientsToils(TargetIndex ingredientInd, TargetIndex containerInd)
        {
            // Extract next ingredient from queue
            Toil extract = Toils_JobTransforms.ExtractNextTargetFromQueue(ingredientInd);
            yield return extract;

            // Jump back to extract if more ingredients in queue
            Toil jumpIfHaveTargetInQueue = Toils_Jump.JumpIfHaveTargetInQueue(ingredientInd, extract);

            // Skip if ingredient is already in the container
            yield return JumpIfTargetInsideContainer(jumpIfHaveTargetInQueue, ingredientInd, containerInd);

            // Go to ingredient
            Toil getToHaulTarget = Toils_Goto.GotoThing(ingredientInd, PathEndMode.ClosestTouch)
                .FailOnForbidden(ingredientInd)
                .FailOnSomeonePhysicallyInteracting(ingredientInd);
            yield return getToHaulTarget;

            // Pick up ingredient
            yield return Toils_Haul.StartCarryThing(ingredientInd, putRemainderInQueue: true, subtractNumTakenFromJobCount: false, failIfStackCountLessThanJobCount: true);

            // Try to collect more of the same ingredient if possible
            yield return JumpToCollectNextIntoHands(getToHaulTarget, ingredientInd);

            // Go to container
            yield return Toils_Goto.GotoThing(containerInd, PathEndMode.InteractionCell)
                .FailOnDestroyedOrNull(ingredientInd);

            // Deposit ingredient in container
            yield return Toils_Haul.DepositHauledThingInContainer(containerInd, ingredientInd);

            // Continue with next ingredient if any
            yield return jumpIfHaveTargetInQueue;
        }

        private static Toil JumpIfTargetInsideContainer(Toil jumpToil, TargetIndex ingredientIndex, TargetIndex containerIndex)
        {
            Toil toil = ToilMaker.MakeToil("JumpIfTargetInsideContainer");
            toil.initAction = delegate
            {
                Thing container = toil.actor.CurJob.GetTarget(containerIndex).Thing;
                if (container != null && container.Spawned)
                {
                    Thing ingredientThing = toil.actor.jobs.curJob.GetTarget(ingredientIndex).Thing;
                    if (ingredientThing != null)
                    {
                        ThingOwner thingOwner = container.TryGetInnerInteractableThingOwner();
                        if (thingOwner != null && thingOwner.Contains(ingredientThing))
                        {
                            HaulAIUtility.UpdateJobWithPlacedThings(toil.actor.jobs.curJob, ingredientThing, ingredientThing.stackCount);
                            toil.actor.jobs.curDriver.JumpToToil(jumpToil);
                        }
                    }
                }
            };
            return toil;
        }

        private static Toil JumpToCollectNextIntoHands(Toil gotoGetTargetToil, TargetIndex ind)
        {
            Toil toil = ToilMaker.MakeToil("JumpToCollectNextIntoHands");
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                if (actor.carryTracker.CarriedThing == null)
                {
                    Log.Error("JumpToCollectNextIntoHands run on " + actor?.ToString() + " who is not carrying something.");
                }
                else if (!actor.carryTracker.Full)
                {
                    Job curJob = actor.jobs.curJob;
                    List<LocalTargetInfo> targetQueue = curJob.GetTargetQueue(ind);
                    if (!targetQueue.NullOrEmpty())
                    {
                        for (int i = 0; i < targetQueue.Count; i++)
                        {
                            if (GenAI.CanUseItemForWork(actor, targetQueue[i].Thing) &&
                                targetQueue[i].Thing.CanStackWith(actor.carryTracker.CarriedThing) &&
                                !((float)(actor.Position - targetQueue[i].Thing.Position).LengthHorizontalSquared > 64f))
                            {
                                int currentCount = actor.carryTracker.CarriedThing?.stackCount ?? 0;
                                int requestedCount = curJob.countQueue[i];
                                int availableCount = Mathf.Min(requestedCount, targetQueue[i].Thing.def.stackLimit - currentCount);
                                availableCount = Mathf.Min(availableCount, actor.carryTracker.AvailableStackSpace(targetQueue[i].Thing.def));

                                if (availableCount > 0)
                                {
                                    curJob.count = availableCount;
                                    curJob.SetTarget(ind, targetQueue[i].Thing);
                                    curJob.countQueue[i] -= availableCount;
                                    if (curJob.countQueue[i] <= 0)
                                    {
                                        curJob.countQueue.RemoveAt(i);
                                        targetQueue.RemoveAt(i);
                                    }
                                    actor.jobs.curDriver.JumpToToil(gotoGetTargetToil);
                                    break;
                                }
                            }
                        }
                    }
                }
            };
            return toil;
        }
    }
}