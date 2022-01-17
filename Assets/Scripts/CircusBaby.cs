using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Rng = UnityEngine.Random;

public class CircusBaby : Animatronic
{
    private CircusBabyScript _script;
    private Component _bomb;
    private PropertyInfo _info;

    public CircusBaby(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Circus Baby is coming to attack! Watch out to your left.");
#if !UNITY_EDITOR
        Instance.AddCoroutineNow(WaitToMove());
        GameObject o = Instance.PublicInstantiate(Instance.CircusBabyPrefab);
        _script = o.GetComponent<CircusBabyScript>();
        _script.Inactive.SetActive(true);
        _script.Active.SetActive(false);

        _bomb = Instance.GetComponentsInParent<Component>().Where(c => c.GetType().Name.Contains("FloatingHoldable")).First();
        _info = _bomb.GetType().GetProperty("HoldState", BindingFlags.Public | BindingFlags.Instance);
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(120f, 180f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        Instance.Log("Circus Baby is now active.");
        _script.Inactive.SetActive(false);
        _script.Active.SetActive(true);
        yield return new WaitForSeconds(60f);

        if((int)_info.GetValue(_bomb, new object[0]) == 0)
        {
            Instance.Log("Strike from Circus Baby!");
            Instance.Strike();
        }

        _script.Inactive.SetActive(true);
        _script.Active.SetActive(false);

        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
#endif
    }
}