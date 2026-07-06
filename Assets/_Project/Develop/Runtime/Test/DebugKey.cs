using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System.Collections;
using UnityEngine;

/// <summary>
/// Скрипт для отладки перехода на сцену Entrance по нажатию пробела.
/// Вешается на сцену MainMenu.
/// </summary>
public class DebugKey : MonoBehaviour
{
    private SceneSwitcherService _sceneSwitcherService;
    private ICoroutinesPerformer _coroutinesPerformer; // Если у вас есть такой интерфейс

    private void Start()
    {
        // Получаем SceneSwitcherService из DI контейнера
        // Используем FindObjectOfType для получения Bootstrap'а текущей сцены
        var bootstrap = FindFirstObjectByType<SceneBootstrap>();
        if (bootstrap == null)
        {
            Debug.LogError("[DebugKey] SceneBootstrap не найден на сцене MainMenu!");
            return;
        }

        // Получаем контейнер через рефлексию или публичное поле
        // Вариант 1: если у вас есть публичное поле в Bootstrap
        var container = GetContainerFromBootstrap(bootstrap);
        if (container != null)
        {
            _sceneSwitcherService = container.Resolve<SceneSwitcherService>();
            _coroutinesPerformer = container.Resolve<ICoroutinesPerformer>();
        }
        else
        {
            Debug.LogError("[DebugKey] Не удалось получить DI контейнер!");
        }
    }

    private DIContainer GetContainerFromBootstrap(SceneBootstrap bootstrap)
    {
        // Используем рефлексию для получения приватного поля, если оно есть
        var field = bootstrap.GetType().GetField("_container",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
            return field.GetValue(bootstrap) as DIContainer;

        // Если поле не найдено, пробуем получить через публичное свойство
        var property = bootstrap.GetType().GetProperty("Container");
        if (property != null)
            return property.GetValue(bootstrap) as DIContainer;

        return null;
    }

    private void Update()
    {
        // Проверяем нажатие пробела
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_sceneSwitcherService == null)
            {
                Debug.LogError("[DebugKey] SceneSwitcherService не инициализирован!");
                return;
            }

            Debug.Log("[DebugKey] Переход на сцену Entrance");

            // Запускаем переход через корутину
            if (_coroutinesPerformer != null)
                _coroutinesPerformer.StartPerform(SwitchToEntrance());
            else
                StartCoroutine(SwitchToEntrance());
        }
    }

    private IEnumerator SwitchToEntrance()
    {
        // Используем существующий метод ProcessSwitchTo для загрузки сцены
        yield return _sceneSwitcherService.ProcessSwitchTo(Scenes.Entrance);

        // После загрузки подъезда запускаем фоновую загрузку лифта
        //_sceneSwitcherService.StartPreloadElevator();

        Debug.Log("[DebugKey] Сцена Entrance загружена, лифт начал загружаться в фоне");
    }
}