using System.Collections;
using Rng = UnityEngine.Random;

class Springtrap : Animatronic, ITP
{
    private int _currentCam = 10;

    public Springtrap(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Springtrap is coming to attack! Watch out for your vents.");
        Instance.SetCameraFlag(CameraFlag.SpringtrapCam10, true);
        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(5f, 15f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        switch(_currentCam)
        {
            case 10:
                _currentCam = 8;
                Instance.SetCameraFlag(CameraFlag.SpringtrapCam10, false);
                Instance.SetCameraFlag(CameraFlag.SpringtrapCam8, true);
                break;
            case 8:
                Instance.SetCameraFlag(CameraFlag.SpringtrapCam8, false);
                if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.VFront))
                {
                    _currentCam = 10;
                    Instance.SetCameraFlag(CameraFlag.SpringtrapCam10, true);
                    Instance.PlaySound(Constants.SOUND_BANG);
                }
                else
                {
                    _currentCam = 9;
                    Instance.SetCameraFlag(CameraFlag.SpringtrapCam9, true);
                }
                break;
            case 9:
                _currentCam = 11;
                Instance.SetCameraFlag(CameraFlag.SpringtrapCam9, false);
                Instance.SetCameraFlag(CameraFlag.SpringtrapCam11, true);
                break;
            case 11:
                _currentCam = 10;
                Instance.SetCameraFlag(CameraFlag.SpringtrapCam11, false);
                yield return WaitFor(Rng.Range(5f, 15f));
                if(ForcedSolve)
                    Instance.CloseDoor(UltraCustomNightScript.DoorPosition.VRight);
                if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.VRight))
                    Instance.PlaySound(Constants.SOUND_BANG);
                else
                {
                    Strike();
                    Instance.Log("Strike from Springtrap!");
                }
                Instance.SetCameraFlag(CameraFlag.SpringtrapCam10, true);
                break;
        }

        Instance.Log("Springtrap is now at cam {0}.", _currentCam);
        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
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
