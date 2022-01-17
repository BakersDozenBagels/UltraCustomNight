using System.Collections;
using UnityEngine;
using Rng = UnityEngine.Random;

public class FuntimeFreddy : Animatronic
{
    public FuntimeFreddy(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Funtime Freddy is coming to attack! Listen carefully.");
        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(60f, 120f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        int dir = Rng.Range(0, 3);
        bool simon = Rng.Range(0, 2) == 0;
        if(simon)
        {
            Instance.PlaySound(Constants.SOUND_FUNTIME_FREDDY_SIMON);
            yield return new WaitForSeconds(1.724f);
        }
        Instance.PlaySound(dir == 0 ? Constants.SOUND_FUNTIME_FREDDY_LEFT : dir == 1 ? Constants.SOUND_FUNTIME_FREDDY_UP : Constants.SOUND_FUNTIME_FREDDY_RIGHT);

        Instance.Log("Funtime Freddy said \"{0}{1}!\"", simon ? "Simon says, " : "", dir == 0 ? "left" : dir == 1 ? "up" : "right");
        if(!simon)
        {
            dir += 1;
            dir %= 3;
        }
        yield return WaitFor(Rng.Range(5f, 15f));
        if(!Instance.GetDoorClosed(dir == 0 ? UltraCustomNightScript.DoorPosition.Left : dir == 1 ? UltraCustomNightScript.DoorPosition.Front : UltraCustomNightScript.DoorPosition.Right))
        {
            Instance.Log("Strike from Funtime Freddy!");
            Instance.Strike();
        }
        else
        {
            Instance.PlaySound(Constants.SOUND_BANG);
        }

        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }
}