using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float smoothingSpeed = 5f;

    [Header("Input Settings")]
    [SerializeField] private bool useMouseMovement = true;
    [SerializeField] private bool useKeyboardMovement = true;
    [SerializeField] private float keyboardMoveSpeed = 7f;

    [Header("Boundaries")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4.5f;
    [SerializeField] private float maxY = 4.5f;

    private Camera mainCamera;
    private Vector3 targetPosition;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found! Make sure your camera has the 'MainCamera' tag.");
        }

        // Đặt vị trí mục tiêu ban đầu là vị trí hiện tại của phi thuyền
        targetPosition = transform.position;
    }

    private void Update()
    {
        // Di chuyển theo chuột nếu được bật
        if (useMouseMovement)
        {
            UpdateMouseTarget();
        }

        // Di chuyển theo phím nếu được bật
        if (useKeyboardMovement)
        {
            MoveWithKeyboard();
        }

        // Di chuyển phi thuyền đến vị trí mục tiêu
        MoveTowardsTarget();

        // Giới hạn vị trí của phi thuyền trong màn hình
        ClampPosition();
    }

    private void UpdateMouseTarget()
    {
        // Lấy vị trí chuột trong không gian thế giới
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            -mainCamera.transform.position.z));

        // Giữ nguyên vị trí Z của máy bay
        mouseWorldPosition.z = transform.position.z;

        // Cập nhật vị trí mục tiêu
        targetPosition = mouseWorldPosition;
    }

    private void MoveWithKeyboard()
    {
        // Lấy input từ phím WASD hoặc phím mũi tên
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            // Tạo vector di chuyển
            Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0).normalized;

            // Di chuyển phi thuyền theo hướng đã tính
            transform.position += moveDirection * keyboardMoveSpeed * Time.deltaTime;

            // Cập nhật vị trí mục tiêu theo vị trí hiện tại nếu đang di chuyển bằng phím
            targetPosition = transform.position;
        }
    }

    private void MoveTowardsTarget()
    {
        // Chỉ di chuyển đến mục tiêu nếu không đang điều khiển bằng phím
        if (!useKeyboardMovement || (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0))
        {
            if (useSmoothing)
            {
                // Di chuyển mượt
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothingSpeed * Time.deltaTime);
            }
            else
            {
                // Di chuyển trực tiếp
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void ClampPosition()
    {
        // Giới hạn vị trí trong phạm vi màn hình
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        // Cập nhật lại vị trí mục tiêu nếu nằm ngoài giới hạn
        targetPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minX, maxX),
            Mathf.Clamp(targetPosition.y, minY, maxY),
            targetPosition.z);
    }
}
