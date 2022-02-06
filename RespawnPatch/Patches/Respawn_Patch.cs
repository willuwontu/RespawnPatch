using HarmonyLib;
using HarmonyLib.Tools;
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
            var codesToInsert = new List<CodeInstruction>();
            /* Replace
             *      this.data.stats.remainingRespawns--;
             * with
             *      ReduceRespawns(data.player.PlayerID);
             */
            //var player = AccessTools.Field(typeof(HealthHandler), "player");
            //codes.RemoveRange(11, 7);
            //codes.InsertRange(11, codesToInsert);
            //codes[10].opcode = OpCodes.Nop;
            //codes.RemoveRange(11, 7);

            var reduceRespawns = AccessTools.Method(typeof(RespawnPatch), nameof(RespawnPatch.ReduceRespawns), new Type[] { typeof(CharacterData) });

            for (var i = 0; i < codes.Count; i++)
            {
                UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            }

            codes.RemoveRange(13, 17);
            codes[12] = new CodeInstruction(OpCodes.Call, reduceRespawns);

            for (var i = 0; i < codes.Count; i++)
            {
                UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            }

            return codes.AsEnumerable();
        }
    }
}
