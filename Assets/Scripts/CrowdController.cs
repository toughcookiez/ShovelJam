using UnityEngine;

public class CrowdController : MonoBehaviour
{
    [SerializeField] private Animator[] BooAnimators;
    [SerializeField] protected Animator[] CheerAnimators;



    public void Cheer()
    {
        foreach (var animator in CheerAnimators)
        {
            animator.SetTrigger("Cheer");
        }
    }

    public void Boo()
    {
        foreach (var animator in BooAnimators)
        {
            animator.SetTrigger("Boo");
        }
    }
}
