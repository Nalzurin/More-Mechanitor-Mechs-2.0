using AnimalBehaviours;
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

namespace VanillaExpandedFramework
{

    [HarmonyPatch]
    public static class MechFriendly_WorkGiver_VEF_AnimalResource
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(AnimalBehaviours.WorkGiver_AnimalResource), "HasJobOnThing");
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            bool skip = false;

            foreach (CodeInstruction instruction in instructions)
            {

                if (skip)
                {
                    skip = false;
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                }
                if (instruction.opcode == OpCodes.Ldloc_2)
                {
                    skip = true;

                }
                
                yield return instruction;
            }
        }



    }
}
