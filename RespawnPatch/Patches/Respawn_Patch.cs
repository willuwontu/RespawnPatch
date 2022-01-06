using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RespawnPatch.Patches
{
    [HarmonyPatch(typeof(HealthHandler), "RPCA_Die_Phoenix")]
    class Respawn_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            }

            return codes.AsEnumerable();
        }
    }
}
