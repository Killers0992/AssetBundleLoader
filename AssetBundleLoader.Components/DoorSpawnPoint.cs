using AssetBundleLoader.Components.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetBundleLoader.Components
{
    public class DoorSpawnPoint : MonoBehaviour
    {
        public bool IsOpen;
        public bool IsDestroyed;

        public DoorType DoorType = DoorType.HCZ;

        public DoorLockReason CurrentLock = DoorLockReason.None;

        public DoorDamageType IgnoredDamageSources = DoorDamageType.None;

        public bool RequireAllPermissions;
        public KeycardPermissions RequiredPermissions = KeycardPermissions.None;
    }
}
