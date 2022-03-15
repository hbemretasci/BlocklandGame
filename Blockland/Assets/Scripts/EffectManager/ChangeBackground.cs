using UnityEngine;

public class ChangeBackground : MonoBehaviour
{
    public Sprite[] firstBackgrounds;
    public Sprite[] secondBackgrounds;
    public Animator animator;
    public int firstIndex = 0;
    public int secondIndex = 0;
    public bool isFirst = true;

    public void Change()
    {
        if (isFirst) animator.SetTrigger("FadeOut");
        else animator.SetTrigger("FadeIn");
        isFirst = !isFirst;
    }

}
