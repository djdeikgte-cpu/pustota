using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TapOrDie.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public enum Language
        {
            EN,
            RU
        }

        [SerializeField] private Language defaultLanguage = Language.EN;
        [SerializeField] private Language currentLanguage;

        private Dictionary<string, Dictionary<Language, string>> translations;

        public Language CurrentLanguage => currentLanguage;

        #region External JS Functions

        [DllImport("__Internal")]
        private static extern string GetYandexLanguage();

        #endregion

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeTranslations();
                DetectLanguage();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeTranslations()
        {
            translations = new Dictionary<string, Dictionary<Language, string>>
            {
                // Main Menu
                ["play"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Play" },
                    { Language.RU, "Играть" }
                },
                ["settings"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Settings" },
                    { Language.RU, "Настройки" }
                },

                // Gameplay
                ["score"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Score" },
                    { Language.RU, "Счёт" }
                },
                ["high_score"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Best" },
                    { Language.RU, "Рекорд" }
                },
                ["pause"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Pause" },
                    { Language.RU, "Пауза" }
                },
                ["resume"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Resume" },
                    { Language.RU, "Продолжить" }
                },
                ["restart"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Restart" },
                    { Language.RU, "Заново" }
                },
                ["menu"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Menu" },
                    { Language.RU, "Меню" }
                },

                // Game Over
                ["game_over"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Game Over" },
                    { Language.RU, "Игра окончена" }
                },
                ["new_record"] = new Dictionary<Language, string>
                {
                    { Language.EN, "New Record!" },
                    { Language.RU, "Новый рекорд!" }
                },
                ["watch_ad_continue"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Watch Ad to Continue" },
                    { Language.RU, "Смотреть рекламу" }
                },
                ["extra_life"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Extra Life!" },
                    { Language.RU, "Доп. жизнь!" }
                },

                // Tutorial
                ["tutorial"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Tap to Jump!\nAvoid Obstacles!" },
                    { Language.RU, "Нажми чтобы прыгнуть!\nИзбегай препятствий!" }
                },

                // UI Elements
                ["music"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Music" },
                    { Language.RU, "Музыка" }
                },
                ["sound"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Sound" },
                    { Language.RU, "Звук" }
                },
                ["on"] = new Dictionary<Language, string>
                {
                    { Language.EN, "On" },
                    { Language.RU, "Вкл" }
                },
                ["off"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Off" },
                    { Language.RU, "Выкл" }
                },

                // Speed Warning
                ["speed_up"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Speed Up!" },
                    { Language.RU, "Ускорение!" }
                },

                // Loading
                ["loading"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Loading..." },
                    { Language.RU, "Загрузка..." }
                },
                ["tap_to_start"] = new Dictionary<Language, string>
                {
                    { Language.EN, "Tap to Start" },
                    { Language.RU, "Нажми чтобы начать" }
                }
            };
        }

        private void DetectLanguage()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                string langCode = GetYandexLanguage();
                SetLanguageFromCode(langCode);
            }
            catch
            {
                // Fallback to default
                currentLanguage = defaultLanguage;
            }
#else
            // In editor, use system language or default
            if (Application.systemLanguage == SystemLanguage.Russian)
            {
                currentLanguage = Language.RU;
            }
            else
            {
                currentLanguage = defaultLanguage;
            }
#endif
            Debug.Log($"[Localization] Language set to: {currentLanguage}");
        }

        private void SetLanguageFromCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                currentLanguage = defaultLanguage;
                return;
            }

            code = code.ToLower().Trim();

            switch (code)
            {
                case "ru":
                case "rus":
                case "russian":
                    currentLanguage = Language.RU;
                    break;
                case "en":
                case "eng":
                case "english":
                default:
                    currentLanguage = Language.EN;
                    break;
            }
        }

        public string GetText(string key)
        {
            if (translations.TryGetValue(key, out var langDict))
            {
                if (langDict.TryGetValue(currentLanguage, out string text))
                {
                    return text;
                }

                // Fallback to English
                if (langDict.TryGetValue(Language.EN, out string fallback))
                {
                    return fallback;
                }
            }

            Debug.LogWarning($"[Localization] Missing translation for key: {key}");
            return key;
        }

        public void SetLanguage(Language language)
        {
            currentLanguage = language;
            // Trigger UI refresh event if needed
        }

        public void ToggleLanguage()
        {
            if (currentLanguage == Language.EN)
                currentLanguage = Language.RU;
            else
                currentLanguage = Language.EN;
        }

        /// <summary>
        /// Add custom translation at runtime
        /// </summary>
        public void AddTranslation(string key, Language language, string text)
        {
            if (!translations.ContainsKey(key))
            {
                translations[key] = new Dictionary<Language, string>();
            }
            translations[key][language] = text;
        }
    }
}
