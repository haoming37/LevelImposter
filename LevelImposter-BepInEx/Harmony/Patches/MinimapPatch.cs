using HarmonyLib;
using LevelImposter.MinimapGen;
using LevelImposter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LevelImposter.Harmony.Patches
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.Awake))]
    public static class MinimapPatch
    {
        private static MinimapGenerator mapApplicator = new MinimapGenerator();

        public static void Prefix(MapBehaviour __instance)
        {
            mapApplicator.PreGen(__instance);
        }

        public static void Postfix(MapBehaviour __instance)
        {
            mapApplicator.Finish();
        }
    }
     [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
     public static class FixedUpdatePatch{
         public static bool Prefix(MapBehaviour __instance){
             System.Console.WriteLine("FixedUpdatePatch");
             if (!ShipStatus.Instance)
             {
                 return false;
             }
             Vector3 vector = PlayerControl.LocalPlayer.transform.position;
             vector /= ShipStatus.Instance.MapScale;
             vector /= 2; // 地図の縮尺を変更したため実施
             vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
             vector.y -= 2.5f;
             vector.z = -1f;
             __instance.HerePoint.transform.localPosition = vector;
             // LILogger.LogInfo($"{vector.x},{vector.y}");
             return false;
         }
     }
     // [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.GenericShow))]
     // public static class GenericShowPatch{
     //     public static void Postfix(MapBehaviour __instance){
     //         __instance.taskOverlay.transform.localScale /= 2;

     //     }
     // }
     [HarmonyPatch(typeof(MapTaskOverlay), nameof(MapTaskOverlay.SetIconLocation))]
     public static class SetIconLocationPatch{
             public static void Postfix(MapTaskOverlay __instance, PlayerTask task){
                //Il2CppSystem.Collections.Generic.Dictionary<string, PooledMapIcon> data = Traverse.Create(__instance).Field("data").GetValue() as Il2CppSystem.Collections.Generic.Dictionary<string, PooledMapIcon>;
                Il2CppSystem.Collections.Generic.Dictionary<string, PooledMapIcon> data = __instance.data;
                for (int i = 0; i < task.Locations.Count; i++)
                {
                    Vector3 localPosition = task.Locations[i] / ShipStatus.Instance.MapScale;
                    localPosition.z = -1f;
                    localPosition.x /= 2f;
                    localPosition.y /= 2f;
                    localPosition.y -= 2.5f;
                    if(data.ContainsKey(task.name + "0")){
                        LILogger.LogInfo(task.name);
                        LILogger.LogInfo($"{localPosition.x},{localPosition.y}");
                        data[task.name + "0"].transform.localPosition = localPosition;
                    }
                    LILogger.LogInfo("[keys]");
                    foreach(string key in data.Keys){
                        LILogger.LogInfo(key);
                    }
                }
                //Traverse.Create(__instance).Field("data").SetValue(data);
                __instance.data = data;
             }
     }
}
