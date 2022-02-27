using System.Collections;
using Rng = UnityEngine.Random;

class ToyFreddy : Animatronic, ITP
{
    private int _currentCam = 3;

    public ToyFreddy(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Toy Freddy is coming to attack! Watch out for your vents.");
        Instance.SetCameraFlag(CameraFlag.ToyFreddyCam3, true);
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
            case 1:
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam1, false);
                if(Rng.Range(0, 2) == 0)
                {
                    Instance.SetCameraFlag(CameraFlag.ToyFreddyCam3, true);
                    _currentCam = 3;
                }
                else
                {
                    Instance.SetCameraFlag(CameraFlag.ToyFreddyCam8, true);
                    _currentCam = 8;
                }
                break;
            case 2:
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam2, false);
                if(Rng.Range(0, 2) == 0)
                {
                    Instance.SetCameraFlag(CameraFlag.ToyFreddyCam3, true);
                    _currentCam = 3;
                }
                else
                {
                    Instance.SetCameraFlag(CameraFlag.ToyFreddyCam9, true);
                    _currentCam = 9;
                }
                break;
            case 3:
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam3, false);
                if(Rng.Range(0, 2) == 0)
                {
                    Instance.SetCameraFlag(CameraFlag.ToyFreddyCam1, true);
                    _currentCam = 1;
                }
                else
                {
                    Instance.SetCameraFlag(CameraFlag.ToyFreddyCam2, true);
                    _currentCam = 2;
                }
                break;
            case 8:
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam8, false);
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam10, true);
                _currentCam = 10;
                break;
            case 9:
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam9, false);
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam11, true);
                _currentCam = 11;
                break;
            case 10:
            case 11:
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam10, false);
                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam11, false);
                yield return WaitFor(Rng.Range(5f, 15f));
                if(ForcedSolve)
                    Instance.CloseDoor(_currentCam == 10 ? UltraCustomNightScript.DoorPosition.VLeft : UltraCustomNightScript.DoorPosition.VRight);
                if(Instance.GetDoorClosed(_currentCam == 10 ? UltraCustomNightScript.DoorPosition.VLeft : UltraCustomNightScript.DoorPosition.VRight))
                    Instance.PlaySound(Constants.SOUND_BANG);
                else
                {
                    Strike();
                    Instance.Log("Strike from Toy Freddy!");
                }

                Instance.SetCameraFlag(CameraFlag.ToyFreddyCam3, true);
                _currentCam = 3;
                break;
        }

        Instance.Log("Toy Freddy is now at cam {0}.", _currentCam);
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
