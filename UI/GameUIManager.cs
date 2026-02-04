using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Кнопки управления")]
    public Button propellerButton;
    public Button rocketButton;
    public Button wheelsButton;
    public Button wingsButton;
    public Button homeButton;
    public Button restartButton;

    [Header("Ссылки")]
    public VehicleController vehicle;

    [Header("Состояния")]
    private bool propellerActive = false;
    private bool wheelsActive = false;

    void Start()
    {
        // Привязка кнопок
        if (propellerButton != null)
        {
            propellerButton.onClick.AddListener(OnPropellerClicked);
        }

        if (rocketButton != null)
        {
            rocketButton.onClick.AddListener(OnRocketClicked);
        }

        if (wheelsButton != null)
        {
            wheelsButton.onClick.AddListener(OnWheelsClicked);
        }

        if (wingsButton != null)
        {
            wingsButton.onClick.AddListener(OnWingsClicked);
        }

        if (homeButton != null)
        {
            homeButton.onClick.AddListener(OnHomeClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        // Находим транспорт если не назначен
        if (vehicle == null)
        {
            vehicle = FindObjectOfType<VehicleController>();
        }
    }

    void OnPropellerClicked()
    {
        if (vehicle != null)
        {
            propellerActive = !propellerActive;
            vehicle.ActivatePropeller(propellerActive);

            // Меняем цвет кнопки
            if (propellerButton != null)
            {
                ColorBlock colors = propellerButton.colors;
                colors.normalColor = propellerActive ? Color.green : Color.white;
                propellerButton.colors = colors;
            }
        }
    }

    void OnRocketClicked()
    {
        if (vehicle != null)
        {
            vehicle.FireRocket();

            // Делаем кнопку неактивной после выстрела
            if (rocketButton != null)
            {
                rocketButton.interactable = false;
            }
        }
    }

    void OnWheelsClicked()
    {
        if (vehicle != null)
        {
            vehicle.ToggleSpikedWheels();
            wheelsActive = !wheelsActive;

            // Меняем цвет кнопки
            if (wheelsButton != null)
            {
                ColorBlock colors = wheelsButton.colors;
                colors.normalColor = wheelsActive ? Color.green : Color.white;
                wheelsButton.colors = colors;
            }
        }
    }

    void OnWingsClicked()
    {
        if (vehicle != null)
        {
            vehicle.Jump();

            // Блокируем кнопку на 1 секунду
            if (wingsButton != null)
            {
                wingsButton.interactable = false;
                Invoke(nameof(EnableWingsButton), 1f);
            }
        }
    }

    void EnableWingsButton()
    {
        if (wingsButton != null)
        {
            wingsButton.interactable = true;
        }
    }

    void OnHomeClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        // Отписываемся от всех событий
        if (propellerButton != null) propellerButton.onClick.RemoveAllListeners();
        if (rocketButton != null) rocketButton.onClick.RemoveAllListeners();
        if (wheelsButton != null) wheelsButton.onClick.RemoveAllListeners();
        if (wingsButton != null) wingsButton.onClick.RemoveAllListeners();
        if (homeButton != null) homeButton.onClick.RemoveAllListeners();
        if (restartButton != null) restartButton.onClick.RemoveAllListeners();
    }
}