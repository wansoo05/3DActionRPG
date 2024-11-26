using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float force = 1000.0f;

    private new Rigidbody rigidbody;
    private new Collider collider;

    public event Action<Collider, Collider, Vector3, DoActionData, GameObject> OnProjectileHit;

    [SerializeField]
    private DoActionData doActionData;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        rigidbody.velocity = Vector3.zero;
    }

    private void OnDisable()
    {
        OnProjectileHit = null;
    }

    private void Start()
    {

    }

    public void Shoot(float destroyTime, Vector3 direction)
    {
        StartCoroutine(DestroyProjectile(destroyTime));
        rigidbody.AddForce(direction * force);
    }

    /// <summary>
    /// Projectile�� ���� �ڿ� �������� �ϱ� ���� �ڷ�ƾ �Լ�.
    /// </summary>
    /// <param name="time"> ���� �ð� </param>
    /// <returns></returns>
    private IEnumerator DestroyProjectile(float time)
    {
        yield return new WaitForSeconds(time);
        Factory.Instance.ReturnProjectileDelay(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnProjectileHit?.Invoke(collider, other, transform.position, doActionData, gameObject);
    }
}