using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel; // Reference to the loading panel in the UI

    private void Start()
    {
        loadingPanel.SetActive(true);
        LoadScene("Menu");
    }

    /// <summary>
    /// Starts the async loading of a scene with the given name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Coroutine to load a scene asynchronously.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        // Display the loading panel
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        // Use CustomSceneManager to load the scene asynchronously
        AsyncOperation asyncOperation = CustomSceneManager.LoadSceneAsync(sceneName);
        // Wait until the scene is fully loaded
        while (!asyncOperation.isDone)
        {
            yield return null; // Wait for the next frame
        }

        // Hide the loading panel after the scene is loaded
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

    }
}
