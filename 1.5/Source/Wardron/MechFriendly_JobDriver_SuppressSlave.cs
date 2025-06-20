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
    public static class MechFriendlySuppress
    {
        static void Suppress(Pawn actor, Pawn animal)
        {
            InteractionDefOf.Suppress.Worker.Interacted(actor, animal, new List<RulePackDef>(), out _, out _, out _, out _);
        }

    }
    [HarmonyPatch]
    public static class MechFriendly_JobDriver_SuppressSlave
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(JobDriver_SuppressSlave), "<TrySuppress>b__5_0");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var skipTryInteractWith = il.DefineLabel();
            var continueTryInteractWith = il.DefineLabel();
            bool edit = true;
            bool skip = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (edit)
                {
                    edit = false;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "pawn"));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueTryInteractWith); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(JobDriver), "pawn"));
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JobDriver_SuppressSlave), "get_Slave"));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MechFriendlySuppress), "Suppress"));
                    yield return new CodeInstruction(OpCodes.Br, skipTryInteractWith);
                    instruction.labels.Add(continueTryInteractWith);
                }
                if (skip)
                {
                    skip = false;
                    instruction.labels.Add(skipTryInteractWith);
                }
                if (instruction.opcode == OpCodes.Pop)
                {
                    skip = true;
                }
                
                yield return instruction;
            }
        }



    }
}
