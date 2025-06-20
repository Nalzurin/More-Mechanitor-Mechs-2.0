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
    public static class MechFriendly_DoReleaseAnimal
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ReleaseAnimalToWildUtility), "DoReleaseAnimal");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            MethodInfo tryInteractWith = AccessTools.Method(typeof(Pawn_InteractionsTracker), "TryInteractWith");
            var continueTryInteractWith = il.DefineLabel();
            var skipTryInteractWith = il.DefineLabel();
            bool skip = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (skip)
                {
                    instruction.labels.Add(skipTryInteractWith);
                    skip = false;
                }
                if (instruction.opcode == OpCodes.Callvirt && instruction.Calls(tryInteractWith))
                {
                    // if (pawn.RaceProps.IsMechanoid)
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueTryInteractWith); // If not Mechanoid, jump to the original code
                   
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br, skipTryInteractWith);
                    instruction.labels.Add(continueTryInteractWith);
                    skip = true;
                }
                
                yield return instruction;
            }
        }



    }
}
