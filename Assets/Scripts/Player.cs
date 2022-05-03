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
    Rigidbody rb_;

    float moveSpeed_ = 10f;
    float stayTime = 2f;
    float timer = 0f;
    Vector3 towards = Vector3.zero;
    float rotationAngle_ = 45f;

    bool isGoing = false;
    bool isArrive = false;

    private void Awake()
    {
        enemy_ = goTarget_.GetComponent<Enemy>();
        targetTransform_ = goTarget_.transform.GetChild(1).transform;
        targetCollider_ = goTarget_.GetComponent<CapsuleCollider>();

        rb_ = this.gameObject.GetComponent<Rigidbody>();
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
                    dest_ = Vector3.zero;
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
        //towards = Vector3.RotateTowards(Rigidbody.transform.forward, targetForward, (float)(rotationAngle_ * 0.5 * Mathf.Deg2Rad), 1f);
        towards = Vector3.RotateTowards(rb_.transform.forward, targetTransform_.position, rotationAngle_ * Mathf.Deg2Rad, 1f);

        if (Vector3.zero != towards)
        {
            Quaternion curRotation = Quaternion.LookRotation(towards);

            Vector3 angleClamp = curRotation.eulerAngles;
            curRotation.eulerAngles = new Vector3(Mathf.Clamp((angleClamp.x > 180) ? angleClamp.x - 360 : angleClamp.x, -45, 45), angleClamp.y, 0);

            rb_.transform.localRotation = curRotation;
        }
    }

}
