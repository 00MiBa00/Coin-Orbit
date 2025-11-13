using System.Collections;
using Types;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    public static class SceneLoaderUtility
    {
        public static void LoadScene(SceneType type, MonoBehaviour host)
        {
            host.StartCoroutine(DelayLoadScene(type));
        }

        private static IEnumerator DelayLoadScene(SceneType type)
        {
            yield return new WaitForSecondsRealtime(0.3f);

            SceneManager.LoadScene(type.ToString());
        }
    }
}