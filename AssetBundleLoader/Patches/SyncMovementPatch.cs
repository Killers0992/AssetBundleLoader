using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetBundleLoader.Patches
{
	[HarmonyPatch(typeof(AdminToys.AdminToyBase), nameof(AdminToys.AdminToyBase.UpdatePositionServer))]
	public class SyncMovementPatch
	{
		public class CachedData
        {
			public Vector3 Pos { get; set; } = Vector3.zero;
			public Quaternion Rot { get; set; } = Quaternion.identity;
			public Vector3 Scale { get; set; } = Vector3.zero;
		}

		public static Dictionary<AdminToys.AdminToyBase, CachedData> Cache = new Dictionary<AdminToys.AdminToyBase, CachedData>();

		public static bool Prefix(AdminToys.AdminToyBase __instance)
		{
			if (Cache.TryGetValue(__instance, out CachedData cache))
			{
				if (cache.Pos == __instance.transform.position && cache.Rot == __instance.transform.rotation && cache.Scale == __instance.transform.localScale)
					return false;

				foreach (var plr in Player.List)
				{
					if ((__instance.transform.position - plr.Position).sqrMagnitude > (MainClass.instance.Config.RenderDistance ^ 2))
						continue;

					if (cache.Pos != __instance.transform.position)
					{
						cache.Pos = __instance.transform.position;
						plr.SendFakeSyncVar(__instance.netIdentity, __instance.GetType(), nameof(__instance.NetworkPosition), __instance.transform.position);
					}

					if (cache.Scale != __instance.transform.localScale)
					{
						cache.Scale = __instance.transform.localScale;
						plr.SendFakeSyncVar(__instance.netIdentity, __instance.GetType(), nameof(__instance.NetworkScale), __instance.transform.localScale);
					}

					if (cache.Rot != __instance.transform.rotation)
					{
						cache.Rot = __instance.transform.rotation;
						plr.SendFakeSyncVar(__instance.netIdentity, __instance.GetType(), nameof(__instance.Rotation), new LowPrecisionQuaternion(__instance.transform.rotation));
					}
				}
			}
			else
            {
				Cache.Add(__instance, new CachedData());
			}
			return false;
		}
	}
}
