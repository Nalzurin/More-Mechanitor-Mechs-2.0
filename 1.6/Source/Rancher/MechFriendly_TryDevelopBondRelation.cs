using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MoreMechanitorMechs
{

    [HarmonyPatch]
    public static class MechFriendly_TryDevelopBondRelation
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(RelationsUtility), "TryDevelopBondRelation");
        }
        private static bool Prefix(Pawn humanlike, Pawn animal, float baseChance)
        {
            if(humanlike.RaceProps.IsMechanoid)
            {
                return false;
            }
            return true;
        }


    }
}
