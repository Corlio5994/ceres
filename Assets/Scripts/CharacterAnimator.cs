using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {
    [SerializeField] float animationSmoothTime = 0.1f;
    [SerializeField] Animator animator;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;

    void Update () {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat ("speedPercent", speedPercent, animationSmoothTime, Time.deltaTime);
    }
}