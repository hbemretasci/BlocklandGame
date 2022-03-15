using UnityEngine;

public class BonusEvents : MonoBehaviour
{
    public void DisappearComplete()
    {
        gameObject.SetActive(false);
    }

}
