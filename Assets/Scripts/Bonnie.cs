﻿using System.Collections;
using Rng = UnityEngine.Random;

class Bonnie : Animatronic
{
    private int _currentCam = 2;

    public Bonnie(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Bonnie is coming to attack! Watch out for your right hallway.");
        Instance.SetCameraFlag(CameraFlag.BonnieCam2, true);
        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(10f, 25f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        switch(_currentCam)
        {
            case 2:
                _currentCam = 3;
                Instance.SetCameraFlag(CameraFlag.BonnieCam2, false);
                Instance.SetCameraFlag(CameraFlag.BonnieCam3, true);
                break;
            case 3:
                _currentCam = 5;
                Instance.SetCameraFlag(CameraFlag.BonnieCam3, false);
                Instance.SetCameraFlag(CameraFlag.BonnieCam5, true);
                break;
            case 5:
                _currentCam = 7;
                Instance.SetCameraFlag(CameraFlag.BonnieCam5, false);
                Instance.SetCameraFlag(CameraFlag.BonnieCam7, true);
                break;
            case 7:
                _currentCam = 0;
                Instance.SetCameraFlag(CameraFlag.BonnieCam7, false);
                yield return WaitFor(Rng.Range(5f, 10f));
                if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.Right))
                    Instance.PlaySound(Constants.SOUND_BANG);
                else
                {
                    Instance.Strike();
                    Instance.Log("Strike from Bonnie!");
                }
                _currentCam = 2;
                Instance.SetCameraFlag(CameraFlag.BonnieCam2, true);
                break;
        }

        Instance.Log("Bonnie is now at cam {0}.", _currentCam);
        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }
}