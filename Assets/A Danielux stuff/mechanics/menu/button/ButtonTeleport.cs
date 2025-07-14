using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTeleport : MonoBehaviour
{

    public GameObject fade;
    [Tooltip("the scene you want the player to go")]
    public int SceneTP;
  

    public void Click()
    {
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        fade.GetComponent<Animator>().SetBool("activate", true);

        yield return new WaitForSeconds(fade.GetComponent<Animator>().runtimeAnimatorController.animationClips.Length);
        SceneManager.LoadScene(SceneTP);

    }
}
