using UnityEngine;

public class DestroyCell : MonoBehaviour
{
    public Material destroyMaterial;

    private float fade;
    private bool isDestroyEffectStarting = false;

    private void Update()
    {
        if (isDestroyEffectStarting)
        {
            fade -= Time.deltaTime * .5f;
            destroyMaterial.SetFloat("_DissolveAmount", fade);
            if (fade <= 0f)
            {
                isDestroyEffectStarting = false;
                destroyMaterial.SetFloat("_DissolveAmount", .6f);
            }
        }  
    }

    public void StartDestroyLines()
    {
        fade = destroyMaterial.GetFloat("_DissolveAmount");
        isDestroyEffectStarting = true;
    }
}
