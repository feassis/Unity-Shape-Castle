using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Sprite buildButtonIcon;

    public Sprite GetBuildingIcon() { return buildButtonIcon; }
}
