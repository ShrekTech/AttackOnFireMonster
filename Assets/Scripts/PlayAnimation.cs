using UnityEngine;
using System.Collections;

public class PlayAnimation : MonoBehaviour {
    public string AnimationName;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        animator.Play(AnimationName);
    }

    void Update()
    {
        var currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!currentStateInfo.IsName(AnimationName)) {
            Object.Destroy(this.gameObject);
        }
    }
}
