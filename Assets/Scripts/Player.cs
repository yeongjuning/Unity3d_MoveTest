using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager gameManager_;
    [SerializeField] GameObject goTarget_;

    Enemy enemy;
    Transform targetTransform;
    CapsuleCollider targetCollider;

    float moveSpeed_ = 10f;
    float stayTime = 2f;
    float timer = 0f;

    private void Awake()
    {
        enemy = goTarget_.GetComponent<Enemy>();
        targetTransform = goTarget_.transform.GetChild(1).transform;
        targetCollider = goTarget_.GetComponent<CapsuleCollider>();
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
        //Debug.Log("PlayerMoveToWards");
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, moveSpeed_ * Time.deltaTime);
        if (enemy.IsTriggerEnter)
        {
            Debug.Log("IsTriggerEnter");
            timer += Time.deltaTime;
            if (timer >= stayTime)
            {
                Debug.Log("timer : " + timer);
                
                Transform returnTransform = gameManager_.GetReturnRandomPosition();
                transform.position = Vector3.MoveTowards(transform.position, returnTransform.position, moveSpeed_ * Time.deltaTime);

                gameManager_.eButtonType = GameManager.EButtonType.None;
                timer = 0f;
            }
        }
    }

    void PlayerRotateToWards()
    {
        Debug.Log("PlayerRotateToWards");
    }
}
