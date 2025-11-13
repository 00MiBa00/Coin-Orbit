using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class JsonHelper
    {
        public static List<T> FromJsonArray<T>(string json)
        {
            string wrapped = "{\"Items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
            return new List<T>(wrapper.Items);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}