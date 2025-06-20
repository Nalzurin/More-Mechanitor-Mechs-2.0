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
    public static class MechFriendly_JobDriver_OperateScanner
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.Inner(typeof(JobDriver_OperateScanner), "<>c__DisplayClass1_0"), "<MakeNewToils>b__1");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {

            MethodInfo learnMethod = AccessTools.Method(typeof(Pawn_SkillTracker), "Learn");
            Label skipLearnLabel = il.DefineLabel();
            Label continueLearnLabel= il.DefineLabel();

            bool postLearn = false;
            foreach (CodeInstruction instruction in instructions)
            {

                if (postLearn)
                {
                    instruction.labels.Add(skipLearnLabel);
                    postLearn = false;
                }
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand.ToString().Equals("RimWorld.Pawn_SkillTracker skills"))
                {
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueLearnLabel); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br_S, skipLearnLabel);
                    instruction.labels.Add(continueLearnLabel);

                }
                else if (instruction.opcode == OpCodes.Callvirt && instruction.Calls(learnMethod))
                {
                    postLearn = true;
                }

                yield return instruction;
            }
        }
    }
}
