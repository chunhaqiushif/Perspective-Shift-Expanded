using RimWorld;
using Verse;

namespace PerspectiveShiftExpanded
{
    [DefOf]
    public static class DefsOf
    {
        public static KeyBindingDef PSE_ReadBook;
        public static KeyBindingDef PSE_SelectAvatar;
        public static KeyBindingDef PSE_CE_AvatarReload;
        public static JobDef PSE_AvatarReading;
        static DefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DefsOf));
        }
    }
}
