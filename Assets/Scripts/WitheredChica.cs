using System.Collections;
using Rng = UnityEngine.Random;

public class WitheredChica : Animatronic
{
    public WitheredChica(UltraCustomNightScript instance) : base(instance)
    {
        instance.AddCoroutineNow(Move());
        instance.Log("Withered Chica is coming to attack! Keep on your toes.");
    }

    private IEnumerator Move()
    {
        yield return WaitFor(Rng.Range(30f, 60f));

        Instance.Log("Withered Chica is attacking!");
        int cam = Instance.LastCamSelected < 8 ? Rng.Range(1, 8) : Rng.Range(8, 12);
        Instance.Cams.SetCam(cam);
        Instance.LastCamSelected = cam;
        Instance.OnCameraChange(false);

        Instance.AddCoroutineNow(Move());
    }
}