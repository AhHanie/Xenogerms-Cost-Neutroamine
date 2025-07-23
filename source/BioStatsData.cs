using UnityEngine;

namespace SK.Xenogerms_Cost_Neutroamine
{
    public struct BiostatData
    {
        public string labelKey;

        public string descKey;

        public Texture2D icon;

        public bool displayMaxGCXInfo;

        public BiostatData(string labelKey, string descKey, Texture2D icon, bool displayMaxGCXInfo)
        {
            this.labelKey = labelKey;
            this.descKey = descKey;
            this.icon = icon;
            this.displayMaxGCXInfo = displayMaxGCXInfo;
        }
    }
}
