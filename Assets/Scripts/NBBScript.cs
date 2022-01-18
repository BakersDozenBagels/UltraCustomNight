using System;
using UnityEngine;

public class NBBScript : MonoBehaviour
{
    private Collider _collider;

    public event Action OnClick = () => { };

    public GameObject Active, Inactive;

    private void Awake()
    {
        _collider = GetComponentInChildren<Collider>();
    }

    private void Update()
    {
        if(!Input.GetMouseButtonDown(0))
            return;

        if(Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height)
            return;

        if(OnClick == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach(RaycastHit hit in Physics.RaycastAll(ray))
        {
            if(hit.collider == _collider)
            {
                OnClick();
                return;
            }
        }
    }
}