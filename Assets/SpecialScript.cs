using System;
using UnityEngine;

public class SpecialScript : MonoBehaviour
{
    public SpriteRenderer[] Sprites;
    public GameObject Lol;

    private void Start()
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }
}
