using AdminToys;
using AssetBundleLoader.Components;
using AssetBundleLoader.Components.Enums;
using Interactables.Interobjects;
using Mirror;
using PluginAPI.Core;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleLoader
{
    public class AssetBundlePrefab
    {
        public string PrefabName { get; set; }

        public GameObject PrefabObject { get; set; }

        public void Cache()
        {
            if (PrefabObject == null)
                return;

            Position = PrefabObject.transform.position;
            Rotation = PrefabObject.transform.eulerAngles;
            Scale = PrefabObject.transform.localScale;
        }

        public bool Load(AssetBundle bundle)
        {
            var prefab = bundle.LoadAsset<GameObject>(PrefabName);

            if (prefab == null)
            {
                Log.Error($"Prefab with name \"{PrefabName}\" not exists in bundle!");
                return false;
            }

            PrefabObject = UnityEngine.Object.Instantiate(prefab, Position, Quaternion.Euler(Rotation));
            PrefabObject.transform.localScale = Scale;
            PrefabObject.hideFlags = HideFlags.HideAndDontSave;

            foreach (var transform in PrefabObject.GetComponentsInChildren<Transform>())
            {
                if (transform.TryGetComponent<Collider>(out Collider col))
                {
                    UnityEngine.Object.Destroy(col);
                }

                if (transform.TryGetComponent<Light>(out Light light))
                {
                    PrefabObjects.Add(CreateLight(transform,
                        light.color,
                        light.intensity,
                        light.range,
                        light.shadows != LightShadows.None).gameObject);
                }

                if (transform.TryGetComponent<ShottingTargetSpawnPoint>(out ShottingTargetSpawnPoint shtarget))
                {
                    var target = UnityEngine.Object.Instantiate(shtarget.TargetType == Components.Enums.ShottingTargetType.Binary ?
                        AssetBundleManager.BinaryShootingTarget : shtarget.TargetType == Components.Enums.ShottingTargetType.DBoy ?
                        AssetBundleManager.DboyShootingTarget :
                        AssetBundleManager.SportShootingTarget, transform.position, transform.rotation);

                    var shootingTarget = target.GetComponent<AdminToys.ShootingTarget>();
                    target.transform.localScale = transform.localScale;

                    shootingTarget.NetworkScale = target.transform.localScale;
                    shootingTarget.NetworkPosition = target.transform.position;
                    shootingTarget.NetworkRotation = new LowPrecisionQuaternion(target.transform.rotation);

                    shootingTarget.NetworkMovementSmoothing = 60;

                    NetworkServer.Spawn(shootingTarget.gameObject);
                    PrefabObjects.Add(target);
                }

                if (transform.TryGetComponent<DoorSpawnPoint>(out DoorSpawnPoint door))
                {
                    var door2 = UnityEngine.Object.Instantiate(door.DoorType == Components.Enums.DoorType.HCZ ? AssetBundleManager.HCZDoor
                         : door.DoorType == DoorType.LCZ ? AssetBundleManager.LCZDoor : AssetBundleManager.EZDoor, transform.position, transform.rotation);
                    door2.transform.localScale = transform.localScale;

                    if (door2.TryGetComponent<BreakableDoor>(out BreakableDoor breakableDoor))
                    {
                        breakableDoor.NetworkTargetState = door.IsOpen;
                        breakableDoor.Network_destroyed = door.IsDestroyed;
                        breakableDoor.RequiredPermissions.RequireAll = door.RequireAllPermissions;
                        breakableDoor.RequiredPermissions.RequiredPermissions = (Interactables.Interobjects.DoorUtils.KeycardPermissions)(ushort)door.RequiredPermissions;
                        //breakableDoor._ignoredDamageSources = (Interactables.Interobjects.DoorUtils.DoorDamageType)(byte)door.IgnoredDamageSources;
                        if (door.CurrentLock != Components.Enums.DoorLockReason.None)
                        {
                            breakableDoor.ServerChangeLock((Interactables.Interobjects.DoorUtils.DoorLockReason)(ushort)door.CurrentLock, true);
                        }
                        NetworkServer.Spawn(door2);
                        PrefabObjects.Add(door2);
                    }
                }

                if (transform.TryGetComponent<PrimitiveMetadata>(out PrimitiveMetadata metadata))
                {
                    PrefabObjects.Add(CreatePrimitive(
                        transform,
                        metadata.Type,
                        metadata.Color).gameObject);
                }
            }
            return true;
        }

        private static PrimitiveObjectToy primitiveBaseObject = null;

        public static PrimitiveObjectToy PrimitiveBaseObject
        {
            get
            {
                if (primitiveBaseObject == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<PrimitiveObjectToy>(out var component))
                            primitiveBaseObject = component;
                    }
                }

                return primitiveBaseObject;
            }
        }

        private static LightSourceToy primitiveBaseLight = null;

        public static LightSourceToy PrimitiveBaseLight
        {
            get
            {
                if (primitiveBaseLight == null)
                {
                    foreach (var gameObject in NetworkClient.prefabs.Values)
                    {
                        if (gameObject.TryGetComponent<LightSourceToy>(out var component))
                            primitiveBaseLight = component;
                    }
                }

                return primitiveBaseLight;
            }
        }

        public static PrimitiveObjectToy CreatePrimitive(Transform parent, PrimitiveType type, Color color)
        {
            PrimitiveObjectToy toy = null;
            if (parent.name.Contains("IGN"))
            {
                toy = UnityEngine.Object.Instantiate(PrimitiveBaseObject, parent.transform.position, parent.transform.rotation);
            }
            else
            {
                toy = UnityEngine.Object.Instantiate(PrimitiveBaseObject, parent.transform.position, parent.transform.rotation);
                //var con = toy.gameObject.AddComponent<ParentConstraint>();
                //con.AddSource(new ConstraintSource()
                //{
                //    sourceTransform = parent,
                //    weight = 1f,
                //});
                //con.constraintActive = true;
            }
            toy.transform.localScale = parent.localScale;


            toy.NetworkScale = toy.transform.localScale;
            toy.NetworkPosition = toy.transform.position;
            toy.NetworkRotation = new LowPrecisionQuaternion(toy.transform.rotation);

            toy.NetworkPrimitiveType = type;
            toy.NetworkMaterialColor = color;

            toy.NetworkMovementSmoothing = 60;
            toy.IsStatic = true;

            NetworkServer.Spawn(toy.gameObject);
            return toy;
        }


        public static LightSourceToy CreateLight(Transform parent, Color color, float intensity, float range, bool shadows)
        {
            LightSourceToy toy = null;

            if (parent.name.Contains("IGN"))
            {
                toy = UnityEngine.Object.Instantiate(PrimitiveBaseLight, parent.transform.position, parent.transform.rotation);
            }
            else
            {
                toy = UnityEngine.Object.Instantiate(PrimitiveBaseLight, parent.transform.position, parent.transform.rotation);

               // var con = toy.gameObject.AddComponent<ParentConstraint>();
               // con.AddSource(new ConstraintSource()
               // {
                //    sourceTransform = parent,
                //    weight = 1f,
                //});
                //con.constraintActive = true;
            }
            toy.transform.localScale = parent.localScale;

            toy.NetworkScale = toy.transform.localScale;
            toy.NetworkPosition = toy.transform.position;
            toy.NetworkRotation = new LowPrecisionQuaternion(toy.transform.rotation);

            toy.NetworkLightColor = color;
            toy.NetworkLightIntensity = intensity;
            toy.NetworkLightRange = range;
            toy.NetworkLightShadows = shadows;

            toy.NetworkMovementSmoothing = 60;

            NetworkServer.Spawn(toy.gameObject);
            return toy;
        }

        public void Unload()
        {
            if (PrefabObject != null)
                UnityEngine.Object.Destroy(PrefabObject);

            foreach (var created in PrefabObjects)
            {
                UnityEngine.Object.Destroy(created);
            }
            PrefabObjects.Clear();
        }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public List<GameObject> PrefabObjects { get; set; } = new List<GameObject>();
    }
}