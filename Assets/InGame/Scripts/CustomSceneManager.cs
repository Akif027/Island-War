using UnityEngine;
using UnityEngine.SceneManagement;
public static class CustomSceneManager
{
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Load a scene asynchronously by name
    public static AsyncOperation LoadSceneAsync(string sceneName)

    {

        // Your existing code to load the scene asynchronously

        return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

    }

    // Load a scene by index
    public static void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Load a scene asynchronously by index
    public static void LoadSceneAsync(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    // Load a scene additively by name (additive loading means the scene is added to the current one)
    public static void LoadSceneAdditive(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    // Load a scene additively by index
    public static void LoadSceneAdditive(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    }

    // Unload a scene by name
    public static void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    // Unload a scene by index
    public static void UnloadScene(int sceneIndex)
    {
        SceneManager.UnloadSceneAsync(sceneIndex);
    }

    // Get the active scene name
    public static string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    // Get the active scene index
    public static int GetActiveSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    // Reload the current scene
    public static void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // Load the next scene in the build index
    public static void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("This is the last scene in the build index.");
        }
    }

    // Load the previous scene in the build index
    public static void LoadPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = currentSceneIndex - 1;

        if (previousSceneIndex >= 0)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.LogWarning("This is the first scene in the build index.");
        }
    }

    // Check if a scene is loaded
    public static bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }

    // Set active scene by name
    public static void SetActiveScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.isLoaded)
        {
            SceneManager.SetActiveScene(scene);
        }
        else
        {
            Debug.LogWarning($"Scene {sceneName} is not loaded.");
        }
    }

    // Set active scene by index
    public static void SetActiveScene(int sceneIndex)
    {
        Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
        if (scene.isLoaded)
        {
            SceneManager.SetActiveScene(scene);
        }
        else
        {
            Debug.LogWarning($"Scene {sceneIndex} is not loaded.");
        }
    }
}
