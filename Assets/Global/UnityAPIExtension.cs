using UnityEngine;

namespace Global
{
    public static class UnityAPIExtension
    {
        public static T Instantiate<T>(this T go) where T : Object
        {
            return Object.Instantiate(go);
        }

        public static GameObject SetPosition(this GameObject go, Vector3 pos)
        {
            go.transform.position = pos;
            return go;
        }

        public static GameObject SetRotation(this GameObject go, Quaternion rot)
        {
            go.transform.rotation = rot;
            return go;
        }

        public static GameObject SetScale(this GameObject go, Vector3 localScale)
        {
            go.transform.localScale = localScale;
            return go;
        }

        public static GameObject SetParent(this GameObject go, Transform parent, bool worldPositionStays = true)
        {
            go.transform.SetParent(parent, worldPositionStays);
            return go;
        }

        public static GameObject SetName(this GameObject go, string name)
        {
            go.name = name;
            return go;
        }

        public static void Destroy(this Object go)
        {
            Object.Destroy(go);
        }
    }
}
