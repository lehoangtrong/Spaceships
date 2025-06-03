using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed = 1f;
    public float speedIncrement = 0.5f;
    public float maxSpeed = 10f; // Tốc độ tối đa
    private float timer = 0f;
    public float interval = 5f; // thời gian mỗi lần tăng
    public GameObject explosionAndFlarePrefab; // Hiệu ứng nổ
    public GameObject starPrefab;              // ngôi sao để spawn sau nổ
    public int scoreValue = 1;
    [Range(0f, 1f)]
    public float starSpawnChance = 0.5f;
    public Vector3 rotationSpeed = new Vector3(100f, 0f, 0f); // ← Thêm dòng này để khai báo biến
    void Update()
    {
        //transform.Translate(Vector3.down * speed * Time.deltaTime);
        //transform.Rotate(rotationSpeed * Time.deltaTime);
        // Di chuyển thẳng xuống
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        // Xoay quanh chính nó
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            speed += speedIncrement;
            speed = Mathf.Min(speed, maxSpeed); // Đảm bảo tốc độ không vượt quá tốc độ tối đa
            timer = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Laser") || collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(scoreValue);
            Explode();
            //Destroy(collision.gameObject);
        }
    }

    void Explode()
    {
        if (explosionAndFlarePrefab != null)
            Instantiate(explosionAndFlarePrefab, transform.position, Quaternion.identity);

        float randomChance = Random.value; // Giá trị ngẫu nhiên từ 0.0 đến 1.0
                                           //if (starPrefab != null && Random.value < starSpawnChance) // % cơ hội
        if (starPrefab != null) // % cơ hội

        {
            Instantiate(starPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
