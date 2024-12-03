using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealCircle : MonoBehaviour
{
    [SerializeField]
    private float amount;

    [SerializeField]
    private GameObject auraPrefab;

    private List<GameObject> auras;

    private void Awake()
    {
        auras = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == false)
            return;

        //회복 아우라 생성
        if (auraPrefab == null) return;


        Transform t = other.transform.FindChildByName("Aura");
        if (t == null) return;

        Transform aura = other.transform.FindChildByName("Healing");
        if (aura != null)
            return;

        GameObject obj = Instantiate(auraPrefab, t);
        obj.name = "Healing";
        auras.Add(obj);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == false)
            return;

        //회복 매커니즘
        HealthPointComponent health = other.transform.GetComponent<HealthPointComponent>();
        if (health == null) return;
        health.Heal(amount * Time.deltaTime);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == false)
            return;

        Transform t = other.transform.FindChildByName("Healing");
        if (t == null)
            return;

        Destroy(t.gameObject, 0.5f);
    }

    private void OnDestroy()
    {
        foreach(GameObject obj in  auras)
        {
            Destroy(obj);
        }
    }
}