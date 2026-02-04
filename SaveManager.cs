using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [System.Serializable]
    public class UserData
    {
        public int coins = 1500;
        public int lastCompletedLevel = 0;
        public List<string> purchasedBackgrounds = new List<string>();
        public List<string> purchasedItems = new List<string>();
    }

    [System.Serializable]
    public class GameSettings
    {
        public int resolutionWidth = 1920;
        public int resolutionHeight = 1080;
        public bool fullscreen = true;
        public bool soundEnabled = true;
        public bool musicEnabled = true;
        public string language = "ru";
    }

    private UserData userData;
    private GameSettings gameSettings;
    private string savePath;

    void Awake()
    {
        // Паттерн Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        savePath = Application.persistentDataPath;
        Debug.Log("Путь сохранения: " + savePath);

        LoadAllData();
    }

    void LoadAllData()
    {
        userData = LoadData<UserData>("UserData.json") ?? new UserData();
        gameSettings = LoadData<GameSettings>("GameSettings.json") ?? new GameSettings();

        // Если файлов нет - сохраняем дефолтные
        if (!File.Exists(Path.Combine(savePath, "UserData.json")))
            SaveUserData();
        if (!File.Exists(Path.Combine(savePath, "GameSettings.json")))
            SaveGameSettings();
    }

    T LoadData<T>(string fileName) where T : new()
    {
        string filePath = Path.Combine(savePath, fileName);

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка загрузки {fileName}: {e.Message}");
                return new T();
            }
        }

        return new T();
    }

    void SaveData<T>(T data, string fileName)
    {
        string filePath = Path.Combine(savePath, fileName);

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Сохранено в {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка сохранения {fileName}: {e.Message}");
        }
    }

    // === ПУБЛИЧНЫЕ МЕТОДЫ ===

    public void SaveUserData()
    {
        SaveData(userData, "UserData.json");
    }

    public void SaveGameSettings()
    {
        SaveData(gameSettings, "GameSettings.json");
    }

    // Монеты
    public int GetCoins() => userData.coins;

    public void AddCoins(int amount)
    {
        userData.coins += amount;
        SaveUserData();
        UpdateUI();
    }

    public bool SpendCoins(int amount)
    {
        if (userData.coins >= amount)
        {
            userData.coins -= amount;
            SaveUserData();
            UpdateUI();
            return true;
        }
        return false;
    }

    // Уровни
    public int GetCurrentLevel() => userData.lastCompletedLevel + 1;

    public void CompleteLevel(int level, int reward)
    {
        if (level > userData.lastCompletedLevel)
        {
            userData.lastCompletedLevel = level;
            AddCoins(reward);
        }
    }

    // Настройки
    public GameSettings GetSettings() => gameSettings;

    public void UpdateSettings(GameSettings newSettings)
    {
        gameSettings = newSettings;
        SaveGameSettings();
    }

    // Обновление UI
    void UpdateUI()
    {
        // Ищем все отображения монет и обновляем
        var coinDisplays = FindObjectsOfType<MainMenuManager>();
        foreach (var display in coinDisplays)
        {
            display.RefreshCoins();
        }
    }

    // Для тестирования
    [ContextMenu("Сбросить все данные")]
    public void ResetAllData()
    {
        userData = new UserData();
        gameSettings = new GameSettings();
        SaveUserData();
        SaveGameSettings();
        Debug.Log("Все данные сброшены!");
    }

    [ContextMenu("Добавить 1000 монет")]
    public void AddTestCoins()
    {
        AddCoins(1000);
    }
}