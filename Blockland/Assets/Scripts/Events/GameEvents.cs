using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action UpdateTable;

    public static Action GameCycleProgress;

    public static Action MoveShapeToStore;

    public static Action GetNewShapeSet;

    public static Action EmptyShapeStore;

    public static Action GameFinish;

    public static Action SetLevel;

}