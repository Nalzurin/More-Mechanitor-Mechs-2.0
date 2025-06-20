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
    [HarmonyPatch]
    public static class MechFriendly_JobDriver_ActivitySuppression
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.Inner(typeof(JobDriver_ActivitySuppression), "<>c__DisplayClass9_0"), "<TrySuppress>b__1");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            MethodInfo learnMethod = AccessTools.Method(typeof(Pawn_SkillTracker), "Learn");
            var skipTryLearn = il.DefineLabel();
            var continueTryLearn = il.DefineLabel();
            bool skip = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand.ToString().Equals("RimWorld.Pawn_SkillTracker skills"))
                {
                    Log.Message("Found skills");
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueTryLearn); // If Mechanoid, skip learn method
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br, skipTryLearn);
                    instruction.labels.Add(continueTryLearn);
                }
                if (skip)
                {
                   
                    instruction.labels.Add(skipTryLearn);
                    skip = false;
                }
                if (instruction.opcode == OpCodes.Callvirt && instruction.Calls(learnMethod))
                {
                    Log.Message("Found learn");
                    skip = true;
                }

                yield return instruction;
            }
        }



    }
}
