using System.Collections;
using Rng = UnityEngine.Random;

class NightmareFreddy : Animatronic, ITP
{
    private int _currentCam = 8;

    public NightmareFreddy(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Nightmare Freddy is coming to attack! Watch out for your vents.");
        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam8, true);
        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(10f, 25f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        bool didMove = false;

        switch(_currentCam)
        {
            case 8:
                if(Rng.Range(0, 2) == 0)
                {
                    Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam8, false);
                    Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam10, true);
                    _currentCam = 10;
                    didMove = true;
                }
                else
                {
                    if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.VFront))
                    {
                        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam8, false);
                        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam9, true);
                        _currentCam = 9;
                        didMove = true;
                    }
                }
                break;
            case 10:
                if(Rng.Range(0, 2) == 0)
                {
                    Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam10, false);
                    Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam8, true);
                    _currentCam = 8;
                    didMove = true;
                }
                else
                {
                    if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.VLeft))
                    {
                        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam10, false);
                        Instance.PlaySound(Constants.SOUND_NIGHTMARE_FREDDY_LAUGH);

                        yield return WaitFor(Rng.Range(5f, 15f));
                        if(ForcedSolve)
                            Instance.CloseDoor(UltraCustomNightScript.DoorPosition.VFront);
                        if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.VLeft))
                        {
                            Strike();
                            Instance.Log("Strike from Nightmare Freddy!");
                        }
                        else
                        {
                            Instance.PlaySound(Constants.SOUND_BANG);
                        }
                        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam10, true);
                    }
                }
                break;
            case 9:
                if(Rng.Range(0, 2) == 0)
                {
                    Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam9, false);
                    Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam11, true);
                    _currentCam = 11;
                    didMove = true;
                }
                else
                {
                    if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.VFront))
                    {
                        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam9, false);
                        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam8, true);
                        _currentCam = 9;
                        didMove = true;
                    }
                }
                break;
            case 11:
                if(Rng.Range(0, 2) == 0)
                {
                    Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam11, false);
                    Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam9, true);
                    _currentCam = 9;
                    didMove = true;
                }
                else
                {
                    if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.VRight))
                    {
                        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam11, false);
                        Instance.PlaySound(Constants.SOUND_NIGHTMARE_FREDDY_LAUGH);

                        yield return WaitFor(Rng.Range(5f, 15f));
                        if(ForcedSolve)
                            Instance.CloseDoor(UltraCustomNightScript.DoorPosition.VFront);
                        if(Instance.GetDoorClosed(UltraCustomNightScript.DoorPosition.VRight))
                        {
                            Strike();
                            Instance.Log("Strike from Nightmare Freddy!");
                        }
                        else
                        {
                            Instance.PlaySound(Constants.SOUND_BANG);
                        }
                        Instance.SetCameraFlag(CameraFlag.NightmareFreddyCam11, true);
                    }
                }
                break;
        }

        if(didMove)
            Instance.PlaySound(Constants.SOUND_NIGHTMARE_FREDDY_LAUGH);

        Instance.Log("Nightmare Freddy is now at cam {0}.", _currentCam);
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
