using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Основные настройки")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float propellerForce = 15f;

    [Header("Состояния")]
    public bool hasPropeller = false;
    public bool hasRocket = false;
    public bool hasSpikedWheels = false;
    public bool hasWings = false;

    [Header("Компоненты")]
    private Rigidbody rb;
    private bool isGrounded = true;
    private bool propellerActive = false;

    [Header("Префабы")]
    public GameObject rocketPrefab;
    public Transform firePoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Постоянное движение вперед
        rb.velocity = new Vector3(moveSpeed, rb.velocity.y, rb.velocity.z);

        // Активное использование пропеллера
        if (propellerActive && hasPropeller)
        {
            rb.AddForce(Vector3.up * propellerForce, ForceMode.Force);
        }
    }

    void Update()
    {
        // Проверка на земле ли транспорт
        CheckGrounded();

        // Управление с клавиатуры для теста
        TestControls();
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        float distance = 0.6f;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, distance);

        Debug.DrawRay(transform.position, Vector3.down * distance,
                     isGrounded ? Color.green : Color.red);
    }

    void TestControls()
    {
        // Тестовое управление (потом заменишь на UI кнопки)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (hasWings && isGrounded)
            {
                Jump();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (hasPropeller)
            {
                ActivatePropeller(true);
            }
        }
        else
        {
            ActivatePropeller(false);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (hasRocket)
            {
                FireRocket();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (hasSpikedWheels)
            {
                ToggleSpikedWheels();
            }
        }
    }

    // === ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ UI КНОПОК ===

    public void Jump()
    {
        if (isGrounded && hasWings)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Прыжок!");
        }
    }

    public void ActivatePropeller(bool activate)
    {
        propellerActive = activate;
        Debug.Log("Пропеллер: " + (activate ? "ВКЛ" : "ВЫКЛ"));
    }

    public void FireRocket()
    {
        if (hasRocket && rocketPrefab != null && firePoint != null)
        {
            Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
            hasRocket = false; // Одна ракета на уровень
            Debug.Log("Ракета выпущена!");
        }
    }

    public void ToggleSpikedWheels()
    {
        hasSpikedWheels = !hasSpikedWheels;
        Debug.Log("Шипованные колеса: " + (hasSpikedWheels ? "ВКЛ" : "ВЫКЛ"));
    }

    // Установка модулей
    public void InstallModule(string moduleType)
    {
        switch (moduleType)
        {
            case "Propeller":
                hasPropeller = true;
                break;
            case "Rocket":
                hasRocket = true;
                break;
            case "SpikedWheels":
                hasSpikedWheels = true;
                break;
            case "Wings":
                hasWings = true;
                break;
        }
        Debug.Log($"Установлен модуль: {moduleType}");
    }

    void OnCollisionEnter(Collision collision)
    {
        // Проверка финиша
        if (collision.gameObject.CompareTag("Finish"))
        {
            Debug.Log("Уровень пройден!");
            // Здесь вызов метода завершения уровня
        }

        // Проверка смерти
        if (collision.gameObject.CompareTag("Death"))
        {
            Debug.Log("Игрок погиб!");
            // Здесь вызов метода проигрыша
        }
    }
}