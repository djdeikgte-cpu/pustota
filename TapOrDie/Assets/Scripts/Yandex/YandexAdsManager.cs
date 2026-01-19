using UnityEngine;
using System;
using System.Runtime.InteropServices;
using TapOrDie.Audio;

namespace TapOrDie.Yandex
{
    public class YandexAdsManager : MonoBehaviour
    {
        public static YandexAdsManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool enableAds = true;
        [SerializeField] private float minTimeBetweenFullscreenAds = 60f; // Yandex requirement: minimum 1 minute

        private float lastFullscreenAdTime;
        private Action currentRewardCallback;
        private Action currentFullscreenCloseCallback;
        private bool isBannerShowing;
        private bool isAdShowing;

        public bool IsAdShowing => isAdShowing;
        public bool IsBannerShowing => isBannerShowing;

        #region External JS Functions

        [DllImport("__Internal")]
        private static extern void InitYandexSDK();

        [DllImport("__Internal")]
        private static extern void ShowFullscreenAdv();

        [DllImport("__Internal")]
        private static extern void ShowRewardedAdv();

        [DllImport("__Internal")]
        private static extern void ShowBannerAdv();

        [DllImport("__Internal")]
        private static extern void HideBannerAdv();

        [DllImport("__Internal")]
        private static extern bool GetBannerAdvStatus();

        [DllImport("__Internal")]
        private static extern void GameReady();

        [DllImport("__Internal")]
        private static extern void GameplayStart();

        [DllImport("__Internal")]
        private static extern void GameplayStop();

        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            lastFullscreenAdTime = -minTimeBetweenFullscreenAds; // Allow first ad immediately

#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                InitYandexSDK();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Yandex SDK init failed: {e.Message}");
            }
#else
            Debug.Log("[YandexAds] Running in Editor - ads disabled");
#endif
        }

        #region Public Methods

        /// <summary>
        /// Call when game is fully loaded and ready to play
        /// </summary>
        public void NotifyGameReady()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                GameReady();
                Debug.Log("[YandexAds] Game Ready notification sent");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"GameReady failed: {e.Message}");
            }
#endif
        }

        /// <summary>
        /// Call when gameplay starts
        /// </summary>
        public void NotifyGameplayStart()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                GameplayStart();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"GameplayStart failed: {e.Message}");
            }
#endif
        }

        /// <summary>
        /// Call when gameplay stops (pause, game over, etc.)
        /// </summary>
        public void NotifyGameplayStop()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                GameplayStop();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"GameplayStop failed: {e.Message}");
            }
#endif
        }

        /// <summary>
        /// Show fullscreen interstitial ad
        /// </summary>
        public void ShowFullscreenAd(Action onClose = null)
        {
            if (!enableAds)
            {
                Debug.Log("[YandexAds] Ads disabled");
                onClose?.Invoke();
                return;
            }

            // Check minimum time between ads
            if (Time.realtimeSinceStartup - lastFullscreenAdTime < minTimeBetweenFullscreenAds)
            {
                Debug.Log("[YandexAds] Too soon for another fullscreen ad");
                onClose?.Invoke();
                return;
            }

            currentFullscreenCloseCallback = onClose;

#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                isAdShowing = true;
                AudioManager.Instance?.OnAdStart();
                ShowFullscreenAdv();
                lastFullscreenAdTime = Time.realtimeSinceStartup;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ShowFullscreenAd failed: {e.Message}");
                OnFullscreenAdClosed();
            }
#else
            Debug.Log("[YandexAds] Fullscreen Ad would show here");
            // Simulate ad in editor
            StartCoroutine(SimulateAd(() => OnFullscreenAdClosed()));
#endif
        }

        /// <summary>
        /// Show rewarded video ad
        /// </summary>
        public void ShowRewardedAd(Action onRewarded)
        {
            if (!enableAds)
            {
                Debug.Log("[YandexAds] Ads disabled - granting reward anyway in editor");
#if UNITY_EDITOR
                onRewarded?.Invoke();
#endif
                return;
            }

            currentRewardCallback = onRewarded;

#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                isAdShowing = true;
                AudioManager.Instance?.OnAdStart();
                ShowRewardedAdv();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ShowRewardedAd failed: {e.Message}");
                OnRewardedAdError("Failed to show ad");
            }
#else
            Debug.Log("[YandexAds] Rewarded Ad would show here");
            // Simulate ad in editor
            StartCoroutine(SimulateAd(() =>
            {
                OnRewardedAdRewarded();
                OnRewardedAdClosed();
            }));
#endif
        }

        /// <summary>
        /// Show sticky banner ad
        /// </summary>
        public void ShowBanner()
        {
            if (!enableAds) return;

#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                ShowBannerAdv();
                isBannerShowing = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ShowBanner failed: {e.Message}");
            }
#else
            Debug.Log("[YandexAds] Banner would show here");
            isBannerShowing = true;
#endif
        }

        /// <summary>
        /// Hide sticky banner ad
        /// </summary>
        public void HideBanner()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                HideBannerAdv();
                isBannerShowing = false;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"HideBanner failed: {e.Message}");
            }
#else
            Debug.Log("[YandexAds] Banner would hide here");
            isBannerShowing = false;
#endif
        }

        /// <summary>
        /// Check if banner is currently showing
        /// </summary>
        public bool CheckBannerStatus()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                return GetBannerAdvStatus();
            }
            catch
            {
                return isBannerShowing;
            }
#else
            return isBannerShowing;
#endif
        }

        #endregion

        #region Callbacks from JavaScript

        // Called from JavaScript when fullscreen ad opens
        public void OnFullscreenAdOpened()
        {
            Debug.Log("[YandexAds] Fullscreen ad opened");
            isAdShowing = true;
            AudioManager.Instance?.OnAdStart();
            Time.timeScale = 0f;
        }

        // Called from JavaScript when fullscreen ad closes
        public void OnFullscreenAdClosed()
        {
            Debug.Log("[YandexAds] Fullscreen ad closed");
            isAdShowing = false;
            AudioManager.Instance?.OnAdEnd();
            Time.timeScale = 1f;
            currentFullscreenCloseCallback?.Invoke();
            currentFullscreenCloseCallback = null;
        }

        // Called from JavaScript when fullscreen ad fails
        public void OnFullscreenAdError(string error)
        {
            Debug.LogWarning($"[YandexAds] Fullscreen ad error: {error}");
            isAdShowing = false;
            AudioManager.Instance?.OnAdEnd();
            Time.timeScale = 1f;
            currentFullscreenCloseCallback?.Invoke();
            currentFullscreenCloseCallback = null;
        }

        // Called from JavaScript when rewarded ad opens
        public void OnRewardedAdOpened()
        {
            Debug.Log("[YandexAds] Rewarded ad opened");
            isAdShowing = true;
            AudioManager.Instance?.OnAdStart();
            Time.timeScale = 0f;
        }

        // Called from JavaScript when reward is earned
        public void OnRewardedAdRewarded()
        {
            Debug.Log("[YandexAds] Reward earned!");
            currentRewardCallback?.Invoke();
        }

        // Called from JavaScript when rewarded ad closes
        public void OnRewardedAdClosed()
        {
            Debug.Log("[YandexAds] Rewarded ad closed");
            isAdShowing = false;
            AudioManager.Instance?.OnAdEnd();
            Time.timeScale = 1f;
            currentRewardCallback = null;
        }

        // Called from JavaScript when rewarded ad fails
        public void OnRewardedAdError(string error)
        {
            Debug.LogWarning($"[YandexAds] Rewarded ad error: {error}");
            isAdShowing = false;
            AudioManager.Instance?.OnAdEnd();
            Time.timeScale = 1f;
            currentRewardCallback = null;
        }

        #endregion

        #region Editor Simulation

#if UNITY_EDITOR
        private System.Collections.IEnumerator SimulateAd(Action onComplete)
        {
            Debug.Log("[YandexAds] Simulating ad (2 seconds)...");
            isAdShowing = true;
            yield return new WaitForSecondsRealtime(2f);
            isAdShowing = false;
            onComplete?.Invoke();
        }
#endif

        #endregion

        public void SetAdsEnabled(bool enabled)
        {
            enableAds = enabled;
        }
    }
}
