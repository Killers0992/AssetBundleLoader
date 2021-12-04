using AdminToys;
using Exiled.API.Features;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (transform.TryGetComponent<Light>(out Light light))
                {
                    PrefabObjects.Add(CreateLight(transform,
                        transform.position,
                        transform.eulerAngles,
                        transform.localScale,
                        light.color,
                        light.intensity,
                        light.range,
                        light.shadows != LightShadows.None).gameObject);
                }

                if (!transform.TryGetComponent<MeshFilter>(out MeshFilter filter))
                    continue;

                if (!transform.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
                    continue;

                PrimitiveType type = PrimitiveType.Sphere;

                switch (filter.mesh.name)
                {
                    case "Plane Instance":
                        type = PrimitiveType.Plane;
                        break;
                    case "Cube Instance":
                        type = PrimitiveType.Cube;
                        break;
                    case "Capsule Instance":
                        type = PrimitiveType.Capsule;
                        break;
                    case "Quad Instance":
                        type = PrimitiveType.Quad;
                        break;
                    case "Sphere Instance":
                        type = PrimitiveType.Sphere;
                        break;
                    default:
                        continue;
                }

                PrefabObjects.Add(CreatePrimitive(
                    transform,
                    type,
                    transform.position,
                    transform.eulerAngles,
                    transform.localScale,
                    renderer.material.color).gameObject);
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

        public static PrimitiveObjectToy CreatePrimitive(Transform parent, PrimitiveType type, Vector3 pos, Vector3 rot, Vector3 size, Color color)
        {
            AdminToyBase toy = UnityEngine.Object.Instantiate(PrimitiveBaseObject, parent);
            PrimitiveObjectToy ptoy = toy.GetComponent<PrimitiveObjectToy>();
            ptoy.NetworkPrimitiveType = type;
            ptoy.NetworkMaterialColor = color;
            ptoy.transform.position = pos;
            ptoy.transform.rotation = Quaternion.Euler(rot);
            ptoy.transform.localScale = size;
            ptoy.NetworkScale = ptoy.transform.localScale;
            NetworkServer.Spawn(toy.gameObject);
            return ptoy;
        }


        public static LightSourceToy CreateLight(Transform parent, Vector3 pos, Vector3 rot, Vector3 size, Color color, float intensity, float range, bool shadows)
        {
            AdminToyBase toy = UnityEngine.Object.Instantiate(PrimitiveBaseLight, parent);
            LightSourceToy ptoy = toy.GetComponent<LightSourceToy>();
            ptoy.NetworkLightColor = color;
            ptoy.NetworkLightIntensity = intensity;
            ptoy.NetworkLightRange = range;
            ptoy.NetworkLightShadows = shadows;
            ptoy.transform.position = pos;
            ptoy.transform.rotation = Quaternion.Euler(rot);
            ptoy.transform.localScale = size;
            ptoy.NetworkScale = ptoy.transform.localScale;
            NetworkServer.Spawn(toy.gameObject);
            return ptoy;
        }

        public void Unload()
        {
            if (PrefabObject != null)
                UnityEngine.Object.Destroy(PrefabObject);

            foreach(var created in PrefabObjects)
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
