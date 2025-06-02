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

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

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
            Explode();
            //Destroy(collision.gameObject);
        }
    }

    void Explode()
    {
        if (explosionAndFlarePrefab != null)
            Instantiate(explosionAndFlarePrefab, transform.position, Quaternion.identity);

        // tạo ra ngôi sao
        if (starPrefab != null)
            Instantiate(starPrefab, transform.position, Quaternion.identity);  // Spawn ngôi sao

        Destroy(gameObject);
    }
}
