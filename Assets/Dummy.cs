using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{

    [SerializeField] Camera _camera;
    private void FootR()
    {
        AudioManager.Instance.PlaySFX("Walking");
    }
    private void FootL()
    {
        AudioManager.Instance.PlaySFX("Walking");
    }
    private void Hit()
    {

    }

    public void Burst()
    {
        Rigidbody rb = GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * 50000f);
        }
    }

    public void UnParentCam3()
    {
        Player player = GetComponentInParent<Player>();
        if (player == null || player._cameraCinematic == null)
        {
            return;
        }
        player._cameraCinematic.gameObject.SetActive(false);
        _camera.gameObject.SetActive(true);
    }
}
