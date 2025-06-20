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
using Verse.AI;

namespace MoreMechanitorMechs
{

    public static class WardronHelper
    {
        public static bool IsWardron(Pawn p)
        {
            return p.kindDef.defName == "Mech_Wardron";
        }
    }
    [HarmonyPatch]
    public static class Wardron_IsSociallyProper
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(SocialProperness), nameof(SocialProperness.IsSociallyProper), new Type[] {typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool)});
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label skipWardronLabel = il.DefineLabel();
            FieldInfo field = AccessTools.Field(typeof(Verse.ThingDef), "socialPropernessMatters");
            int k = 0;
            bool edit = false;
            bool found = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (edit)
                {
                    Log.Message("Editing");
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(WardronHelper), nameof(WardronHelper.IsWardron)));
                    yield return new CodeInstruction(OpCodes.Brtrue_S, skipWardronLabel);
                    edit = false;
                }
                if(instruction.opcode == OpCodes.Brtrue_S &&  instructions.ElementAt(k-1).opcode == OpCodes.Callvirt && instructions.ElementAt(k - 1).Calls(AccessTools.Method(typeof(RaceProperties), "get_Humanlike")))
                {
                    Log.Message("Found where to edit");
                    edit = true;
                }
                if (!found)
                {
                    if (instruction.opcode == OpCodes.Ldarg_0 && instructions.ElementAt(k+1).opcode == OpCodes.Ldfld && instructions.ElementAt(k + 1).operand.ToString().Equals("Verse.ThingDef def") && instructions.ElementAt(k + 2).opcode == OpCodes.Ldfld && instructions.ElementAt(k + 2).operand as FieldInfo == field)
                    {
                        Log.Message("Found where to add label");
                        found = true;
                        instruction.labels.Add(skipWardronLabel);
                    }
                }
               
                k++;
                yield return instruction;
            }
        }
        /*private static void Postfix(ref bool __result, Thing t, Pawn p, bool forPrisoner, bool animalsCare = false)
        {

            if (!animalsCare && p != null && !p.RaceProps.Humanlike && p.kindDef.defName != "Mech_Wardron")
            {
                __result = true;
            }
            if (!t.def.socialPropernessMatters)
            {
                __result = true;
            }
            if (!t.Spawned)
            {
                __result = true;
            }
            IntVec3 intVec = (t.def.hasInteractionCell ? t.InteractionCell : t.Position);
            if (forPrisoner)
            {
                if (p != null)
                {
                    __result = intVec.GetRoom(t.Map) == p.GetRoom();
                }
                __result = true;
            }
            if (ModsConfig.BiotechActive && t.def == ThingDefOf.HemogenPack)
            {
                __result = !SocialProperness.BloodfeedingPrisonerInRoom(t.GetRoom());
            }
            __result = !intVec.IsInPrisonCell(t.Map);
        }*/



    }
}
