using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeSceneLoader : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeTime = 1.0f;
    public string nextSceneName;

    private bool isFading = false;

    public void StartGame()
    {
        if (!isFading)
            StartCoroutine(FadeOutAndLoad());
    }

    IEnumerator FadeOutAndLoad()
    {
        isFading = true;

        float t = 0f;
        Color color = fadeImage.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            color.a = Mathf.Clamp01(t / fadeTime);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
