using System.Collections;
using Rng = UnityEngine.Random;

class NightmareChica : Animatronic
{
    private int _currentCam = 1;

    public NightmareChica(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Nightmare Chica is coming to attack! Be ready for blacked-out cameras.");
        Instance.AddCoroutineNow(WaitToMove());
        Instance.SetCameraFlag(CameraFlag.NightmareChicaCam1, true);
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
            case 1:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam1, false);
                break;
            case 2:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam2, false);
                break;
            case 3:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam3, false);
                break;
            case 4:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam4, false);
                break;
            case 5:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam5, false);
                break;
            case 6:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam6, false);
                break;
            case 7:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam7, false);
                break;
            case 8:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam8, false);
                break;
            case 9:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam9, false);
                break;
            case 10:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam10, false);
                break;
            case 11:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam11, false);
                break;
        }
        _currentCam = Rng.Range(0, 12);
        switch(_currentCam)
        {
            case 1:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam1, true);
                break;
            case 2:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam2, true);
                break;
            case 3:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam3, true);
                break;
            case 4:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam4, true);
                break;
            case 5:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam5, true);
                break;
            case 6:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam6, true);
                break;
            case 7:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam7, true);
                break;
            case 8:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam8, true);
                break;
            case 9:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam9, true);
                break;
            case 10:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam10, true);
                break;
            case 11:
                Instance.SetCameraFlag(CameraFlag.NightmareChicaCam11, true);
                break;
        }

        Instance.Log("Nightmare Chica is at cam {0}.", _currentCam);
        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }
}
