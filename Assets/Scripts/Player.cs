using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager gameManager_;
    [SerializeField] GameObject goTarget_;

    Enemy enemy_;
    Transform targetTransform_;
    Transform returnTransform_;
    CapsuleCollider targetCollider_;
    Vector3 dest_ = Vector3.zero;

    float moveSpeed_ = 10f;
    float stayTime = 2f;
    float timer = 0f;

    bool isGoing = false;
    bool isArrive = false;

    private void Awake()
    {
        enemy_ = goTarget_.GetComponent<Enemy>();
        targetTransform_ = goTarget_.transform.GetChild(1).transform;
        targetCollider_ = goTarget_.GetComponent<CapsuleCollider>();
    }

    void Start()
    {
    }
    
    void Update()
    {
        switch (gameManager_.eButtonType)
        {
            case GameManager.EButtonType.MoveToWards:
                PlayerMoveToWards();
                break;
            case GameManager.EButtonType.RotateWards:
                PlayerRotateToWards();
                break;
            default:
                break;
        }
    }
    
    void PlayerMoveToWards()
    {   
        if (enemy_.IsTriggerEnter)
        {
            timer += Time.deltaTime;
            if (timer >= stayTime)
            {
                
                if (false == isGoing)
                {
                    returnTransform_ = gameManager_.GetReturnRandomPosition();
                    dest_ = new Vector3(returnTransform_.position.x, transform.position.y, returnTransform_.position.z);
                    isGoing = true;
                }
                
                transform.position = Vector3.MoveTowards(transform.position, dest_, moveSpeed_ * Time.deltaTime);

                isArrive = transform.position == dest_;
                if (isArrive)
                {
                    timer = 0f;
                    gameManager_.eButtonType = GameManager.EButtonType.None;
                    enemy_.IsTriggerEnter = false;

                    isGoing = false;
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTransform_.position, moveSpeed_ * Time.deltaTime);
        }
    }
    
    void PlayerRotateToWards()
    {
        Debug.Log("PlayerRotateToWards");
    }
}
