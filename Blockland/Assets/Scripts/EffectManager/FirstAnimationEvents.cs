using UnityEngine;
using UnityEngine.UI;

public class FirstAnimationEvents : MonoBehaviour
{
    private ChangeBackground changeBackgroundScript;
    [SerializeField] Image secondImage;

    private void Start()
    {
        changeBackgroundScript = GameObject.Find("Background").GetComponent<ChangeBackground>();
    }

    private void OnFadeOutComplete()
    {
        changeBackgroundScript.firstIndex++;
        if (changeBackgroundScript.firstIndex > 3) changeBackgroundScript.firstIndex = 0;
        gameObject.GetComponent<Image>().sprite = changeBackgroundScript.firstBackgrounds[changeBackgroundScript.firstIndex];
    }

    private void OnFadeInComplete()
    {
        changeBackgroundScript.secondIndex++;
        if (changeBackgroundScript.secondIndex > 3) changeBackgroundScript.secondIndex = 0;
        secondImage.sprite = changeBackgroundScript.secondBackgrounds[changeBackgroundScript.secondIndex];
    }

}
