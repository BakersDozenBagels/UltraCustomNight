using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ThePuppetColliderScript : MonoBehaviour
{
    public event Action<bool> OMO;

    private Collider _collider;

    private void MouseExit()
    {
        if(OMO != null)
            OMO(false);
    }

    private void MouseEnter()
    {
        if(OMO != null)
            OMO(true);
    }

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        if(Input.mousePosition.x < 0 || Input.mousePosition.y < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y > Screen.height)
        {
            MouseExit();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach(RaycastHit hit in Physics.RaycastAll(ray))
        {
            if(hit.collider == _collider)
            {
                MouseEnter();
                return;
            }
        }
        MouseExit();
    }
}
