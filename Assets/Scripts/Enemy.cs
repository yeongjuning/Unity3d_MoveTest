using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public bool IsTriggerEnter { private set; get; }

    private void OnTriggerEnter(Collider other)
    {
        IsTriggerEnter = true;
        //if (other.gameObject.tag == "Player")
        //{
        //    Debug.Log("<color=cyan>OnTriggerEnter : </color>" + other.gameObject.name);
        //    IsTriggerEnter = true;
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        IsTriggerEnter = false;
    }
}
