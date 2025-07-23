using Verse;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public class NeutroamineRequiredComp : ThingComp
    {
        private int _neutroamineRequired = 0;

        public int NeutroamineRequired
        {
            get => _neutroamineRequired;
            set => _neutroamineRequired = value;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref _neutroamineRequired, "neutroamineRequired", 10);
        }
    }
}
