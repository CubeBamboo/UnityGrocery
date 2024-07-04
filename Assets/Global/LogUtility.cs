using UnityEngine;

namespace Global
{
    public class LogUtility : MonoBehaviour
    {
        public static void DumpToUnityLogger(object obj)
        {
            Debug.Log(obj);
        }
    }
}
