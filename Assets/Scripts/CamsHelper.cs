using UnityEngine;

public class CamsHelper : MonoBehaviour
{
    [SerializeField]
    private Texture[] _camTextures;
    [SerializeField]
    private Texture _static;
    [SerializeField]
    private Renderer _camRenderer;
    [SerializeField]
    private GameObject[] _miscObjects;
    private bool[] _miscObjectsEnabled;
    private int _currentCam;

    private void Awake()
    {
        _miscObjectsEnabled = new bool[_miscObjects.Length];
        foreach(GameObject o in _miscObjects)
            o.SetActive(false);
    }

    public void SetCam()
    {
        _camRenderer.material.mainTexture = _static;
        foreach(GameObject o in _miscObjects)
            o.SetActive(false);
        _currentCam = 0;
    }

    public void SetCam(int camNo)
    {
        _camRenderer.material.mainTexture = _camTextures[camNo - 1];

        for(int i = 0; i < _miscObjects.Length; i++)
        {
            _miscObjects[i].SetActive(false);
            if(_miscObjects[i].transform.parent.name == camNo.ToString())
                _miscObjects[i].SetActive(_miscObjectsEnabled[i]);
        }
        _currentCam = camNo;
    }

    public void SetMiscObject(CameraFlag id, bool on)
    {
        if(_miscObjects[(int)id].transform.parent.name == _currentCam.ToString())
            _miscObjects[(int)id].SetActive(on);
        _miscObjectsEnabled[(int)id] = on;
    }
}
