using System.Collections;
using Rng = UnityEngine.Random;

class ToyChica : Animatronic
{
    private int _currentCam;

    public ToyChica(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Toy Chica is coming to attack! Watch out for cams 10 and 11.");
        Instance.AddCoroutineNow(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(30f, 75f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        _currentCam = Rng.Range(0, 2) == 0 ? 10 : 11;
        Instance.SetCameraFlag(_currentCam == 10 ? CameraFlag.ToyChicaCam10 : CameraFlag.ToyChicaCam11, true);
        Instance.PlaySound(Constants.SOUND_BANG2);
        Instance.Log("Toy Chica is now at cam {0}.", _currentCam);
        yield return WaitFor(Rng.Range(5f, 15f));
        if(Instance.GetDoorClosed(_currentCam == 10 ? UltraCustomNightScript.DoorPosition.VLeft : UltraCustomNightScript.DoorPosition.VRight))
            Instance.PlaySound(Constants.SOUND_BANG);
        else
        {
            Instance.Strike();
            Instance.Log("Strike from Toy Chica!");
        }

        Instance.SetCameraFlag(_currentCam == 10 ? CameraFlag.ToyChicaCam10 : CameraFlag.ToyChicaCam11, false);

        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }
}
