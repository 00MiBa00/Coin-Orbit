using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Models.General;
using Models.Utility;
using Helpers;
using Unity.VisualScripting;

namespace Utility
{
    public static class CoinAPIUtility
    {
        private static MonoBehaviour coroutineHost;

        private static Dictionary<string, CachedPriceData> priceCache = new Dictionary<string, CachedPriceData>();
        private static List<ItemCoinModel> allCoinsCache = new List<ItemCoinModel>();
        private static List<CoinMarketModel> allCoinsCacheMarket = new List<CoinMarketModel>();
        private static DateTime lastCacheUpdate = DateTime.MinValue;

        private const float PriceCacheDurationSeconds = 30f;
        private const float CoinCacheDurationSeconds = 30f;
        
        private static bool isLoadingAllCoins = false;
        private static Action pendingOnComplete = null;

        private class CachedPriceData
        {
            public float Price;
            public DateTime Timestamp;
            public bool IsValid() => (DateTime.UtcNow - Timestamp).TotalSeconds <= PriceCacheDurationSeconds;
        }

        public static void Initialize(MonoBehaviour host)
        {
            coroutineHost = host;
        }

        public static void EnsureCacheFresh(Action onComplete)
        {
            if (coroutineHost == null)
            {
                Debug.LogError("❗ CoinAPIUtility not initialized.");
                return;
            }

            if (allCoinsCache.Count > 0 && (DateTime.UtcNow - lastCacheUpdate).TotalSeconds <= CoinCacheDurationSeconds)
            {
                onComplete?.Invoke();
                return;
            }

            if (isLoadingAllCoins)
            {
                pendingOnComplete += onComplete;
                return;
            }

            isLoadingAllCoins = true;
            pendingOnComplete = onComplete;
            coroutineHost.StartCoroutine(LoadAllCoinsToCache(() =>
            {
                isLoadingAllCoins = false;
                pendingOnComplete?.Invoke();
                pendingOnComplete = null;
            }));
        }
        
        public static CoinMarketModel GetCoinMarketModelById(string id)
        {
            return allCoinsCacheMarket?.Find(c => c.id == id);
        }
        
        public static List<ItemCoinModel> GetTopCoinsFromCache(int count)
        {
            string[] bannedKeywords = new[] { "wrapped", "bridged", "restaked", "staked", "wormhole", "binance-peg", "usd+" };

            var filtered = allCoinsCache
                .Where(c =>
                {
                    string name = c.FullName?.ToLowerInvariant() ?? "";
                    return !bannedKeywords.Any(keyword => name.Contains(keyword));
                })
                .DistinctBy(c => c.ShortName) // убрать дубликаты вроде 3 WBTC
                .Take(count)
                .ToList();

            return filtered;
        }


        public static List<ItemCoinModel> SearchCoinsFromCache(string query, int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<ItemCoinModel>();

            return allCoinsCache
                .Where(c =>
                    (!string.IsNullOrEmpty(c.FullName) && c.FullName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(c.ShortName) && c.ShortName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0))
                .OrderByDescending(c => c.Cost) 
                .Take(maxResults)
                .ToList();
        }
        
        public static ItemCoinModel GetCoinModelFromCache(string id)
        {
            return allCoinsCache.FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public static void GetCoinPriceUSD(string coinId, Action<float?> onResult)
        {
            if (coroutineHost == null)
            {
                Debug.LogError("❗ CoinAPIUtility not initialized.");
                return;
            }

            string normalizedId = coinId.ToLowerInvariant();

            if (priceCache.TryGetValue(normalizedId, out var cached))
            {
                if (cached.IsValid())
                {
                    Debug.Log($"💰 Using cached price for {normalizedId}: {cached.Price}");
                    onResult?.Invoke(cached.Price);
                    return;
                }
                else
                {
                    Debug.Log($"⏳ Price for {normalizedId} expired, refreshing...");
                }
            }
            else
            {
                Debug.Log($"📥 No cached price for {normalizedId}, requesting...");
            }

            coroutineHost.StartCoroutine(FetchCoinPrice(normalizedId, onResult));
        }

        public static void LoadOrDownloadIcon(string coinId, string imageUrl, Action<Sprite> onLoaded)
        {
            string filePath = GetIconPath(coinId);

            if (File.Exists(filePath))
            {
                try
                {
                    byte[] data = File.ReadAllBytes(filePath);
                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(data);
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    onLoaded?.Invoke(sprite);
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError($"❌ Failed to load cached icon for {coinId}: {e.Message}");
                }
            }

            if (coroutineHost != null)
                coroutineHost.StartCoroutine(DownloadAndCacheIcon(coinId, imageUrl, onLoaded));
            else
                onLoaded?.Invoke(GetDefaultIcon());
        }

        private static IEnumerator LoadAllCoinsToCache(Action onComplete)
        {
            allCoinsCache.Clear();
            lastCacheUpdate = DateTime.UtcNow;

            int page = 1;
            int maxPages = 1;
            int perPage = 90;

            List<CoinMarketModel> allCoinsRaw = new List<CoinMarketModel>();

            while (page <= maxPages)
            {
                string url = $"https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page={perPage}&page={page}";
                UnityWebRequest request = UnityWebRequest.Get(url);
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Coin fetch failed: " + request.error);
                    break;
                }

                var json = request.downloadHandler.text;
                List<CoinMarketModel> coins = JsonHelper.FromJsonArray<CoinMarketModel>(json);

                if (coins == null || coins.Count == 0) break;

                allCoinsRaw.AddRange(coins);

                page++;
                yield return new WaitForSeconds(0.25f);
            }

            int loaded = 0;
            int total = allCoinsRaw.Count;
            
            allCoinsCacheMarket = allCoinsRaw;

            foreach (var coin in allCoinsRaw)
            {
                LoadOrDownloadIcon(coin.id, coin.image, sprite =>
                {
                    var model = new ItemCoinModel(
                        coin.id,
                        coin.name,
                        coin.symbol.ToUpper(),
                        coin.current_price,
                        sprite,
                        false,
                        coin.price_change_percentage_24h
                    );

                    allCoinsCache.Add(model);
                    loaded++;

                    if (loaded == total)
                        onComplete?.Invoke();
                });
            }

            if (allCoinsRaw.Count == 0)
                onComplete?.Invoke();
        }

        private static IEnumerator DownloadAndCacheIcon(string coinId, string url, Action<Sprite> onLoaded)
        {
            if (string.IsNullOrEmpty(url))
            {
                onLoaded?.Invoke(GetDefaultIcon());
                yield break;
            }

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"⚠️ Icon download failed for {coinId}: {request.error}");
                onLoaded?.Invoke(GetDefaultIcon());
                yield break;
            }

            string contentType = request.GetResponseHeader("Content-Type");
            if (string.IsNullOrEmpty(contentType) || !contentType.StartsWith("image/"))
            {
                Debug.LogWarning($"⚠️ Unsupported content type for {coinId}: {contentType}");
                onLoaded?.Invoke(GetDefaultIcon());
                yield break;
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            if (tex == null || tex.width == 0 || tex.height == 0)
            {
                Debug.LogWarning($"⚠️ Icon decode failed for {coinId}: Texture invalid");
                onLoaded?.Invoke(GetDefaultIcon());
                yield break;
            }

            try
            {
                byte[] png = tex.EncodeToPNG();
                File.WriteAllBytes(GetIconPath(coinId), png);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"⚠️ Failed to cache icon for {coinId}: {e.Message}");
            }

            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            onLoaded?.Invoke(sprite);
        }

        private static IEnumerator FetchCoinPrice(string coinId, Action<float?> onResult)
        {
            string url = $"https://api.coingecko.com/api/v3/simple/price?ids={coinId}&vs_currencies=usd";
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ Price fetch failed for {coinId}: {request.error}");
                onResult?.Invoke(null);
                yield break;
            }

            string json = request.downloadHandler.text;
            string pattern = $"\"{coinId}\":{{\"usd\":";
            int index = json.IndexOf(pattern);

            if (index >= 0)
            {
                int start = index + pattern.Length;
                int end = json.IndexOf("}", start);
                string priceStr = json.Substring(start, end - start);

                if (float.TryParse(priceStr, out float price))
                {
                    priceCache[coinId] = new CachedPriceData
                    {
                        Price = price,
                        Timestamp = DateTime.UtcNow
                    };

                    Debug.Log($"✅ Price fetched and cached for {coinId}: {price}");
                    onResult?.Invoke(price);
                    yield break;
                }
            }

            Debug.LogWarning($"⚠️ Failed to parse price for {coinId}");
            onResult?.Invoke(null);
        }


        private static string GetIconPath(string coinId)
        {
            string safeId = coinId.Replace("/", "_").Replace("\\", "_");
            return Path.Combine(Application.persistentDataPath, $"{safeId}.png");
        }

        private static Sprite defaultIcon;
        public static Sprite GetDefaultIcon() => defaultIcon;
    }
}