using UnityEngine;

public class StarController : MonoBehaviour
{
    private float fallSpeed = 1f;
    private float destroyAfterSeconds = 10f;
    public int pointValue = 10;

    private void Start()
    {
        // Destroy the star after a certain time to prevent memory leaks
        Destroy(gameObject, destroyAfterSeconds);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player ăn sao!");  // Kiểm tra va chạm có xảy ra
            GameManager.Instance.AddScore(pointValue);
            Destroy(gameObject); // Hủy ngôi sao (KHÔNG hủy Player!)
        }
    }
}
