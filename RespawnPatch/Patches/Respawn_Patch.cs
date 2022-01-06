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

            /* Remove
             *      this.data.stats.remainingRespawns--;
             * with
             *      CallReduceRespawns(data.player.PlayerID);
             */

            //var player = AccessTools.Field(typeof(HealthHandler), "player");
            //var CallReduce = AccessTools.Method(typeof(RespawnPatch), nameof(RespawnPatch.CallReduceRespawns), new Type[] { typeof(Player) });

            //codesToInsert.Add(new CodeInstruction(OpCodes.Ldfld, player));
            //codesToInsert.Add(new CodeInstruction(OpCodes.Call, CallReduce));

            //codes.RemoveRange(11, 7);
            //codes.InsertRange(11, codesToInsert);

            codes[10].opcode = OpCodes.Nop;
            codes.RemoveRange(11, 7);

            //for (var i = 0; i < codes.Count; i++)
            //{
            //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            //}

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(HealthHandler), nameof(HealthHandler.DoDamage))]
    class ReduceRespawn_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var codesToInsert = new List<CodeInstruction>();

            /* Insert
             *      CallReduceRespawns(data.player.PlayerID);
             * Before
             *      this.data.view.RPC("RPCA_Die_Phoenix", RpcTarget.All, new object[] { damage });
             */

            var player = AccessTools.Field(typeof(HealthHandler), "player");
            var CallReduce = AccessTools.Method(typeof(RespawnPatch), nameof(RespawnPatch.CallReduceRespawns), new Type[] { typeof(Player) });

            codesToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
            codesToInsert.Add(new CodeInstruction(OpCodes.Ldfld, player));
            codesToInsert.Add(new CodeInstruction(OpCodes.Call, CallReduce));

            //codes.InsertRange(110, codesToInsert);

            //codes[10].opcode = OpCodes.Nop;
            //codes.RemoveRange(11, 7);

            var inserted = false;

            for (var i = 0; i < codes.Count; i++)
            {
                var strOperand = codes[i].operand as string;
                if (strOperand == "RPCA_Die_Phoenix" && !inserted)
                {
                    codes.InsertRange(i - 3, codesToInsert);
                    inserted = true;
                }
            }

            //for (var i = 0; i < codes.Count; i++)
            //{
            //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            //}

            return codes.AsEnumerable();
        }
    }
}
