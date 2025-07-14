using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    AudioSource audioSource;
    [SerializeField] Scriptable _data;
    [SerializeField] Material _material;
    [SerializeField] UnityEvent _anim1;
    [SerializeField] UnityEvent _anim2;
    [SerializeField] UnityEvent _anim3;
    [SerializeField] UnityEvent _anim4;
    [SerializeField] UnityEvent _anim5;

    [SerializeField] ParticleSystem _ps1;
    [SerializeField] ParticleSystem _ps2;
    [SerializeField] ParticleSystem _ps3;
    [SerializeField] ParticleSystem _ps4;
    [SerializeField] ParticleSystem _ps5;


    [SerializeField] VisualEffect vfx1;
    [SerializeField] VisualEffect vfx2;
    [SerializeField] VisualEffect vfx3;
    [SerializeField] VisualEffect vfx4;
    [SerializeField] VisualEffect vfx5;

    [SerializeField] TextMeshPro text;
    [SerializeField] TextMeshPro text2;

    Material OGmaterial;
    Renderer _renderer;
    float timer;
    bool isHit;
    bool _actualMode;
    private Color initialColor;
    void Start()
    {
        timer = 0f;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //OGmaterial = GetComponentInChildren<Material>();
        _renderer = GetComponentInChildren<Renderer>();
        OGmaterial = _renderer.material;
        isHit = false;
        _actualMode = true;
        var mainModule = _ps1.main;
        initialColor = mainModule.startColor.color;
        //text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_actualMode != _data.isSpecialMode)
        {
            _actualMode = _data.isSpecialMode;
            if (!_actualMode)
            {
                var mainModule = _ps1.main;
                mainModule.startColor = initialColor;
                var mainModule2 = _ps2.main;
                mainModule2.startColor = initialColor;
                var mainModule3 = _ps3.main;
                mainModule3.startColor = initialColor;
                var mainModule4 = _ps4.main;
                mainModule4.startColor = initialColor;
                var mainModule5 = _ps5.main;
                mainModule5.startColor = initialColor;
            }
            else
            {
                var mainModule = _ps1.main;
                mainModule.startColor = Color.green;
                var mainModule2 = _ps2.main;
                mainModule2.startColor = Color.green;
                var mainModule3 = _ps3.main;
                mainModule3.startColor = Color.green;
                var mainModule4 = _ps4.main;
                mainModule4.startColor = Color.green;
                var mainModule5 = _ps5.main;
                mainModule5.startColor = Color.green;
            }
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Sword" && isHit == false)
    //    {
    //        isHit = true;
    //        animator.SetTrigger("Hit");
    //        audioSource.Play();
    //        _renderer.material = _material;
    //        StartCoroutine(CoRoutineDamage());
    //        //_renderer.material = OGmaterial;
    //    }
    //}

    IEnumerator CoRoutineDamage()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f)
            {
                break;
            }
            yield return null;
        }
        isHit = false;
        timer = 0f;
        _renderer.material = OGmaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ability")
        {
            Vector3 playerCenter = _data._player.gameObject.transform.position + _data._player.gameObject.transform.transform.up * 2.5f;
            Vector3 enemyCenter = transform.position + transform.up * 2.5f;
            Vector3 rayDirection = (enemyCenter - playerCenter).normalized;

            RaycastHit[] hits = Physics.RaycastAll(playerCenter, rayDirection, Mathf.Infinity);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject == other.gameObject)
                {
                    Vector3 randomOffset = Random.insideUnitSphere * 0.3f;
                    Vector3 randomizedContactPoint = hit.point + randomOffset;
                    
                    { ReceiveDamage(30, 9, randomizedContactPoint, _data._player.gameObject.transform.eulerAngles, _data._player.gameObject); }
                    break;
                }
            }
        }
    }

    public void ReceiveDamage(int damage,int attack,Vector3 pos,Vector3 euler,GameObject player)
    {
        if (isHit == false)
        {
            isHit = true;
            //animator.SetTrigger("Hit");
            audioSource.Play();
            _renderer.material = _material;
            GameObject damageText = Instantiate(text.gameObject);
            TextMeshPro textMesh = damageText.GetComponent<TextMeshPro>();
            textMesh.text = damage.ToString();
            textMesh.transform.localPosition = pos - player.transform.forward * 0.4f + player.transform.up * 0.5f;
            textMesh.transform.localEulerAngles = euler;
            StartCoroutine(CoRoutineDamage());
            switch (attack)
            {
                case 0:
                    {
                        _ps1.transform.position = pos - player.transform.forward*0.11f;
                        Vector3 directionToPlayer = (transform.position - _ps1.transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                        float additionalRotationZ = 45f;
                        Quaternion rotatedTarget = targetRotation * Quaternion.Euler(0, 0, additionalRotationZ);
                        _ps1.transform.rotation = rotatedTarget;
                        vfx1.transform.position = pos;
                        vfx1.transform.eulerAngles = new Vector3(vfx1.transform.eulerAngles.x, (euler.y + 180) % 360, vfx1.transform.eulerAngles.z);
                        _anim1.Invoke();
                        break;
                    }
                case 1:
                    {
                        _ps2.transform.position = pos - player.transform.forward * 0.11f;
                        Vector3 directionToPlayer = (transform.position - _ps2.transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                        float additionalRotationZ = -45f;
                        Quaternion rotatedTarget = targetRotation * Quaternion.Euler(0, 0, additionalRotationZ);
                        _ps2.transform.rotation = rotatedTarget;
                        vfx2.transform.position = pos;
                        vfx2.transform.eulerAngles = new Vector3(vfx2.transform.eulerAngles.x, (euler.y + 180) % 360, vfx2.transform.eulerAngles.z);
                        _anim2.Invoke();
                        break;
                    }
                case 2:
                    {
                        _ps3.transform.position = pos - player.transform.forward * 0.05f;
                        Vector3 directionToPlayer = (transform.position - _ps3.transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                        float additionalRotationZ = 35f;
                        Quaternion rotatedTarget = targetRotation * Quaternion.Euler(0, 0, additionalRotationZ);
                        _ps3.transform.rotation = rotatedTarget;
                        vfx3.transform.position = pos;
                        vfx3.transform.eulerAngles = new Vector3(vfx3.transform.eulerAngles.x, (euler.y + 180) % 360, vfx3.transform.eulerAngles.z);
                        _anim3.Invoke();
                        break;
                    }
                case 3:
                    {
                        _ps4.transform.position = pos - player.transform.forward * 0.01f;
                        Vector3 directionToPlayer = (transform.position - _ps4.transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                        float additionalRotationZ = 80f;
                        Quaternion rotatedTarget = targetRotation * Quaternion.Euler(0, 0, additionalRotationZ);
                        _ps4.transform.rotation = rotatedTarget;
                        vfx4.transform.position = pos;
                        vfx4.transform.eulerAngles = new Vector3(vfx4.transform.eulerAngles.x, (euler.y + 180) % 360, vfx4.transform.eulerAngles.z);
                        _anim4.Invoke();
                        break;
                    }
                case 4:
                    {
                        _ps5.transform.position = pos - player.transform.forward * 0.11f;
                        Vector3 directionToPlayer = (transform.position - _ps5.transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                        float additionalRotationZ = 0;
                        Quaternion rotatedTarget = targetRotation * Quaternion.Euler(0, 0, additionalRotationZ);
                        _ps5.transform.rotation = rotatedTarget;
                        vfx5.transform.position = pos;
                        vfx5.transform.eulerAngles = new Vector3(vfx5.transform.eulerAngles.x, (euler.y + 180) % 360, vfx5.transform.eulerAngles.z);
                        _anim5.Invoke();
                        break;
                    }
                case 9:
                    {
                        _ps2.transform.position = pos - player.transform.forward * 0.11f;
                        Vector3 directionToPlayer = (transform.position - _ps2.transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                        float additionalRotationZ = -45f;
                        Quaternion rotatedTarget = targetRotation * Quaternion.Euler(0, 0, additionalRotationZ);
                        _ps2.transform.rotation = rotatedTarget;
                        vfx2.transform.position = pos;
                        vfx2.transform.eulerAngles = new Vector3(vfx2.transform.eulerAngles.x, (euler.y + 180) % 360, vfx2.transform.eulerAngles.z);
                        _anim2.Invoke();
                        break;
                    }
                case 10:
                    {
                        _ps1.transform.position = pos + Vector3.up * 10f;
                        Vector3 directionToPlayer = (transform.position - player.GetComponent<Player>()._cameraCinematic2.transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                        float additionalRotationZ = 0;
                        Quaternion rotatedTarget = targetRotation * Quaternion.Euler(0, 0, additionalRotationZ);
                        _ps1.transform.rotation = rotatedTarget;
                        vfx1.transform.position = pos;
                        vfx1.transform.eulerAngles = new Vector3(vfx1.transform.eulerAngles.x, (euler.y + 180) % 360, vfx1.transform.eulerAngles.z);
                        GameObject damageText2 = Instantiate(text2.gameObject);
                        TextMeshPro textMesh2 = damageText2.GetComponent<TextMeshPro>();
                        textMesh2.text = damage.ToString();
                        textMesh2.transform.localPosition = pos - player.transform.forward * 0.4f + player.transform.up * 0.5f;
                        textMesh2.transform.localEulerAngles = euler;
                        _anim1.Invoke();
                        break;
                    }
            }
        }
    }
}
