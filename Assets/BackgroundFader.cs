using UnityEngine;
using System.Collections;

public class BackgroundFader : MonoBehaviour
{
    public SpriteRenderer[] backgrounds; // Gán các background trong Inspector
    public float fadeDuration = 1f;
    private int currentIndex = 0;

    void Start()
    {
        // Hiển thị chỉ background hiện tại, còn lại ẩn
        for (int i = 0; i < backgrounds.Length; i++)
        {
            Color color = backgrounds[i].color;
            color.a = (i == currentIndex) ? 1f : 0f;
            backgrounds[i].color = color;
        }
    }

    public void SwitchTo(int newIndex)
    {
        if (newIndex >= backgrounds.Length || newIndex == currentIndex) return;
        StartCoroutine(FadeBackgrounds(currentIndex, newIndex));
        currentIndex = newIndex;
    }

    IEnumerator FadeBackgrounds(int fromIndex, int toIndex)
    {
        float time = 0f;

        SpriteRenderer from = backgrounds[fromIndex];
        SpriteRenderer to = backgrounds[toIndex];

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            from.color = new Color(1, 1, 1, Mathf.Lerp(1f, 0f, t));
            to.color = new Color(1, 1, 1, Mathf.Lerp(0f, 1f, t));
            time += Time.deltaTime;
            yield return null;
        }

        from.color = new Color(1, 1, 1, 0f);
        to.color = new Color(1, 1, 1, 1f);
    }
}
