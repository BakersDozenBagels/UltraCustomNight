using UnityEngine;

public class ThePuppetScript : MonoBehaviour
{
    public Transform SpriteTransform;
    public Renderer SpriteRenderer;

    private void Start()
    {
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }
}
