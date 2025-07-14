using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] GameObject player;
    Player playerScript;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector3 playerCenter = player.transform.position + player.transform.up * 2.5f; 
                Vector3 enemyCenter = other.transform.position + other.transform.up * 2.5f;  
                Vector3 rayDirection = (enemyCenter - playerCenter).normalized;  

                RaycastHit[] hits = Physics.RaycastAll(playerCenter, rayDirection, Mathf.Infinity);

                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.gameObject == other.gameObject)
                    {
                        Vector3 randomOffset = Random.insideUnitSphere * 0.3f;
                        Vector3 randomizedContactPoint = hit.point + randomOffset;
                        if(playerScript.isCinematic)
                        {
                            enemy.ReceiveDamage(0, 10, randomizedContactPoint, player.transform.eulerAngles, player);
                        }
                        else
                        { enemy.ReceiveDamage(10, playerScript._trueCount, randomizedContactPoint, player.transform.eulerAngles, player); }
                        break; 
                    }
                }
            }
        }
    }


}
