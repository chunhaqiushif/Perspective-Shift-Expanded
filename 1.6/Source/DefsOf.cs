using RimWorld;
using Verse;

namespace PerspectiveShiftExpanded
{
    [DefOf]
    public static class DefsOf
    {
        public static KeyBindingDef PSE_ReadBook;
        public static JobDef PSE_AvatarReading;
        static DefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
        }
    }
}
