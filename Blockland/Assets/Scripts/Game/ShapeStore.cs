using System.Collections.Generic;
using UnityEngine;

public class ShapeStore : MonoBehaviour
{
    [SerializeField] List<ShapeData> shapeDataList1;
    [SerializeField] List<ShapeData> shapeDataList2;
    [SerializeField] List<ShapeData> shapeDataList3;
    [SerializeField] List<Shape> shapeList;

    private GameManager gm;

    private void OnEnable()
    {
        GameEvents.GetNewShapeSet += GetNewShapeSet;
        GameEvents.EmptyShapeStore += EmptyShapeStore;
    }

    private void OnDisable()
    {
        GameEvents.GetNewShapeSet -= GetNewShapeSet;
        GameEvents.EmptyShapeStore -= EmptyShapeStore;
    }

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameEvents.SetLevel();
        FillShapeStore();
    }

    private void FillShapeStore()
    {
        List<int> randomNumbers1 = new List<int>();
        List<int> randomNumbers2 = new List<int>();
        List<int> randomNumbers3 = new List<int>();
        bool result;
        int shapeIndex;
        int counter = -1;

        for (int random = 0; random < 3; random++)
        {
            int poolIndex = UnityEngine.Random.Range(0, 16);
            if (poolIndex <= 8)
            {
                result = false;
                do
                {
                    shapeIndex = UnityEngine.Random.Range(0, shapeDataList1.Count);
                    if (!randomNumbers1.Contains(shapeIndex)) result = true;
                } while (!result);
                randomNumbers1.Add(shapeIndex);
            }
            else if (poolIndex <= 13)
            {
                result = false;
                do
                {
                    shapeIndex = UnityEngine.Random.Range(0, shapeDataList2.Count);
                    if (!randomNumbers2.Contains(shapeIndex)) result = true;
                } while (!result);
                randomNumbers2.Add(shapeIndex);
            }
            else
            {
                result = false;
                do
                {
                    shapeIndex = UnityEngine.Random.Range(0, shapeDataList3.Count);
                    if (!randomNumbers3.Contains(shapeIndex)) result = true;
                } while (!result);
                randomNumbers3.Add(shapeIndex);
            }
        }

        for(int i = 0; i < randomNumbers1.Count; i++)
        {
            counter++;
            shapeList[counter].tag = "Store";
            shapeList[counter].CreateShape(shapeDataList1[randomNumbers1[i]]);
        }

        for (int i = 0; i < randomNumbers2.Count; i++)
        {
            counter++;
            shapeList[counter].tag = "Store";
            shapeList[counter].CreateShape(shapeDataList2[randomNumbers2[i]]);
        }

        for (int i = 0; i < randomNumbers3.Count; i++)
        {
            counter++;
            shapeList[counter].tag = "Store";
            shapeList[counter].CreateShape(shapeDataList3[randomNumbers3[i]]);
        }

    }

    private void GetNewShapeSet()
    {
        bool requiredNewSet = true;

        for (int i = 0; i < shapeList.Count; i++)
            if (shapeList[i].CompareTag("Store")) requiredNewSet = false;

        if (requiredNewSet)
        {
            GameEvents.SetLevel();
            FillShapeStore();
        }
    }

    private void EmptyShapeStore()
    {
        GameObject[] shapesInStore = GameObject.FindGameObjectsWithTag("Store");
        Shape shapeScript;

        for (int i = 0; i < shapesInStore.Length; i++)
        {
            shapesInStore[i].tag = "Over";
            shapeScript = shapesInStore[i].GetComponent<Shape>();
            shapeScript.DeactiveShape();
        }
    }

}