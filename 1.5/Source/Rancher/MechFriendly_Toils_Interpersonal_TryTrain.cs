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
    public static class MechFriendly_Toils_Interpersonal_TryTrain_TryInteractWithSkip
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(AccessTools.Inner(typeof(Toils_Interpersonal), "<>c__DisplayClass10_0"), "<TryTrain>b__0");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            //TryInteractWith
            MethodInfo TryInteractWithMethod = AccessTools.Method(typeof(Pawn_InteractionsTracker), "TryInteractWith");
            var continueInteractLabel = il.DefineLabel();
            var skipInteractLabel = il.DefineLabel();
            bool doCount = false;
            int count = 0;
            //GetStatValue
            MethodInfo GetStatValue = AccessTools.Method(typeof(StatExtension), "GetStatValue");
            var continueGetStatValue = il.DefineLabel();
            var skipGetStatValue = il.DefineLabel();
            bool assignSkipStat = false;

            //Try Bond With
            MethodInfo TryDeverlopBondRelation = AccessTools.Method(typeof(RelationsUtility), "TryDevelopBondRelation");
            MethodInfo MakeTale = AccessTools.Method(typeof(TaleRecorder), "RecordTale");
            var continueTryDevelopBondRelation = il.DefineLabel();
            var skipTryDevelopBondRelation = il.DefineLabel();
            bool doTaleCount = false;
            int taleCount = 0;
            foreach (CodeInstruction instruction in instructions)
            {

                //TryInteractWith
                if (count == 1)
                {
                    doCount = false;
                    instruction.labels.Add(skipInteractLabel);
                }
                if (doCount)
                {

                    count++;
                }
                if (instruction.opcode == OpCodes.Callvirt && instruction.Calls(TryInteractWithMethod))
                {
                    // if (pawn.RaceProps.IsMechanoid)
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueInteractLabel); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br, skipInteractLabel);
                    doCount = true;
                    instruction.labels.Add(continueInteractLabel);

                }

                //Get Stat Value
                if (assignSkipStat)
                {
                    instruction.labels.Add(skipGetStatValue);
                    assignSkipStat = false;
                }

                if (instruction.opcode == OpCodes.Call && instruction.Calls(GetStatValue))
                {
                    // if (pawn.RaceProps.IsMechanoid)
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueGetStatValue); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldc_R4, 0.35f);
                    yield return new CodeInstruction(OpCodes.Br, skipGetStatValue);
                    instruction.labels.Add(continueGetStatValue);
                    assignSkipStat = true;

                }

                //Try bond with
                if (taleCount == 1)
                {
                    doTaleCount = false;
                    instruction.labels.Add(skipTryDevelopBondRelation);
                }
                if (doTaleCount)
                {
                    taleCount++;
                }

                if (instruction.opcode == OpCodes.Call && instruction.Calls(TryDeverlopBondRelation))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueTryDevelopBondRelation); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br, skipTryDevelopBondRelation);
                    instruction.labels.Add(continueTryDevelopBondRelation);

                }
                if (instruction.opcode == OpCodes.Call && instruction.Calls(MakeTale))
                {
                    doTaleCount = true;
                }

                yield return instruction;
            }
        }



    }
}
