using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mouse Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float smoothingSpeed = 5f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found! Make sure your camera has the 'MainCamera' tag.");
        }
    }

    private void Update()
    {
        MoveTowardsMouse();
    }

    private void MoveTowardsMouse()
    {
        // Lấy vị trí chuột trong không gian thế giới
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -mainCamera.transform.position.z));

        // Giữ nguyên vị trí Z của máy bay
        mouseWorldPosition.z = transform.position.z;

        // Di chuyển máy bay
        if (useSmoothing)
        {
            // Di chuyển mượt
            transform.position = Vector3.Lerp(transform.position, mouseWorldPosition, smoothingSpeed * Time.deltaTime);
        }
        else
        {
            // Di chuyển trực tiếp
            transform.position = Vector3.MoveTowards(transform.position, mouseWorldPosition, moveSpeed * Time.deltaTime);
        }
    }

    // tính điểm khi ăn ngôi sao
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Star"))
        {
            // Tăng điểm
            //GameManager.Instance.AddScore(1);
            // Hủy ngôi sao
            Destroy(other.gameObject);
        }

        //if(other.gameObject.CompareTag("Asteroid"))
        //{
        //    Debug.Log("Player collided with an asteroid!");
        //    // Hủy tiểu hành tinh
        //    Destroy(other.gameObject);
        //}
    }

}
