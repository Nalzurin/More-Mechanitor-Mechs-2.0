using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MoreMechanitorMechs
{

    [HarmonyPatch]
    public static class MechFriendly_Toils_Interpersonal_WaitToBeAbleToInteract_Initial
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.Inner(typeof(Toils_Interpersonal), "<>c__DisplayClass4_0"), "<WaitToBeAbleToInteract>b__0");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var skipCheckLabel = il.DefineLabel();
            bool skipFound = false;
            bool first = true;
            foreach (CodeInstruction instruction in instructions)
            {
                if (first)
                {
                    first = false;
                    // if (pawn.RaceProps.IsMechanoid)
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(AccessTools.Inner(typeof(Toils_Interpersonal), "<>c__DisplayClass4_0"), "pawn"));
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brtrue_S, skipCheckLabel); // If not Mechanoid, jump to the original code
                }
                if (skipFound)
                {
                    skipFound = false;
                    instruction.labels.Add(skipCheckLabel);
                }
                if (instruction.opcode == OpCodes.Brtrue_S)
                {
                    skipFound = true;
                }

                yield return instruction;

            }
        }
        [HarmonyPatch]
        public static class MechFriendly_Toils_Interpersonal_WaitToBeAbleToInteract_Tick
        {
            private static MethodBase TargetMethod()
            {
                return AccessTools.Method(AccessTools.Inner(typeof(Toils_Interpersonal), "<>c__DisplayClass4_0"), "<WaitToBeAbleToInteract>b__1");
            }
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var skipCheckLabel = il.DefineLabel();
                bool skipFound = false;
                bool first = true;
                foreach (CodeInstruction instruction in instructions)
                {
                    if (first)
                    {
                        first = false;
                        // if (pawn.RaceProps.IsMechanoid)
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(AccessTools.Inner(typeof(Toils_Interpersonal), "<>c__DisplayClass4_0"), "pawn"));
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                        yield return new CodeInstruction(OpCodes.Brtrue_S, skipCheckLabel); // If not Mechanoid, jump to the original code
                    }
                    if (skipFound)
                    {
                        skipFound = false;
                        instruction.labels.Add(skipCheckLabel);
                    }
                    if (instruction.opcode == OpCodes.Brtrue_S)
                    {
                        skipFound = true;
                    }

                    yield return instruction;

                }
            }


        }
    }
}