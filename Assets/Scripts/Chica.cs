using System.Collections;
using Rng = UnityEngine.Random;

class Chica : Animatronic, ITP
{
    private int _currentCam = 2;

    public Chica(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Chica is coming to attack! Watch out for your right hallway.");
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
                if(ForcedSolve)
                    Instance.CloseDoor(UltraCustomNightScript.DoorPosition.Right);
                if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.Right))
                    Instance.PlaySound(Constants.SOUND_BANG);
                else
                {
                    Strike();
                    Instance.Log("Strike from Chica!");
                }
                _currentCam = 2;
                Instance.SetCameraFlag(CameraFlag.BonnieCam2, true);
                break;
        }

        Instance.Log("Chica is now at cam {0}.", _currentCam);
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
