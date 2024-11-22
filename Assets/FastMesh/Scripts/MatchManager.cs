using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    public ParticleSystem destroyEffect; // Yok olma efekti
    private List<GameObject> matchingObjects = new List<GameObject>();
    public float destroyDelay = 0.5f; // Yok olma gecikmesi
    public Color defaultColor = Color.white; // Plane'in varsayýlan rengi
    public Color mismatchColor = Color.red; // Eþleþme yoksa kullanýlacak renk
    public Color successColor = Color.green; // Baþarýlý eþleþme olduðunda kullanýlacak renk
    public float mismatchDuration = 1f; // Kýrmýzý rengin süresi
    public float successDuration = 1f; // Yeþil rengin süresi

    private Renderer planeRenderer; // Plane'in Renderer bileþeni

    void Start()
    {
        // Plane'in Renderer bileþenine eriþim
        planeRenderer = GetComponent<Renderer>();

        // Varsayýlan rengi ayarla
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
            return; // Ýki nesne yerleþtirilmeden kontrol yapýlmasýn

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
            StartCoroutine(IndicateSuccessAfterDelay()); // Eþleþme baþarý olduðunda yeþil renk
        }
        else
        {
            // Eþleþme olmadýðýnda kýrmýzýya dönmesi için gecikme
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
        // Eþleþme kontrolü yapýldýktan sonra renk deðiþimini yap
        yield return new WaitForSeconds(0.1f); // Eþleþme kontrolü sonrasý küçük gecikme

        // Plane'in rengini kýrmýzýya deðiþtir
        if (planeRenderer != null)
        {
            planeRenderer.material.color = mismatchColor;

            // Kýrmýzý renkten sonra eski rengine dönmesi için bekle
            yield return new WaitForSeconds(mismatchDuration);

            planeRenderer.material.color = defaultColor;
        }
    }

    IEnumerator IndicateSuccessAfterDelay()
    {
        // Eþleþme kontrolü sonrasýnda yeþil renge dönmesi
        yield return new WaitForSeconds(0.1f); // Eþleþme kontrolü sonrasý küçük gecikme

        if (planeRenderer != null)
        {
            planeRenderer.material.color = successColor;

            // Yeþil renkten sonra eski rengine dönmesi için bekle
            yield return new WaitForSeconds(successDuration);

            planeRenderer.material.color = defaultColor;
        }
    }
}
