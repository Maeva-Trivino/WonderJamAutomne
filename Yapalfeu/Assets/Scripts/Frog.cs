using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        Invoke("Fire", Random.Range(3,20));
    }

    void Fire()
    {
        animator.Play("Show");
        Invoke("Fire", Random.Range(3, 20));
    }
}
