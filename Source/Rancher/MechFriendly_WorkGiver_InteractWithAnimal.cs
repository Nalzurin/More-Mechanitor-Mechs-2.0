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
    public static class MechFriendly_WorkGiver_InteractAnimal
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(WorkGiver_InteractAnimal), "CanInteractWithAnimal", new Type[] { typeof(Pawn), typeof(Pawn),  typeof(string).MakeByRefType(), typeof(bool), typeof(bool), typeof(bool), typeof(bool) });
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            MethodInfo getLevelMethod = AccessTools.Method(typeof(Pawn_SkillTracker), "GetSkill");
            var skipLoadSkillsLabel = il.DefineLabel();
            var comparisonLabel = il.DefineLabel();
            bool count = false;
            int countIndex = 2;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand.ToString().Equals("RimWorld.Pawn_SkillTracker skills"))
                {

                    // if (pawn.RaceProps.IsMechanoid)
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RaceProperties), "get_IsMechanoid")); // Call get_IsMechanoid
                    yield return new CodeInstruction(OpCodes.Brfalse_S, skipLoadSkillsLabel); // If not Mechanoid, jump to the original code
                    
                    // Load the fixed skill level
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // Load pawn
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn), "get_RaceProps")); // Load pawn.RaceProps
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RaceProperties), "mechFixedSkillLevel")); // Load mechFixedSkillLevel

                    yield return new CodeInstruction(OpCodes.Br, comparisonLabel);

                    CodeInstruction instr = new CodeInstruction(OpCodes.Ldarg_0);
                    instr.labels.Add(skipLoadSkillsLabel);
                    yield return instr;
                    yield return instruction;
                    
                }
                else if(instruction.opcode == OpCodes.Callvirt && instruction.Calls(getLevelMethod))
                {
                    count = true;
                    yield return instruction;
                }
                else
                {                                                                                                                                                                               
                    yield return instruction;
                }
                if(count)
                {
                    if(countIndex == 0)
                    {
                        count = false;
                        instruction.labels.Add(comparisonLabel);
                    }
                    else
                    {
                        countIndex--;
                    }
                }
            }
        }


       
    }
}
