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
    public static class MechFriendlyTame
    {
        static void Tame(Pawn actor, Pawn animal)
        {
            InteractionDefOf.TameAttempt.Worker.Interacted(actor, animal, new List<RulePackDef>(), out _, out _, out _, out _);
        }

    }


    [HarmonyPatch]
    public static class MechFriendly_TryRecruit_Tame
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.Inner(typeof(Toils_Interpersonal), "<>c__DisplayClass9_0"), "<TryRecruit>b__0");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            MethodInfo TryInteractWithMethod = AccessTools.Method(typeof(Pawn_InteractionsTracker), "TryInteractWith");
            var listCtor = typeof(List<RulePackDef>).GetConstructor(Type.EmptyTypes);
            var continueInteractLabel = il.DefineLabel();
            var skipInteractLabel = il.DefineLabel();
            bool edited = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (edited)
                {
                    edited = false;
                    instruction.labels.Add(skipInteractLabel);
                }
                if (instruction.opcode == OpCodes.Callvirt && instruction.Calls(TryInteractWithMethod))
                {
                    edited = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueInteractLabel); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MechFriendlyTame), "Tame"));
                    


                    yield return new CodeInstruction(OpCodes.Br, skipInteractLabel);
                    instruction.labels.Add(continueInteractLabel);
                }
                
                yield return instruction;
            }
        }



    }
}
