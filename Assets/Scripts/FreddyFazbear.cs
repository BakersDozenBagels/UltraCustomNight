using System.Collections;
using Rng = UnityEngine.Random;

class FreddyFazbear : Animatronic
{
    private int _currentCam = 1;

    public FreddyFazbear(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Freddy Fazbear is coming to attack! Watch out for your left hallway.");
        Instance.SetCameraFlag(CameraFlag.FreddyCam1, true);
        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(10f, 25f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        Instance.PlaySound(Constants.SOUND_FREDDY_LAUGH);
        switch(_currentCam)
        {
            case 1:
                _currentCam = 3;
                Instance.SetCameraFlag(CameraFlag.FreddyCam1, false);
                Instance.SetCameraFlag(CameraFlag.FreddyCam3, true);
                break;
            case 3:
                _currentCam = 4;
                Instance.SetCameraFlag(CameraFlag.FreddyCam3, false);
                Instance.SetCameraFlag(CameraFlag.FreddyCam4, true);
                break;
            case 4:
                _currentCam = 6;
                Instance.SetCameraFlag(CameraFlag.FreddyCam4, false);
                Instance.SetCameraFlag(CameraFlag.FreddyCam6, true);
                break;
            case 6:
                _currentCam = 0;
                Instance.SetCameraFlag(CameraFlag.FreddyCam6, false);
                yield return WaitFor(Rng.Range(5f, 10f));
                if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.Left))
                    Instance.PlaySound(Constants.SOUND_BANG);
                else
                {
                    Instance.Strike();
                    Instance.Log("Strike from Freddy Fazbear!");
                }
                _currentCam = 1;
                Instance.SetCameraFlag(CameraFlag.FreddyCam1, true);
                break;
        }

        Instance.Log("Freddy Fazbear is now at cam {0}.", _currentCam);
        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }
}
