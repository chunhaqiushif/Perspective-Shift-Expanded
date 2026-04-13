using RimWorld;
using System.Collections.Generic;
using System.Threading;
using Verse;
using Verse.AI;

namespace PerspectiveShiftExpanded
{
    public class JobDriver_AvatarReading : JobDriver_Reading
    {
        public override IEnumerable<Toil> MakeNewToils()
        {
            SetFinalizerJob(delegate (JobCondition condition)
            {
                if (!pawn.IsCarryingThing(Book))
                {
                    return (Job)null;
                }

                if (condition != JobCondition.Succeeded)
                {
                    pawn.carryTracker.innerContainer.TryTransferToContainer(Book, pawn.inventory.innerContainer);
                    return (Job)null;
                }
                pawn.carryTracker.innerContainer.TryTransferToContainer(Book, pawn.inventory.innerContainer);
                return (Job)null;
            });

            foreach (Toil item in PrepareToReadBook())
            {
                yield return item;
            }

            int duration = (job.playerForced ? 10000 : job.def.joyDuration);

            yield return ReadBook(duration);
        }


        public override void Notify_Starting()
        {
            base.Notify_Starting();

            job.count = 1;

            hasInInventory = pawn.inventory != null && pawn.inventory.Contains(Book);

            carrying = pawn?.carryTracker.CarriedThing == Book;

            isLearningDesire = pawn?.learning != null &&
                               pawn.learning.ActiveLearningDesires.Contains(LearningDesireDefOf.Reading);
        }
    }
}