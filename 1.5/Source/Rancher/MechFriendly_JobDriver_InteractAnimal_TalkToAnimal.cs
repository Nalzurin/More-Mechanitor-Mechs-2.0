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
    public static class MechFriendly_JobDriver_InteractAnima_TalkToAnimal
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.Inner(typeof(JobDriver_InteractAnimal), "<>c__DisplayClass18_0"), "<TalkToAnimal>b__0");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            MethodInfo TryInteractWithMethod = AccessTools.Method(typeof(Pawn_InteractionsTracker), "TryInteractWith");
            var continueInteractLabel = il.DefineLabel();
            var skipInteractLabel = il.DefineLabel();
            bool skip = false;
            bool write = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (skip)
                {
                    instruction.labels.Add(skipInteractLabel);
                    skip = false;
                }
                if (instruction.opcode == OpCodes.Callvirt && instruction.Calls(TryInteractWithMethod))
                {
                    skip = true;
                }
                if (write)
                {
                    write = false;
                    // if (pawn.RaceProps.IsMechanoid)
                    yield return new CodeInstruction(OpCodes.Dup);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueInteractLabel); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Br, skipInteractLabel);
                    instruction.labels.Add(continueInteractLabel);
                }
                if (instruction.opcode == OpCodes.Stloc_0)
                {
                    write = true;

                }
                
                yield return instruction;
            }
        }



    }
}
