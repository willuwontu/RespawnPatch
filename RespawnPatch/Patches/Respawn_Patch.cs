using HarmonyLib;
using HarmonyLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Photon.Pun;

namespace RespawnPatch.Patches
{
    //[HarmonyPatch(typeof(HealthHandler), "RPCA_Die_Phoenix")]
    //class Respawn_Patch
    //{
    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var codes = new List<CodeInstruction>(instructions);

    //        var codesToInsert = new List<CodeInstruction>();

    //        /* Remove
    //         *      this.data.stats.remainingRespawns--;
    //         * with
    //         *      CallReduceRespawns(data.player.PlayerID);
    //         */

    //        //var player = AccessTools.Field(typeof(HealthHandler), "player");
    //        //var CallReduce = AccessTools.Method(typeof(RespawnPatch), nameof(RespawnPatch.CallReduceRespawns), new Type[] { typeof(Player) });

    //        //codesToInsert.Add(new CodeInstruction(OpCodes.Ldfld, player));
    //        //codesToInsert.Add(new CodeInstruction(OpCodes.Call, CallReduce));

    //        //codes.RemoveRange(11, 7);
    //        //codes.InsertRange(11, codesToInsert);

    //        codes[10].opcode = OpCodes.Nop;
    //        codes.RemoveRange(11, 7);

    //        //for (var i = 0; i < codes.Count; i++)
    //        //{
    //        //    UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
    //        //}

    //        return codes.AsEnumerable();
    //    }
    //}

    [HarmonyPatch(typeof(HealthHandler), nameof(HealthHandler.DoDamage))]
    class ReduceRespawn_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codes = new List<CodeInstruction>(instructions);

            var codesToInsert = new List<CodeInstruction>();

            /* Insert
             *      CallReduceRespawns(data.player.PlayerID);
             * Before
             *      this.data.view.RPC("RPCA_Die_Phoenix", RpcTarget.All, new object[] { damage });
             */

            //var player = AccessTools.Field(typeof(HealthHandler), "player");
            //var CallReduce = AccessTools.Method(typeof(RespawnPatch), nameof(RespawnPatch.CallReduceRespawns), new Type[] { typeof(Player) });

            //codesToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
            //codesToInsert.Add(new CodeInstruction(OpCodes.Ldfld, player));
            //codesToInsert.Add(new CodeInstruction(OpCodes.Call, CallReduce));

            ////codes.InsertRange(110, codesToInsert);

            ////codes[10].opcode = OpCodes.Nop;
            ////codes.RemoveRange(11, 7);

            //var inserted = false;

            var phoenixInsert = -1;
            var final = false;

            Label phoenixLabel = il.DefineLabel();
            Label notMyViewLabel = il.DefineLabel();

            var lastDamaged = AccessTools.Field(typeof(HealthHandler), "lastDamaged");

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand == "RPCA_Die_Phoenix")
                {
                    phoenixInsert = i-3;
                    codes[i - 3].labels.Add(phoenixLabel);
                }
                else if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand == lastDamaged)
                {
                    final = true;
                    codes[i - 1].labels.Add(notMyViewLabel);
                }
            }

            var player = AccessTools.Field(typeof(HealthHandler), "player");
            var data = AccessTools.Field(typeof(HealthHandler), "data");
            var view = AccessTools.Field(typeof(CharacterData), nameof(CharacterData.view));
            var mine = AccessTools.Property(typeof(PhotonView), nameof(PhotonView.IsMine));
            var offline = AccessTools.Property(typeof(PhotonNetwork), nameof(PhotonNetwork.OfflineMode));

            codesToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
            codesToInsert.Add(new CodeInstruction(OpCodes.Ldfld, data));
            codesToInsert.Add(new CodeInstruction(OpCodes.Ldfld, view));
            codesToInsert.Add(new CodeInstruction(OpCodes.Ldfld, mine));
            codesToInsert.Add(new CodeInstruction(OpCodes.Brtrue, phoenixLabel));
            codesToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
            codesToInsert.Add(new CodeInstruction(OpCodes.Ldfld, offline));
            codesToInsert.Add(new CodeInstruction(OpCodes.Brfalse, notMyViewLabel));

            if (phoenixInsert != -1&& final)
            {
                codes.InsertRange(phoenixInsert, codesToInsert);
            }

            for (var i = 0; i < codes.Count; i++)
            {
                UnityEngine.Debug.Log($"{i}: {codes[i].opcode}, {codes[i].operand}");
            }

            return codes.AsEnumerable();
        }
    }
}
