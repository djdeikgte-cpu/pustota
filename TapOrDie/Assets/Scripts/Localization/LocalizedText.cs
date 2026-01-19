using UnityEngine;
using TMPro;

namespace TapOrDie.Localization
{
    /// <summary>
    /// Component to automatically localize text in UI elements
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string localizationKey;
        [SerializeField] private bool updateOnEnable = true;

        private TextMeshProUGUI textComponent;

        private void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (updateOnEnable)
            {
                UpdateText();
            }
        }

        private void Start()
        {
            UpdateText();
        }

        public void UpdateText()
        {
            if (textComponent == null || string.IsNullOrEmpty(localizationKey)) return;

            if (LocalizationManager.Instance != null)
            {
                textComponent.text = LocalizationManager.Instance.GetText(localizationKey);
            }
        }

        public void SetKey(string key)
        {
            localizationKey = key;
            UpdateText();
        }
    }
}
