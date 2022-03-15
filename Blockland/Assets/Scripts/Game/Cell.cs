using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Image emptyImage;
    public Image busyImage;
    public Image activeImage;
    public Image destroyImage;

    public int cellRow;
    public int cellCol;

    public bool isBonusCell = false;

    public void SetCellEmpty()
    {
        tag = "Cell";
        isBonusCell = false;
        Invoke("ViewCellDestroyToEmpty", .5f);
    }

    private void ViewCellDestroyToEmpty()
    {
        destroyImage.gameObject.SetActive(false);
    }

    public void SetCellActive()
    {
        activeImage.sprite = busyImage.sprite;
        ViewCellActive();
        tag = "Active";
    }

    public void ViewCellActive()
    {
        destroyImage.gameObject.SetActive(false);
        busyImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
    }

    public void ViewCellEmpty()
    {
        activeImage.gameObject.SetActive(false);
        busyImage.gameObject.SetActive(false);
        destroyImage.gameObject.SetActive(false);
    }

    public void ViewCellBusy()
    {
        destroyImage.gameObject.SetActive(false);
        busyImage.gameObject.SetActive(true);
    }

    public void ViewCellDestroy()
    {
        activeImage.gameObject.SetActive(false);
        busyImage.gameObject.SetActive(false);
        destroyImage.gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CompareTag("Cell")) tag = "Busy";
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (CompareTag("Cell")) tag = "Busy";
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CompareTag("Busy")) tag = "Cell";            
    }
}