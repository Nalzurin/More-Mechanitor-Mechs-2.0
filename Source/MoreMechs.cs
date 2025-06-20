using HarmonyLib;
using Verse;

namespace MoreMechanitorMechs
{
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            Log.Message("Hello world from RancherMech");
            Harmony harmony = new Harmony(content.PackageId);
            harmony.PatchAll();
        }
    }
}