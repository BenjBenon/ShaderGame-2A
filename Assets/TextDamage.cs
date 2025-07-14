using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextDamage : MonoBehaviour
{
    public TextMeshPro textMesh;
    public float lifeTime = 1.5f;
    public float floatSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        StartCoroutine(FadeInAndOut());
    }
    void Update()
    {
        Vector3 lookDirection = Camera.main.transform.position - textMesh.transform.position;
        lookDirection.y = 0; 
        lookDirection = Quaternion.Euler(0, 180, 0) * lookDirection;
        textMesh.transform.rotation = Quaternion.LookRotation(lookDirection);
        textMesh.transform.position += Vector3.up * 0.0001f;
    }
    private IEnumerator FadeInAndOut()
    {
        yield return Fade(0, 1);

        yield return new WaitForSeconds(0.5f);

        yield return Fade(1, 0);

        Destroy(gameObject);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color color = textMesh.color;

        while (timer < 0.2f)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / 0.2f);
            textMesh.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }
}
