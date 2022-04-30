using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public bool IsTriggerEnter { set; get; }

    private void OnTriggerEnter(Collider other)
    {
        IsTriggerEnter = true;
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    IsTriggerEnter = false;
    //}
}
