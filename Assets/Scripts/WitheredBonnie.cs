using System.Collections;
using Rng = UnityEngine.Random;

public class WitheredBonnie : Animatronic
{
    public WitheredBonnie(UltraCustomNightScript instance) : base(instance)
    {
        instance.AddCoroutineNow(Move());
        instance.Log("Withered Bonnie is coming to attack! Keep on your toes.");
    }

    private IEnumerator Move()
    {
        yield return WaitFor(Rng.Range(30f, 60f));

        Instance.Log("Withered Bonnie is attacking!");
        Instance.GetComponentsInChildren<DoorSelectable>().PickRandom().OnInteract();

        Instance.AddCoroutineNow(Move());
    }
}