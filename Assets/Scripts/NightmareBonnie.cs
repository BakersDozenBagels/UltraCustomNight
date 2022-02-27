using System.Collections;
using Rng = UnityEngine.Random;

class NightmareBonnie : Animatronic
{
    private int _currentCam = 5;

    public NightmareBonnie(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Nightmare Bonnie is coming to attack! Watch out for cam 3.");
        Instance.SetCameraFlag(CameraFlag.ChicaCam5, true);
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
            case 3:
                switch(Rng.Range(0, 3))
                {
                    case 0:
                        Instance.SetCameraFlag(CameraFlag.ChicaCam3, false);
                        Instance.SetCameraFlag(CameraFlag.ChicaCam4, true);
                        _currentCam = 4;
                        break;
                    case 1:
                        Instance.SetCameraFlag(CameraFlag.ChicaCam3, false);
                        Instance.SetCameraFlag(CameraFlag.ChicaCam5, true);
                        _currentCam = 5;
                        break;
                    case 2:
                        Instance.SetCameraFlag(CameraFlag.ChicaCam3, false);
                        Instance.SetCameraFlag(CameraFlag.ChicaCam3Attack, true);
                        _currentCam = 0;
                        yield return WaitFor(Rng.Range(10f, 15f));
                        if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.Front))
                            Instance.PlaySound(Constants.SOUND_BANG);
                        else
                        {
                            Strike();
                            Instance.Log("Strike from Nightmare Bonnie!");
                        }
                        Instance.SetCameraFlag(CameraFlag.ChicaCam3Attack, false);
                        Instance.SetCameraFlag(CameraFlag.ChicaCam3, true);
                        _currentCam = 3;
                        break;
                }
                break;
            case 4:
                Instance.SetCameraFlag(CameraFlag.ChicaCam4, false);
                Instance.SetCameraFlag(CameraFlag.ChicaCam3, true);
                _currentCam = 3;
                break;
            case 5:

                Instance.SetCameraFlag(CameraFlag.ChicaCam5, false);
                Instance.SetCameraFlag(CameraFlag.ChicaCam3, true);
                _currentCam = 3;
                break;
        }

        Instance.Log("Nightmare Bonnie is now at cam {0}.", _currentCam);
        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }
}
