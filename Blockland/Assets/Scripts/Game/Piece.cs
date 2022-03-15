using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public Image storeImage;

    Material material;
    float fade = 0f;
    bool isVisible = false;

    private void Start()
    {
        material = storeImage.material;
        material.SetFloat("_DissolveAmount", 0);
    }

    private void Update()
    {
        if (!isVisible)
        {
            fade += Time.deltaTime;
            if (fade >= 1f)
            {
                fade = 1f;
                isVisible = true;
            }
            material.SetFloat("_DissolveAmount", fade);
        }
    }

    public void ChangeStoreImage(Sprite newLevelSprite)
    {
        storeImage.sprite = newLevelSprite;
    }

}