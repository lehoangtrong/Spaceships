using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform firePoint;
    private GameObject currentLaser;

    void Update()
    {
        // Bắt đầu bắn laser khi nhấn giữ chuột trái hoặc phím Space
        if ((Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) && currentLaser == null)
        {
            currentLaser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
            currentLaser.transform.parent = firePoint; // gắn laser vào tàu để nó follow theo
            currentLaser.transform.rotation = Quaternion.identity;
        }

        // Dừng bắn laser khi thả chuột/phím
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
        {
            if (currentLaser != null)
            {
                Destroy(currentLaser);
                currentLaser = null;
            }
        }
    }
}
