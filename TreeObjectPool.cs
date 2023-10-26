using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesObjectPool : MonoBehaviour
{
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject flowerPrefab;
    public int treePoolSize = 20;
    public int rockPoolSize = 20;
    public int flowerPoolSize = 20;
    public float spawnInterval = 5f;
    public float MinDistanceBetweenTrees = 2.0f;
    public float Increase = 5f;
    public Transform[] spawnAreas; // Birden fazla spawn bölgesi
    private List<GameObject> treePool = new List<GameObject>();
    private List<GameObject> rockPool = new List<GameObject>();
    private List<GameObject> flowerPool = new List<GameObject>();
    private bool _isPositionActive = true;
    public GameObject tree;
    private void Awake()
    {
        

    }
    private void Start()
    {
        
        InitializePool(treePrefab, treePool, treePoolSize);
        InitializePool(rockPrefab, rockPool, rockPoolSize);
        InitializePool(flowerPrefab, flowerPool, flowerPoolSize);
        if(_isPositionActive)
        StartCoroutine(SpawnObjects());
    }
    private void Update()
    {
    }
    public void FindGroundObjects()
    {
       
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground");
        spawnAreas = new Transform[groundObjects.Length];

        for (int i = 0; i < groundObjects.Length; i++)
        {
            spawnAreas[i] = groundObjects[i].transform;
        }
    }

    private void InitializePool(GameObject prefab, List<GameObject> pool, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            FindGroundObjects();
            // Eðer kare alanýnda maksimum aðaç sayýsýna ulaþýlmamýþsa spawn et
            if (CountActiveObjects(treePool) < spawnAreas.Length)
            {
                SpawnObject(treePrefab, treePool);
            }
            if (CountActiveObjects(rockPool) < spawnAreas.Length)
            {
                SpawnObject(rockPrefab, rockPool);
            }
            if (CountActiveObjects(flowerPool) < spawnAreas.Length)
            {
                SpawnObject(flowerPrefab, flowerPool);
            }
        }
    }
    private void SpawnObject(GameObject prefab, List<GameObject> pool)
    {
        GameObject obj = GetObjectFromPool(pool);
        if (obj != null)
        {
            Vector3 randomPosition = GetRandomSpawnPosition();
            obj.transform.position = randomPosition;
            obj.SetActive(true);
        }
    }
    private GameObject GetObjectFromPool(List<GameObject> pool)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeSelf)
            {
                return obj;
            }
        }
        return null;
    }
    private int CountActiveObjects(List<GameObject> pool)
    {
        int count = 0;
        foreach (var obj in pool)
        {
            if (obj.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    private bool IsClearPosition(Vector3 position, List<GameObject> pool)
    {
        foreach (var obj in pool)
        {
            if (obj.activeSelf)
            {
                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < MinDistanceBetweenTrees)
                {
                    return false; // Eðer mesafe yeterince uzak deðilse, pozisyon kullanýlamaz.
                }
            }
        }

        Collider[] colliders = Physics.OverlapSphere(position, 0.1f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return false; // Eðer baþka bir obje ile çakýþma varsa ve bu obje "Ground" dýþýndaysa, pozisyon kullanýlamaz.
            }
        }

        return true; // Pozisyon kullanýlabilir.
    }
    private Vector3 GetRandomSpawnPosition()
    {
        Transform randomSpawnArea = spawnAreas[Random.Range(0, spawnAreas.Length)];
      
            Vector3 randomPosition = Vector3.zero;
            for (int i = 0; i < 100; i++) // Rastgele bir konum bulmak için 100 kez deneyin
            {
                // Rastgele bir pozisyon hesapla
                randomPosition = randomSpawnArea.position + new Vector3(
                    Random.Range(-(randomSpawnArea.localScale.x * Increase) / 2f, (randomSpawnArea.localScale.x * Increase) / 2f),
                     0.35f,
                    Random.Range(-(randomSpawnArea.localScale.z * Increase) / 2f, (randomSpawnArea.localScale.z * Increase) / 2f)
                );

                // Minimum mesafeyi saðlamak için kontrol ekleyin
                bool isClear = true;
                // Tüm havuzlarý kontrol et
                isClear &= IsClearPosition(randomPosition, treePool);
                isClear &= IsClearPosition(randomPosition, rockPool);
                isClear &= IsClearPosition(randomPosition, flowerPool);

                // Eðer bu konum temizse, bu konumu kullan
                if (isClear)
                {
                    _isPositionActive = true;
                    return randomPosition;
                }
            }

        
        // Eðer 100 denemede uygun bir konum bulunamazsa, (0,0,0) noktasýna döndürün.
        _isPositionActive = false;
        return Vector3.zero;
       
    }

        // Eðer 100 denemede uygun bir konum bulunamazsa, (0,0,0) noktasýna döndürün.
      
  }
