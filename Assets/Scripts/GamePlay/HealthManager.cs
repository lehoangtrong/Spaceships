using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static int health = 3; // Người chơi bắt đầu với 3 mạng

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    
    void Awake()
    {
        health = 3; // Đặt lại số mạng khi khởi tạo
    }

    // Update is called once per frame
    void Update()
    {
        // Đảm bảo giá trị health không âm
        health = Mathf.Max(0, health);
        
        foreach (Image img in hearts)
        {
            img.sprite = emptyHeart;
        }
        for (int i = 0; i < health; i++)
        {
            if (i < hearts.Length)
            {
                hearts[i].sprite = fullHeart;
            }
        }
    }

    // Thêm hàm để giảm mạng
    public static void LoseHeart()
    {
        health--;
    }

    // Thêm hàm để lấy số mạng hiện tại
    public static int GetHealth()
    {
        return health;
    }
}
