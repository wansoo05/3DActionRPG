using System;
using UnityEngine;

public class Fist_Trigger : MonoBehaviour
{
    public event Action<Collider> OnTrigger;

    public void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(other);
    }
}