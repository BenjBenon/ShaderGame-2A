using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSlash : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    [SerializeField] ParticleSystem ps1;
    [SerializeField] Scriptable _data;
    Color initialColor;
    bool isSpecial;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject,2.0f);
        var mainModule = ps1.main;
        initialColor = mainModule.startColor.color;
        isSpecial = _data.isSpecialMode;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isSpecial = _data.isSpecialMode;
        if (isSpecial)
        {
            var mainModule = ps1.main;
            mainModule.startColor = Color.green;
        }
        else
        {
            var mainModule = ps1.main;
            mainModule.startColor = initialColor;

        }
        rb.velocity = transform.forward * 15f;
        ps1.transform.localScale = new Vector3(ps1.transform.localScale.x + 0.005f, ps1.transform.localScale.y, ps1.transform.localScale.z);

    }
}
