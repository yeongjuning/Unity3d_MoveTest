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

        None,
    }
    
    public static GameManager Manager { private set; get; }
    public EButtonType eButtonType { set; get; }

    [SerializeField] Transform[] ReturnPositions;
    [SerializeField] Button[] buttons_;

    bool[] isClicked = new bool[(int)EButtonType.None];
    
    private void Awake()
    {
        Manager = this;
        eButtonType = EButtonType.None;
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

        Debug.Log("OnClickButton : " + eButtonType.ToString());
    }

    public Transform GetReturnRandomPosition()
    {
        int ranIdx = Random.Range(0, ReturnPositions.Length);
        return ReturnPositions[ranIdx];
    }

    public bool IsClicked(int idx)
    {
        return isClicked[idx];
    }
}
