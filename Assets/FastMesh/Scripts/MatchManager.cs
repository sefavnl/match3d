using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    public ParticleSystem destroyEffect; // Yok olma efekti
    private List<GameObject> matchingObjects = new List<GameObject>();
    public float destroyDelay = 0.5f; // Yok olma gecikmesi
    public Color defaultColor = Color.white; // Plane'in varsay�lan rengi
    public Color mismatchColor = Color.red; // E�le�me yoksa kullan�lacak renk
    public Color successColor = Color.green; // Ba�ar�l� e�le�me oldu�unda kullan�lacak renk
    public float mismatchDuration = 1f; // K�rm�z� rengin s�resi
    public float successDuration = 1f; // Ye�il rengin s�resi

    private Renderer planeRenderer; // Plane'in Renderer bile�eni

    void Start()
    {
        // Plane'in Renderer bile�enine eri�im
        planeRenderer = GetComponent<Renderer>();

        // Varsay�lan rengi ayarla
        if (planeRenderer != null)
        {
            planeRenderer.material.color = defaultColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Matchable"))
        {
            matchingObjects.Add(other.gameObject);
            CheckForMatchingObjects();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (matchingObjects.Contains(other.gameObject))
        {
            matchingObjects.Remove(other.gameObject);
        }
    }

    void CheckForMatchingObjects()
    {
        if (matchingObjects.Count < 2)
            return; // �ki nesne yerle�tirilmeden kontrol yap�lmas�n

        int matchCount = 0;
        string typeToMatch = matchingObjects[matchingObjects.Count - 1].GetComponent<ItemType>().type;

        foreach (GameObject obj in matchingObjects)
        {
            if (obj != null && obj.GetComponent<ItemType>().type == typeToMatch)
            {
                matchCount++;
            }
        }

        if (matchCount >= 2)
        {
            StartCoroutine(DestroyMatchingObjects(typeToMatch));
            StartCoroutine(IndicateSuccessAfterDelay()); // E�le�me ba�ar� oldu�unda ye�il renk
        }
        else
        {
            // E�le�me olmad���nda k�rm�z�ya d�nmesi i�in gecikme
            StartCoroutine(IndicateMismatchAfterDelay());
        }
    }

    IEnumerator DestroyMatchingObjects(string typeToMatch)
    {
        yield return new WaitForSeconds(destroyDelay);

        for (int i = matchingObjects.Count - 1; i >= 0; i--)
        {
            if (matchingObjects[i] != null && matchingObjects[i].GetComponent<ItemType>().type == typeToMatch)
            {
                ParticleSystem effect = Instantiate(destroyEffect, matchingObjects[i].transform.position, Quaternion.identity);
                effect.Play();

                Destroy(matchingObjects[i]);
                matchingObjects.RemoveAt(i);

                Destroy(effect.gameObject, effect.main.duration);
            }
        }
    }

    IEnumerator IndicateMismatchAfterDelay()
    {
        // E�le�me kontrol� yap�ld�ktan sonra renk de�i�imini yap
        yield return new WaitForSeconds(0.1f); // E�le�me kontrol� sonras� k���k gecikme

        // Plane'in rengini k�rm�z�ya de�i�tir
        if (planeRenderer != null)
        {
            planeRenderer.material.color = mismatchColor;

            // K�rm�z� renkten sonra eski rengine d�nmesi i�in bekle
            yield return new WaitForSeconds(mismatchDuration);

            planeRenderer.material.color = defaultColor;
        }
    }

    IEnumerator IndicateSuccessAfterDelay()
    {
        // E�le�me kontrol� sonras�nda ye�il renge d�nmesi
        yield return new WaitForSeconds(0.1f); // E�le�me kontrol� sonras� k���k gecikme

        if (planeRenderer != null)
        {
            planeRenderer.material.color = successColor;

            // Ye�il renkten sonra eski rengine d�nmesi i�in bekle
            yield return new WaitForSeconds(successDuration);

            planeRenderer.material.color = defaultColor;
        }
    }
}
