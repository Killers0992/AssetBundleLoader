using System;

namespace AssetBundleLoader.Components.Enums
{
	[Flags]
	public enum DoorDamageType : byte
	{
		None = 1,
		ServerCommand = 2,
		Grenade = 4,
		Weapon = 8,
		Scp096 = 16
	}
}
