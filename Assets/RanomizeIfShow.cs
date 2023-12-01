using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanomizeIfShow : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float showPercentage;

    private void Start()
    {
        float randNum = Random.Range(0f, 1f);

        if(randNum > showPercentage)
        {
            gameObject.SetActive(false);
        }
    }
}
