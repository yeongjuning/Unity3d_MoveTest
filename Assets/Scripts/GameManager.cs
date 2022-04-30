using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum EButtonType
    {
        MoveToWards,
        RotateWards,

        Max,
    }
    
    public static GameManager Manager { private set; get; }
    public EButtonType eButtonType { private get; set; }

    [SerializeField] Button[] buttons_;
    
    bool[] isClicked = new bool[(int)EButtonType.Max];
    
    private void Awake()
    {
        Manager = this;
    }
    
    public void OnClickButton(int idx)
    {
        eButtonType = (EButtonType)idx;
        switch ((EButtonType)idx)
        {
            case EButtonType.MoveToWards:
            case EButtonType.RotateWards:
                isClicked[idx] = true;
                break;
        }
    }

    public bool IsClicked(int idx)
    {
        for (int i = 0; i < (int)EButtonType.Max; i++)
        {
            isClicked[i] = false;
        }

        return isClicked[idx] = true;
    }
}
