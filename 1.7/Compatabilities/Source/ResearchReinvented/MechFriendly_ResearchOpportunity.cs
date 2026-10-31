using HarmonyLib;
using PeteTimesSix.ResearchReinvented.Opportunities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static UnityEngine.Scripting.GarbageCollector;

namespace MechFriendlyResearchReinvented
{
    public class Mod : Verse.Mod
    {
        public Mod(ModContentPack content) : base(content)
        {
            Harmony harmony = new Harmony(content.PackageId);
            harmony.PatchAll();
        }
    }
    [HarmonyPatch]
    public static class MechFriendly_ResearchOpportunity
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(ResearchOpportunity), "ResearchTickPerformed");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label skipLearnLabel = il.DefineLabel();
            Label continueLearnLabel = il.DefineLabel();
            var methodInfo = AccessTools.Method(typeof(Pawn_SkillTracker), "Learn", new Type[] {
            typeof(SkillDef), typeof(float), typeof(bool), typeof(bool)
        });
            bool postLearn = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (postLearn)
                {
                    instruction.labels.Add(skipLearnLabel);
                    postLearn = false;
                }
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand as MethodInfo == methodInfo)
                {
                    Log.Message("Found learn method");
                    postLearn = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, continueLearnLabel); // If not Mechanoid, jump to the original code
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br_S, skipLearnLabel);
                    instruction.labels.Add(continueLearnLabel);

                }

                yield return instruction;
            }
        }
    }
}
