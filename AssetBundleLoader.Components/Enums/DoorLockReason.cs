using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetBundleLoader.Components.Enums
{
	[Flags]
	public enum DoorLockReason : ushort
	{
		None = 0,
		Regular079 = 1,
		Lockdown079 = 2,
		Warhead = 4,
		AdminCommand = 8,
		DecontLockdown = 16,
		DecontEvacuate = 32,
		SpecialDoorFeature = 64,
		NoPower = 128,
		Isolation = 256,
		Lockdown2176 = 512
	}
}
