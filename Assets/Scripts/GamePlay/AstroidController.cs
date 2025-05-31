using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float speed;
    public GameObject explosionAndFlarePrefab; // Hiệu ứng nổ
    public GameObject starPrefab;              // ngôi sao để spawn sau nổ

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            Explode();
            Destroy(collision.gameObject);
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
