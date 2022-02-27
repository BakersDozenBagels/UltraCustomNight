using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Rng = UnityEngine.Random;

public class SpecialAnimatronics : Animatronic
{
    private SpecialScript _script;

    public SpecialAnimatronics(UltraCustomNightScript instance) : base(instance)
    {
        instance.AddCoroutineNow(WaitToActivate());
    }

    private IEnumerator WaitToActivate()
    {
#if !UNITY_EDITOR
        yield return new WaitForSeconds(Rng.Range(5f, 10f)); // 300f
#endif

        Instance.AddCoroutineToQueue(Activate());
        _script = Instance.PublicInstantiate(Instance.SpecialPrefab).GetComponent<SpecialScript>();
        for(int i = 0; i < 5; i++)
        {
            _script.Sprites[i].enabled = false;
        }
        yield break;
    }

    private IEnumerator Activate()
    {
#if !UNITY_EDITOR
        IEnumerable<Component> lights = UnityEngine.Object.FindObjectsOfType<Component>().Where(c => c.GetType().Name.Contains("CeilingLight"));
        List<Type> types = new List<Type>();
        foreach(Component light in lights)
        {
            Type typ = light.GetType();
            if(typ.Name.Contains("Mod"))
                typ = typ.BaseType;

            types.Add(typ);
        }
        
        Type t = types[0];

        MethodInfo mOff = t.GetMethod("TurnOff", BindingFlags.Public | BindingFlags.Instance);
        MethodInfo mOn = t.GetMethod("TurnOn", BindingFlags.Public | BindingFlags.Instance);
#endif

        Instance.PlaySound(Constants.SOUND_NEW_CHALLENGER);
        yield return new WaitForSeconds(2.269f);

#if !UNITY_EDITOR
        foreach(Component light in lights)
            mOff.Invoke(light, new object[] { false });
#endif

        yield return new WaitForSeconds(9.628f - 2.269f);
        const float delay = 1.904f;
        Instance.AddCoroutineNow(Flash());

        // Lolbit, !Nightmare BB, !Golden Freddy, Withered Chica, Withered Bonnie
        Action[] anims = new Action[] {
            () => { new NightmareBonnie(Instance); },
            () => { new NightmareChica(Instance); },
            () => { new Lolbit(Instance, _script); },
            () => { new WitheredChica(Instance); },
            () => { new WitheredBonnie(Instance); }
        };

        for(int i = 0; i < 5; i++)
        {
            Instance.Log("A new Challenger is approaching!");
            anims[i]();
            _script.Sprites[i].enabled = true;
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(1.87f);

        for(int i = 0; i < 5; i++)
        {
            _script.Sprites[i].enabled = false;
        }

#if !UNITY_EDITOR
        foreach(Component light in lights)
            mOn.Invoke(light, new object[] { true });
#endif
        Instance.PlayGameSound(KMSoundOverride.SoundEffect.Switch);

        yield break;
    }

    private IEnumerator Flash()
    {
        float start = Time.time;
        while(1.904f * 5f + 1.87f > Time.time - start)
        {
            for(int i = 0; i < 5; i++)
            {
                _script.Sprites[i].color = Color.HSVToRGB((Time.time - start) % 1f, 1f, 1f);
            }
            yield return null;
        }
    }
}