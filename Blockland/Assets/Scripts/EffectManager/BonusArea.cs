using TMPro;
using UnityEngine;

public class BonusArea : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leftValueText;
    [SerializeField] private Animator bonusAnimator;

    public RectTransform bonusRect;
    public bool state = false;

    public void ShowBonusArea()
    {
        bonusRect.localScale = new Vector3(0, 1, 1);
        bonusRect.gameObject.SetActive(true);
        bonusAnimator.SetTrigger("DoAppear");
        state = true;
    }

    public void HideBonusArea()
    {
        bonusRect.localScale = new Vector3(1, 1, 1);
        bonusAnimator.SetTrigger("DoDisappear");
        state = false;
    }

    public void ShowBonusValue(int value)
    {
        leftValueText.text = value.ToString();
    }
    
}
