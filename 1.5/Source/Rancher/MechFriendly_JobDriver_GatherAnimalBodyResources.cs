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
    public static class MechFriendly_JobDriver_GatherAnimalBodyResources
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.Inner(typeof(JobDriver_GatherAnimalBodyResources), "<>c__DisplayClass7_0"), "<MakeNewToils>b__1");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            MethodInfo learnMethod = AccessTools.Method(typeof(Pawn_SkillTracker), "Learn");
            Label postEditLabel = il.DefineLabel();
            Label postLearnLabel = il.DefineLabel();
            bool edited = false;
            bool postLearn = false;
            byte edits = 0;
            foreach (CodeInstruction instruction in instructions)
            {
                if (edited)
                {
                    instruction.labels.Add(postEditLabel);
                    edited = false;
                    edits++;
                }
                else if (postLearn)
                {
                    instruction.labels.Add(postLearnLabel);
                    postLearn = false;
                    edits++;
                }
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand.ToString().Equals("RimWorld.Pawn_SkillTracker skills"))
                {
                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Brtrue_S, postEditLabel);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br_S, postLearnLabel);
                    edited = true;
                }
                else if (instruction.opcode == OpCodes.Callvirt && instruction.Calls(learnMethod))
                {
                    yield return instruction;
                    postLearn = true;
                }
                else
                {
                    yield return instruction;
                }
            }
            if (edits != 2)
            {
                Log.Warning("Making JobDriver_GatherAnimalBodyResources Mech friendly failed");
            }
        }
    }
}
