using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundNPCAnimator : MonoBehaviour
{
    [SerializeField] private bool isCheckWatch;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (isCheckWatch)
        {
            animator.SetFloat("Speed_f", 0f);
            StartCoroutine(AlternateCheckWatchHandOnHips());
        }
    }

    private IEnumerator AlternateCheckWatchHandOnHips()
    {
        while (true)
        {
            // Idle_CheckWatch
            animator.SetInteger("Animation_int", 3);
            yield return new WaitForSeconds(1.3f);
            // Idle_HandOnHips
            animator.SetInteger("Animation_int", 2);
            yield return new WaitForSeconds(5f);
        }
    }
}
