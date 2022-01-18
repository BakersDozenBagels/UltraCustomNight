using System.Collections;
using UnityEngine;
using Rng = UnityEngine.Random;

public class Ballora : Animatronic
{
    private BalloraScript _script;

    public Ballora(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Ballora is coming to attack! Listen close.");
        Instance.AddCoroutineNow(WaitToMove());
        GameObject o = Instance.PublicInstantiate(Instance.BalloraPrefab);
        _script = o.GetComponent<BalloraScript>();

        Instance.Destroy += () => { Destroy(); };
    }

    private void Destroy()
    {
        Object.Destroy(_script.gameObject);
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(60f, 120f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        bool left = Rng.Range(0, 2) == 0;

        Instance.Log("Ballora is coming from the {0}.", left ? "left" : "right");
        AudioSource src = left ? _script.Left : _script.Right;
        float start = Time.time;
        float end = 20f * TimeAdjust;
        while(Time.time - start < end)
        {
            src.volume = Mathf.Lerp(0f, 1f, (Time.time - start) / end);
            yield return null;
        }

        if(!Instance.GetDoorClosed(left ? UltraCustomNightScript.DoorPosition.Left : UltraCustomNightScript.DoorPosition.Right))
        {
            Instance.Log("Strike from Ballora!");
            Instance.Strike();
        }
        else
        {
            Instance.PlaySound(Constants.SOUND_BANG);
        }

        start = Time.time;
        end = 5f * TimeAdjust;
        while(Time.time - start < end)
        {
            src.volume = Mathf.Lerp(1f, 0f, (Time.time - start) / end);
            yield return null;
        }

        src.volume = 0f;

        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }
}