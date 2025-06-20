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
    public static class MechFriendly_InteractionWorker_RecruitAttempt
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(InteractionWorker_RecruitAttempt), "DoRecruit", new Type[] { typeof(Pawn), typeof(Pawn), typeof(string).MakeByRefType(), typeof(string).MakeByRefType(), typeof(bool), typeof(bool)});
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var recruiterField = typeof(InteractionWorker_RecruitAttempt)
                        .GetNestedType("<>c__DisplayClass14_0", BindingFlags.NonPublic)
                        .GetField("recruiter", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo TryDevelopBond = AccessTools.Method(typeof(RelationsUtility), "TryDevelopBondRelation");
            var continueDevelopBond = il.DefineLabel();
            var skipDevelopBond = il.DefineLabel();
            bool skip = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (skip)
                {
                    instruction.labels.Add(skipDevelopBond);
                    skip = false;
                }
                if (instruction.opcode == OpCodes.Call && instruction.Calls(TryDevelopBond))
                {
                    // if (pawn.RaceProps.IsMechanoid)
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, recruiterField);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueDevelopBond); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br, skipDevelopBond);
                    instruction.labels.Add(continueDevelopBond);
                    skip = true;
                }
                
                yield return instruction;
            }
        }



    }
}
