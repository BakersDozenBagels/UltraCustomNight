using System;
using System.Collections;
using UnityEngine;
using Rng = UnityEngine.Random;

public class FuntimeFoxy : Animatronic, ITP
{
    DateTime _sign;

    public FuntimeFoxy(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Funtime Foxy is coming to attack! Watch cam 2 carefully.");
        Instance.AddCoroutineNow(WaitForTime());
        Instance.SetCameraFlag(CameraFlag.FuntimeFoxyCam2, true);
    }

    private IEnumerator WaitForTime()
    {
        _sign = DateTime.Now.AddMinutes(Rng.Range(2f, 5f));
        string s = (_sign.Hour <= 9 ? "0" + _sign.Hour : _sign.Hour.ToString()) + ":" + (_sign.Minute <= 9 ? "0" + _sign.Minute : _sign.Minute.ToString());
        Instance.FuntimeFoxyText.text = s;
        Instance.Log("Funtime foxy is attacking at {0}.", s);

        yield return new WaitUntil(() => DateTime.Now.Hour == _sign.Hour && DateTime.Now.Minute == _sign.Minute);

        if(ForcedSolve)
        {
            Instance.CloseDoor(UltraCustomNightScript.DoorPosition.Front);
            Instance.Cams.SetCam(2);
            Instance.LastCamSelected = 2;
            Instance.OnCameraChange(true);
        }

        if(Instance.LastCamSelected != 2)
        {
            Instance.Log("Strike from Funtime Foxy!");
            Strike();
        }

        Instance.AddCoroutineNow(WaitForTime());
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