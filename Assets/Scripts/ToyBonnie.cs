using System.Collections;
using Rng = UnityEngine.Random;

class ToyBonnie : Animatronic, ITP
{
    private int _currentCam;

    public ToyBonnie(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Toy Bonnie is coming to attack! Watch out for cams 6 and 7.");
        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(30f, 75f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        _currentCam = Rng.Range(0, 2) == 0 ? 6 : 7;
        Instance.SetCameraFlag(_currentCam == 6 ? CameraFlag.ToyBonnieCam6 : CameraFlag.ToyBonnieCam7, true);
        Instance.PlaySound(Constants.SOUND_BANG2);
        Instance.Log("Toy Bonnie is now at cam {0}.", _currentCam);
        yield return WaitFor(Rng.Range(10f, 20f));
        if(ForcedSolve)
            Instance.CloseDoor(_currentCam == 6 ? UltraCustomNightScript.DoorPosition.Left : UltraCustomNightScript.DoorPosition.Right);
        if(Instance.GetDoorClosed(_currentCam == 6 ? UltraCustomNightScript.DoorPosition.Left : UltraCustomNightScript.DoorPosition.Right))
            Instance.PlaySound(Constants.SOUND_BANG);
        else
        {
            Strike();
            Instance.Log("Strike from Toy Bonnie!");
        }
        Instance.SetCameraFlag(_currentCam == 6 ? CameraFlag.ToyBonnieCam6 : CameraFlag.ToyBonnieCam7, false);

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
