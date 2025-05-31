using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject astroidPrefab;
    public float minInstantiateTime;
    public float maxInstantiateTime;
    public float destroyTime = 10f; // Time after which the asteroid will be destroyed

    private void Start()
    {
        InvokeRepeating("InstantiateAstroid", 1f, 1f); 
    }

    void InstantiateAstroid()
    {
        Vector3 randomPos = new Vector3(Random.Range(minInstantiateTime, maxInstantiateTime), 6f);
        GameObject astroid = Instantiate(astroidPrefab, randomPos, Quaternion.identity);
        Destroy(astroid, destroyTime);
    }
}
