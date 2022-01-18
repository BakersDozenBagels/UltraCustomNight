using System.Collections;
using UnityEngine;
using Rng = UnityEngine.Random;

class NightmareBB : Animatronic
{
    private NBBScript _script;
    private bool _isActive;

    public NightmareBB(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Nightmare BB is coming to attack! Watch out to your right.");
        Instance.AddCoroutineNow(Move());
        Instance.Destroy += () => { Destroy(); };
        _script = Instance.PublicInstantiate(Instance.NBBPrefab).GetComponent<NBBScript>();
        _script.Inactive.SetActive(true);
        _script.Active.SetActive(false);
        _script.OnClick += Click;
    }

    private void Click()
    {
        if(_isActive)
        {
            _isActive = false;
            _script.Inactive.SetActive(true);
            _script.Active.SetActive(false);
        }
        else
        {
            Instance.Log("Strike from Nightmare BB!");
            Instance.Strike();
        }
    }

    private void Destroy()
    {
        Object.Destroy(_script.gameObject);
    }

    private IEnumerator Move()
    {
        yield return WaitFor(Rng.Range(60f, 90f));
        _script.Inactive.SetActive(false);
        _script.Active.SetActive(true);
        Instance.Log("Nightmare BB is attacking!");
        _isActive = true;

        yield return WaitFor(Rng.Range(60f, 900f));
        if(_isActive)
        {
            Instance.Log("Strike from Nightmare BB!");
            Instance.Strike();
        }
        _script.Inactive.SetActive(true);
        _script.Active.SetActive(false);

        Instance.AddCoroutineNow(Move());
    }
}
