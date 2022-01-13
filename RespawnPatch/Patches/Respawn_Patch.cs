using HarmonyLib;
using HarmonyLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RespawnPatch.Patches
{
    [HarmonyPatch(typeof(HealthHandler), nameof(HealthHandler.DoDamage))]
    class ReduceRespawn_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var checkResponsibility = AccessTools.Method(typeof(RespawnPatch), nameof(RespawnPatch.IsPlayerResponsibleForRespawns), new Type[] { typeof(CharacterData) });

            codes[102] = new CodeInstruction(OpCodes.Call, checkResponsibility);

            //for (var i = 0; i < codes.Count; i++)
            //{
            //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            //}

            return codes.AsEnumerable();
        }
    }
}
