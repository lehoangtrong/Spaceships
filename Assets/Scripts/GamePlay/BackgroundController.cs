using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public float scrollSpeed = 2f;
    private Transform[] backgrounds;
    private float backgroundHeight;

    void Start()
    {
        // Lấy tất cả các background con
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i] = transform.GetChild(i);
        }

        // Giả sử tất cả background có cùng chiều cao
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // Di chuyển từng background lên
            backgrounds[i].position += Vector3.up * scrollSpeed * Time.fixedDeltaTime;
        }

        // Kiểm tra nếu background vượt khỏi vị trí hiển thị thì đưa về dưới cùng
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].position.y - backgroundHeight / 2 > Camera.main.transform.position.y + Camera.main.orthographicSize)
            {
                // Tìm background có y thấp nhất
                float lowestY = backgrounds[0].position.y;
                for (int j = 1; j < backgrounds.Length; j++)
                {
                    if (backgrounds[j].position.y < lowestY)
                    {
                        lowestY = backgrounds[j].position.y;
                    }
                }

                // Đưa background này xuống dưới cùng
                backgrounds[i].position = new Vector3(
                    backgrounds[i].position.x,
                    lowestY - backgroundHeight,
                    backgrounds[i].position.z
                );
            }
        }
    }
}
