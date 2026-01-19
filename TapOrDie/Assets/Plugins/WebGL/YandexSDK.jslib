mergeInto(LibraryManager.library, {

    InitYandexSDK: function() {
        if (typeof YaGames === 'undefined') {
            console.warn('YaGames SDK not loaded');
            return;
        }

        YaGames.init().then(function(ysdk) {
            window.ysdk = ysdk;
            console.log('Yandex SDK initialized');

            // Get player info
            ysdk.getPlayer().then(function(player) {
                window.yandexPlayer = player;
                console.log('Player loaded');
            }).catch(function(err) {
                console.warn('Player error:', err);
            });

            // Get environment
            window.yandexEnv = ysdk.environment;
            console.log('Language:', ysdk.environment.i18n.lang);

        }).catch(function(err) {
            console.error('Yandex SDK init error:', err);
        });
    },

    ShowFullscreenAdv: function() {
        if (!window.ysdk) {
            console.warn('SDK not initialized');
            if (window.unityInstance) {
                window.unityInstance.SendMessage('YandexAdsManager', 'OnFullscreenAdError', 'SDK not initialized');
            }
            return;
        }

        window.ysdk.adv.showFullscreenAdv({
            callbacks: {
                onOpen: function() {
                    console.log('Fullscreen ad opened');
                    if (window.unityInstance) {
                        window.unityInstance.SendMessage('YandexAdsManager', 'OnFullscreenAdOpened');
                    }
                },
                onClose: function(wasShown) {
                    console.log('Fullscreen ad closed, wasShown:', wasShown);
                    if (window.unityInstance) {
                        window.unityInstance.SendMessage('YandexAdsManager', 'OnFullscreenAdClosed');
                    }
                },
                onError: function(error) {
                    console.error('Fullscreen ad error:', error);
                    if (window.unityInstance) {
                        window.unityInstance.SendMessage('YandexAdsManager', 'OnFullscreenAdError', error.toString());
                    }
                },
                onOffline: function() {
                    console.log('Fullscreen ad offline');
                    if (window.unityInstance) {
                        window.unityInstance.SendMessage('YandexAdsManager', 'OnFullscreenAdError', 'offline');
                    }
                }
            }
        });
    },

    ShowRewardedAdv: function() {
        if (!window.ysdk) {
            console.warn('SDK not initialized');
            if (window.unityInstance) {
                window.unityInstance.SendMessage('YandexAdsManager', 'OnRewardedAdError', 'SDK not initialized');
            }
            return;
        }

        window.ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: function() {
                    console.log('Rewarded ad opened');
                    if (window.unityInstance) {
                        window.unityInstance.SendMessage('YandexAdsManager', 'OnRewardedAdOpened');
                    }
                },
                onRewarded: function() {
                    console.log('Rewarded!');
                    if (window.unityInstance) {
                        window.unityInstance.SendMessage('YandexAdsManager', 'OnRewardedAdRewarded');
                    }
                },
                onClose: function() {
                    console.log('Rewarded ad closed');
                    if (window.unityInstance) {
                        window.unityInstance.SendMessage('YandexAdsManager', 'OnRewardedAdClosed');
                    }
                },
                onError: function(error) {
                    console.error('Rewarded ad error:', error);
                    if (window.unityInstance) {
                        window.unityInstance.SendMessage('YandexAdsManager', 'OnRewardedAdError', error.toString());
                    }
                }
            }
        });
    },

    ShowBannerAdv: function() {
        if (!window.ysdk) {
            console.warn('SDK not initialized');
            return;
        }

        window.ysdk.adv.showBannerAdv();
    },

    HideBannerAdv: function() {
        if (!window.ysdk) {
            console.warn('SDK not initialized');
            return;
        }

        window.ysdk.adv.hideBannerAdv();
    },

    GetBannerAdvStatus: function() {
        if (!window.ysdk) {
            return false;
        }

        var status = window.ysdk.adv.getBannerAdvStatus();
        return status.stickyAdvIsShowing || false;
    },

    GameReady: function() {
        if (!window.ysdk) {
            console.warn('SDK not initialized');
            return;
        }

        window.ysdk.features.LoadingAPI.ready();
        console.log('Game ready signal sent');
    },

    GameplayStart: function() {
        if (!window.ysdk) {
            return;
        }

        if (window.ysdk.features && window.ysdk.features.GameplayAPI) {
            window.ysdk.features.GameplayAPI.start();
        }
    },

    GameplayStop: function() {
        if (!window.ysdk) {
            return;
        }

        if (window.ysdk.features && window.ysdk.features.GameplayAPI) {
            window.ysdk.features.GameplayAPI.stop();
        }
    },

    GetYandexLanguage: function() {
        if (window.ysdk && window.ysdk.environment && window.ysdk.environment.i18n) {
            var lang = window.ysdk.environment.i18n.lang;
            var bufferSize = lengthBytesUTF8(lang) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(lang, buffer, bufferSize);
            return buffer;
        }

        // Fallback to browser language
        var browserLang = navigator.language || navigator.userLanguage || 'en';
        browserLang = browserLang.substring(0, 2);
        var bufferSize = lengthBytesUTF8(browserLang) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(browserLang, buffer, bufferSize);
        return buffer;
    },

    // Leaderboard functions (optional)
    SetLeaderboardScore: function(leaderboardName, score) {
        if (!window.ysdk) {
            console.warn('SDK not initialized');
            return;
        }

        var name = UTF8ToString(leaderboardName);

        window.ysdk.getLeaderboards().then(function(lb) {
            lb.setLeaderboardScore(name, score);
            console.log('Score set:', score);
        }).catch(function(err) {
            console.error('Leaderboard error:', err);
        });
    },

    // Cloud save functions (optional)
    SaveToCloud: function(key, data) {
        if (!window.yandexPlayer) {
            console.warn('Player not loaded');
            return;
        }

        var keyStr = UTF8ToString(key);
        var dataStr = UTF8ToString(data);
        var saveData = {};
        saveData[keyStr] = dataStr;

        window.yandexPlayer.setData(saveData).then(function() {
            console.log('Data saved to cloud');
        }).catch(function(err) {
            console.error('Cloud save error:', err);
        });
    },

    LoadFromCloud: function(key) {
        if (!window.yandexPlayer) {
            console.warn('Player not loaded');
            return null;
        }

        var keyStr = UTF8ToString(key);

        // This is async, would need callback pattern
        window.yandexPlayer.getData([keyStr]).then(function(data) {
            console.log('Data loaded:', data);
            // Would need to send message back to Unity
        }).catch(function(err) {
            console.error('Cloud load error:', err);
        });

        return null;
    }
});
