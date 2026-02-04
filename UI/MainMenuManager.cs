using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Основные кнопки")]
    public Button playButton;
    public Button settingsButton;
    public Button shopButton;
    public Button exitButton;

    [Header("Монеты")]
    public TextMeshProUGUI coinText;

    [Header("Окна")]
    public GameObject settingsWindow;
    public GameObject shopWindow;

    [Header("Подсказки")]
    public GameObject tooltipPrefab;
    private GameObject currentTooltip;
    private Coroutine tooltipCoroutine;

    void Start()
    {
        // Привязка кнопок
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        if (shopButton != null)
            shopButton.onClick.AddListener(OnShopClicked);
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        // Загрузка монет
        UpdateCoinDisplay();

        // Скрываем окна
        if (settingsWindow != null) settingsWindow.SetActive(false);
        if (shopWindow != null) shopWindow.SetActive(false);
    }

    void OnPlayClicked()
    {
        Debug.Log("Загрузка игры...");
        SceneManager.LoadScene("Game");
    }

    void OnSettingsClicked()
    {
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(true);
        }
    }

    void OnShopClicked()
    {
        if (shopWindow != null)
        {
            shopWindow.SetActive(true);
        }
    }

    void OnExitClicked()
    {
        Debug.Log("Выход из игры");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void UpdateCoinDisplay()
    {
        if (coinText != null)
        {
            int coins = PlayerPrefs.GetInt("PlayerCoins", 1500);

            if (coins >= 1000)
            {
                coinText.text = (coins / 1000f).ToString("F1") + "k";
            }
            else
            {
                coinText.text = coins.ToString();
            }
        }
    }

    // Обновление монет из других скриптов
    public void RefreshCoins()
    {
        UpdateCoinDisplay();
    }

    void OnDestroy()
    {
        // Отписываемся от событий
        if (playButton != null) playButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (shopButton != null) shopButton.onClick.RemoveAllListeners();
        if (exitButton != null) exitButton.onClick.RemoveAllListeners();
    }
}