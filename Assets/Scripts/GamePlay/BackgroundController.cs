using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public float scrollSpeed = 2f;
    private Transform[] backgrounds;
    private float backgroundHeight;

    void Start()
    {
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i] = transform.GetChild(i);
        }

        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // Di chuyển từng background xuống
            backgrounds[i].position += Vector3.down * scrollSpeed * Time.fixedDeltaTime;
        }

        // Kiểm tra nếu background vượt khỏi vị trí hiển thị thì đưa về trên cùng
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].position.y + backgroundHeight / 2 < Camera.main.transform.position.y - Camera.main.orthographicSize)
            {
                // Tìm background có y cao nhất
                float highestY = backgrounds[0].position.y;
                for (int j = 1; j < backgrounds.Length; j++)
                {
                    if (backgrounds[j].position.y > highestY)
                    {
                        highestY = backgrounds[j].position.y;
                    }
                }

                // Đưa background này lên trên cùng
                backgrounds[i].position = new Vector3(
                    backgrounds[i].position.x,
                    highestY + backgroundHeight,
                    backgrounds[i].position.z
                );
            }
        }
    }
}
