using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScenePlay : MonoBehaviour
{
    public float delay = 0f;

    public void PlayNextScene()
    {
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("no scene next in build settings");
        }
    }
}

