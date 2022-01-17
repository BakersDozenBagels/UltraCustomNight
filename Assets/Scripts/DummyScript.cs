using UnityEngine;

public class DummyScript : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<KMSelectable>().OnInteract += () => { GetComponent<KMBombModule>().HandlePass(); return false; };
    }
}
