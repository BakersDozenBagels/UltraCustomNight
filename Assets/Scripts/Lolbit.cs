using System.Collections;
using UnityEngine;
using Rng = UnityEngine.Random;

public class Lolbit : Animatronic
{
    private GameObject _lol;
    private int _state;

    public Lolbit(UltraCustomNightScript instance, SpecialScript lol) : base(instance)
    {
        _lol = lol.Lol;
        instance.AddCoroutineNow(Move());
        instance.AddCoroutineNow(Update());
        instance.Log("Lolbit is coming to attack! Keep your keyboard ready.");
    }

    private IEnumerator Update()
    {
        while(true)
        {
            yield return null;
            if(_state == 3 && Input.GetKeyDown(KeyCode.L))
            {
                _lol.SetActive(false);
                _state = 0;
            }
            if(_state == 1 && Input.GetKeyDown(KeyCode.L))
                _state++;
            if(_state == 2 && Input.GetKeyDown(KeyCode.O))
                _state++;
        }
    }

    private IEnumerator Move()
    {
        yield return WaitFor(Rng.Range(60f, 90f));
        Instance.Log("Lolbit is attacking!");
        Instance.PlaySound(Constants.SOUND_LOL);
        _lol.SetActive(true);
        _state = 1;
        yield return WaitFor(10f);
        if(_state != 0)
        {
            _state = 0;
            Instance.Log("Strike from Lolbit!");
            Strike();
        }
        _lol.SetActive(false);
        Instance.AddCoroutineNow(Move());
    }
}