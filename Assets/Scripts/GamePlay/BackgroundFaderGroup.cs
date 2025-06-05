using UnityEngine;
using System.Collections;

public class BackgroundFaderGroup : MonoBehaviour
{
    public float fadeDuration = 1f;

    private SpriteRenderer[] sprites;

    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetAlpha(float alpha)
    {
        foreach (var sr in sprites)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    public IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = sprites.Length > 0 ? sprites[0].color.a : 1f;
        float t = 0f;

        while (t < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            SetAlpha(newAlpha);
            t += Time.deltaTime;
            yield return null;
        }

        SetAlpha(targetAlpha);
    }
}
