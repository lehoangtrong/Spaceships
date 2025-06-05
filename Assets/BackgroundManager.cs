using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] roundBackgrounds;

    private int currentIndex = -1;

    public void SwitchTo(int roundIndex)
    {
        if (roundIndex == currentIndex) return;

        for (int i = 0; i < roundBackgrounds.Length; i++)
        {
            roundBackgrounds[i].SetActive(false);
        }

        if (roundIndex >= 0 && roundIndex < roundBackgrounds.Length)
        {
            roundBackgrounds[roundIndex].SetActive(true);
            currentIndex = roundIndex;
        }
    }
}
