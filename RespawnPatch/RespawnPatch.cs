using BepInEx;
using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;

namespace RespawnPatch
{
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class RespawnPatch : BaseUnityPlugin
    {
        private const string ModId = "com.willuwontu.rounds.RespawnPatch";
        private const string ModName = "Respawn Patch";
        public const string Version = "1.0.0"; // What version are we on (major.minor.patch)?



        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        internal static void CallReduceRespawns(int playerID)
        {
            Player player = PlayerManager.instance.players.Where(p => p.playerID == playerID).First();

            if (PhotonNetwork.IsMasterClient)
            {
                player.data.view.RPC(nameof(RPCA_ReduceRespawns), RpcTarget.All, playerID);
            }
        }

        [PunRPC]
        private void RPCA_ReduceRespawns(int playerID)
        {
            var player = PlayerManager.instance.players.Where(p => p.playerID == playerID).First();

            player.data.stats.remainingRespawns--;
        }
    }
}
