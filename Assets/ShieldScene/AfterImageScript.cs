using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageScript : MonoBehaviour
{
    public Material outlineMaterial;
    public float fadeDuration = 1.5f;
    public float spawnInterval = 0.1f;

    private bool canSpawn = true;

    public void startAfterImages()
    {
        Renderer baseRenderer = GetComponent<Renderer>();
        if (baseRenderer != null)
        {
            foreach (Material mat in baseRenderer.materials)
            {
                if (mat.HasProperty("_isActive"))
                {
                    mat.SetFloat("_isActive", 0f);
                }
            }
        }

        if(canSpawn)
        {
            StartCoroutine(SpawnAfterImages());
        }
    }

    public void stopAfterImages()
    {
        StopCoroutine(SpawnAfterImages());
    }

    IEnumerator SpawnAfterImages()
    {
        canSpawn = false;

        GameObject afterImage = Instantiate(gameObject, transform.position, transform.rotation);
        // Supp this script
        Destroy(afterImage.GetComponent<AfterImageScript>());

        // The problem is the SkinnedMeshRender so we have to change it
        SkinnedMeshRenderer skinnedRenderer = afterImage.GetComponent<SkinnedMeshRenderer>();
        if (skinnedRenderer != null)
        {
            Mesh bakedMesh = new Mesh();
            skinnedRenderer.BakeMesh(bakedMesh);

            MeshFilter meshFilter = afterImage.AddComponent<MeshFilter>();
            meshFilter.mesh = bakedMesh;

            // Instead of creating a new material, use the original one with modifications
            MeshRenderer meshRenderer = afterImage.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(skinnedRenderer.materials[skinnedRenderer.materials.Length - 1]);

            foreach (Material mat in meshRenderer.materials)
            {
                if (mat.HasProperty("_isActive"))
                {
                    mat.SetFloat("_isActive", 1f);
                }
            }

            Destroy(afterImage.GetComponent<SkinnedMeshRenderer>());
        }

        if (afterImage.GetComponent<Rigidbody>() != null)
        {
            Destroy(afterImage.GetComponent<Rigidbody>());
        }

        MonoBehaviour[] scripts = afterImage.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            Destroy(script);
        }

        afterImage.isStatic = true;
        afterImage.transform.parent = null;

        afterImage.AddComponent<AfterImageFade>().StartFade(fadeDuration);

        yield return new WaitForSeconds(spawnInterval);
        canSpawn = true;
    }
}

// Script for the diparition
public class AfterImageFade : MonoBehaviour
{
    private float fadeDuration;
    private Material material;
    private float timeElapsed = 0f;

    public void StartFade(float duration)
    {
        fadeDuration = duration;
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }
    }

    void Update()
    {
        if (material != null)
        {
            timeElapsed += Time.deltaTime;
            float isActiveValue = Mathf.Lerp(1, 0, timeElapsed/fadeDuration);

            material.SetFloat("_isActive",isActiveValue);
            Debug.Log(isActiveValue);

            if (material.GetFloat("_isActive") <= 0)
            {
                Destroy(gameObject); 
            }
        }
    }
}