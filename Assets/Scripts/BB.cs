using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rng = UnityEngine.Random;

class BB : Animatronic
{
    public bool _jj;
    private static readonly List<GameObject> _selected = new List<GameObject>();
    private KMBombModule _currentModule;
    private bool _handled, _removed;
    private GameObject _cover, _obj;

    private KMSelectable.OnInteractHandler _f;

    public BB(UltraCustomNightScript instance, bool jj) : base(instance)
    {
        Instance.Log("{0} is coming to attack! Watch out for other modules.", jj ? "JJ" : "BB");
        _jj = jj;
        Instance.AddCoroutineNow(Move());
        Instance.AddCoroutineNow(Update());
        Instance.Destroy += () => { Destroy(); };

        _f = () =>
        {
            if(_currentModule != _obj.GetComponent<KMBombModule>() || _handled)
                return true;

            if(_jj)
            {
                Instance.Log("Strike from JJ!");
                Strike();
                _handled = true;
            }
            else
            {
                Object.Destroy(_cover);
                _currentModule = null;
                _selected.Remove(_obj);
                _handled = true;
                _removed = true;
            }
            return true;
        };
    }

    private void Destroy()
    {
        if(!_removed)
        {
            Object.Destroy(_cover);
            _removed = true;
        }
    }

    private IEnumerator Update()
    {
        while(true)
        {
            _handled = false;
            yield return null;
        }
    }

    private IEnumerator Move()
    {
        yield return WaitFor(Rng.Range(60f, 90f));

        if(Object.FindObjectsOfType<KMBombModule>().Where(m => !_selected.Contains(m.gameObject)).Count() < 1)
        {
            Instance.AddCoroutineNow(Move());
            yield break;
        }

        _currentModule = Object.FindObjectsOfType<KMBombModule>().Where(m => !_selected.Contains(m.gameObject) && !Instance.Ignored.Contains(m.ModuleDisplayName) && m.ModuleDisplayName != "Ultra Custom Night").PickRandom();
        _obj = _currentModule.gameObject;
        _selected.Add(_obj);
        _cover = Instance.PublicInstantiate(Instance.BBJJPrefab);
        _cover.transform.parent = _obj.transform;
        _cover.transform.localPosition = Vector3.zero;
        _cover.transform.localEulerAngles = Vector3.zero;
        _cover.transform.localScale = Vector3.one;
        _cover.GetComponentInChildren<Renderer>().material.color = _jj ? Color.magenta : Color.blue;
        _removed = false;
        _obj.GetComponent<KMSelectable>().OnInteract += _f;

        Instance.Log("{0} is attacking!", _jj ? "JJ" : "BB");
        Instance.PlaySound(Constants.SOUND_BB_JJ);

        yield return new WaitForSeconds(30f);

        if(!_jj && !_removed)
        {
            Instance.Log("Strike from BB!");
            Strike();
        }

        if(!_removed)
        {
            _obj.GetComponent<KMSelectable>().OnInteract -= _f;
            Object.Destroy(_cover);
            _currentModule = null;
            _selected.Remove(_obj);
        }

        Instance.AddCoroutineNow(Move());
    }
}
