using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Spawn edilecek nesneler
    public int objectCount = 10; // Toplam oluşturulacak nesne sayısı
    public Vector3 spawnArea = new Vector3(5, 0, 5); // Spawn alanının boyutları
    public float minSpawnHeight = 5f; // Minimum spawn yüksekliği
    public float maxSpawnHeight = 10f; // Maksimum spawn yüksekliği

    void Start()
    {
        // Yerçekimi kontrolü
        Physics.gravity = new Vector3(0, -9.81f, 0); // Varsayılan yerçekimi

        for (int i = 0; i < objectCount; i++)
        {
            // Rastgele nesne oluştur
            GameObject obj = Instantiate(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)]);

            // Rastgele pozisyon belirle
            obj.transform.position = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                Random.Range(minSpawnHeight, maxSpawnHeight),
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
            );

            // Rigidbody bileşenini kontrol et ve ekle
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = obj.AddComponent<Rigidbody>();
            }

            // Rigidbody ayarlarını uygula
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.mass = Random.Range(0.5f, 2f); // Rastgele kütle
            rb.linearDamping = 0.1f; // Hava sürtünmesi
            rb.angularDamping = 0.05f; // Dönme sürtünmesi
        }
    }
}
