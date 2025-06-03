using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundFader : MonoBehaviour
{
    public Image backgroundImage; // Ảnh nền (UI Image)
    public float fadeDuration = 1f;

    public void ChangeBackground(Sprite newSprite)
    {
        StartCoroutine(FadeTransition(newSprite));
    }

    IEnumerator FadeTransition(Sprite newSprite)
    {
        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));
        // Đổi ảnh nền
        backgroundImage.sprite = newSprite;
        // Fade in
        yield return StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color c = backgroundImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            backgroundImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        backgroundImage.color = new Color(c.r, c.g, c.b, endAlpha);
    }
}
