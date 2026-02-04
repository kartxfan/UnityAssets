using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SettingsManager : MonoBehaviour
{
    [Header("Элементы настроек")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle soundToggle;
    public Toggle musicToggle;
    public TMP_Dropdown languageDropdown;
    public Button closeButton;
    public Button resetButton;

    [Header("Диалог подтверждения")]
    public GameObject confirmationDialog;
    public TextMeshProUGUI timerText;
    public Button confirmButton;
    public Button cancelButton;

    [Header("Цвета переключателей")]
    public Color enabledColor = Color.green;
    public Color disabledColor = new Color(0.5f, 0, 0.5f); // фиолетовый

    private bool settingsChanged = false;
    private float confirmationTime = 10f;
    private Coroutine timerCoroutine;

    void Start()
    {
        // Загрузка сохраненных настроек
        LoadSettings();

        // Подписка на изменения
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(OnSettingChanged);
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnSettingChanged);
        if (soundToggle != null)
            soundToggle.onValueChanged.AddListener(OnSettingChanged);
        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnSettingChanged);
        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(OnSettingChanged);

        // Кнопки
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClicked);
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);

        // Скрываем диалог
        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);

        // Настройка цветов переключателей
        UpdateToggleColors();
    }

    void LoadSettings()
    {
        // Разрешение
        int savedWidth = PlayerPrefs.GetInt("ScreenWidth", 1920);
        int savedHeight = PlayerPrefs.GetInt("ScreenHeight", 1080);

        if (resolutionDropdown != null)
        {
            // Находим индекс сохраненного разрешения
            for (int i = 0; i < resolutionDropdown.options.Count; i++)
            {
                if (resolutionDropdown.options[i].text.Contains(savedWidth.ToString()))
                {
                    resolutionDropdown.value = i;
                    break;
                }
            }
        }

        // Полноэкранный режим
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        // Звук
        if (soundToggle != null)
            soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;

        // Музыка
        if (musicToggle != null)
            musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        // Язык
        if (languageDropdown != null)
            languageDropdown.value = PlayerPrefs.GetInt("Language", 0);

        settingsChanged = false;
    }

    void SaveSettings()
    {
        // Разрешение
        if (resolutionDropdown != null)
        {
            string res = resolutionDropdown.options[resolutionDropdown.value].text;
            string[] parts = res.Split('x');
            if (parts.Length == 2)
            {
                int width = int.Parse(parts[0].Trim());
                int height = int.Parse(parts[1].Trim());

                PlayerPrefs.SetInt("ScreenWidth", width);
                PlayerPrefs.SetInt("ScreenHeight", height);

                // Применяем разрешение
                Screen.SetResolution(width, height, fullscreenToggle.isOn);
            }
        }

        // Полноэкранный режим
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);

        // Звук
        PlayerPrefs.SetInt("SoundEnabled", soundToggle.isOn ? 1 : 0);

        // Музыка
        PlayerPrefs.SetInt("MusicEnabled", musicToggle.isOn ? 1 : 0);

        // Язык
        PlayerPrefs.SetInt("Language", languageDropdown.value);

        PlayerPrefs.Save();
        settingsChanged = false;

        Debug.Log("Настройки сохранены!");
    }

    void OnSettingChanged<T>(T value)
    {
        settingsChanged = true;
        UpdateToggleColors();
    }

    void UpdateToggleColors()
    {
        // Меняем цвет фона тогглов
        UpdateToggleColor(fullscreenToggle);
        UpdateToggleColor(soundToggle);
        UpdateToggleColor(musicToggle);
    }

    void UpdateToggleColor(Toggle toggle)
    {
        if (toggle != null)
        {
            ColorBlock colors = toggle.colors;
            colors.normalColor = toggle.isOn ? enabledColor : disabledColor;
            colors.selectedColor = toggle.isOn ? enabledColor : disabledColor;
            colors.highlightedColor = toggle.isOn ? enabledColor : disabledColor;
            colors.pressedColor = toggle.isOn ? enabledColor : disabledColor;
            toggle.colors = colors;
        }
    }

    void OnCloseClicked()
    {
        if (settingsChanged)
        {
            // Показываем диалог подтверждения
            if (confirmationDialog != null)
            {
                confirmationDialog.SetActive(true);
                StartTimer();
            }
        }
        else
        {
            // Закрываем окно
            gameObject.SetActive(false);
        }
    }

    void OnResetClicked()
    {
        // Сброс к настройкам по умолчанию
        PlayerPrefs.DeleteKey("ScreenWidth");
        PlayerPrefs.DeleteKey("ScreenHeight");
        PlayerPrefs.DeleteKey("Fullscreen");
        PlayerPrefs.DeleteKey("SoundEnabled");
        PlayerPrefs.DeleteKey("MusicEnabled");
        PlayerPrefs.DeleteKey("Language");

        LoadSettings();
        SaveSettings();
    }

    void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        confirmationTime = 10f;

        while (confirmationTime > 0)
        {
            if (timerText != null)
                timerText.text = $"({Mathf.CeilToInt(confirmationTime)}s)";

            confirmationTime -= Time.deltaTime;
            yield return null;
        }

        // Время вышло - отмена
        OnCancelClicked();
    }

    void OnConfirmClicked()
    {
        // Сохраняем настройки
        SaveSettings();

        // Закрываем диалог и окно
        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);

        gameObject.SetActive(false);
    }

    void OnCancelClicked()
    {
        // Отменяем изменения
        LoadSettings();

        // Закрываем диалог и окно
        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);

        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        // Отписываемся от событий
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.RemoveAllListeners();
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveAllListeners();
        if (soundToggle != null)
            soundToggle.onValueChanged.RemoveAllListeners();
        if (musicToggle != null)
            musicToggle.onValueChanged.RemoveAllListeners();
        if (languageDropdown != null)
            languageDropdown.onValueChanged.RemoveAllListeners();

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }
}