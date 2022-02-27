using System.Collections;
using UnityEngine;
using Rng = UnityEngine.Random;

class GoldenFreddy : Animatronic
{
    private bool _attacking;

    public GoldenFreddy(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Golden Freddy is coming to attack! Watch out for your cameras.");
        Instance.OnCameraChange += b => { Instance.AddCoroutineNow(Move(b)); };
    }

    private IEnumerator Move(bool b)
    {
        _attacking = false;
        Instance.SetGoldenFreddy(false);
        yield return null;
        yield return null;
        if(b && Rng.Range(0, 20) < 2)
        {
            _attacking = true;
            Instance.Log("Golden Freddy is attacking!");
            Instance.SetGoldenFreddy(true);
            float time = Time.time;
            while(Time.time < time + 5f * TimeAdjust)
            {
                yield return null;
                if(!_attacking)
                    yield break;
            }
            if(ForcedSolve)
            {
                Instance.Cams.SetCam();
                Instance.LastCamSelected = 0;
                Instance.OnCameraChange(false);
            }
            else
            {
                Instance.Log("Strike from Golden Freddy!");
                Strike();
            }
        }
    }

    public IEnumerable HandleTwitchCommand(string command)
    {
        yield break;
    }

    public IEnumerable HandleTwitchForcedSolve()
    {
        ForcedSolve = true;
        yield break;
    }
}
