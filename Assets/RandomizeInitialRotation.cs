using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeInitialRotation : MonoBehaviour
{
    private void Start()
    {
        transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }
}
