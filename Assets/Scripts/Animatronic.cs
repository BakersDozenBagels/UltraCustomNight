using System;
using UnityEngine;
/// <summary>
/// Encapsulates the funtionality of a single enemy on and Ultra Custom Night module.
/// </summary>
public abstract class Animatronic
{
    protected float TimeAdjust { get { return Instance.TimeAdjust; } }
    protected bool ForcedSolve { get; set; }

    /// <summary>
    /// Backing field for <see cref="Instance"/>. Use that instead.
    /// </summary>
    private readonly UltraCustomNightScript _instance;

    /// <summary>
    /// Holds the <see cref="UltraCustomNightScript"/> that this <see cref="Animatronic"/> is attached to.
    /// </summary>
    protected UltraCustomNightScript Instance { get { return _instance; } }

    /// <summary>
    /// Creates a new <see cref="Animatronic"/>.
    /// </summary>
    /// <param name="instance">The <see cref="UltraCustomNightScript"/> that this <see cref="Animatronic"/> is attached to.</param>
    protected Animatronic(UltraCustomNightScript instance)
    {
        if(instance == null)
            throw new ArgumentNullException("instance", "Null instance passed to an Animatronic.");

        _instance = instance;
    }

    /// <summary>
    /// Returns an object that tells a coroutine to wait for a certain amount of time.
    /// </summary>
    /// <param name="time">The time to wait for.</param>
    /// <returns>The wait command.</returns>
    protected WaitForSeconds WaitFor(float time)
    {
        return new WaitForSeconds(time * TimeAdjust);
    }

    /// <summary>
    /// Creates a new instance of a derivative of <see cref="Animatronic"/> by its name.
    /// </summary>
    /// <param name="name">The type to create.</param>
    /// <param name="instance">The instance to pass to the animatronic.</param>
    /// <returns>The animatronic generated.</returns>
    public static Animatronic GetByName(string name, UltraCustomNightScript instance)
    {
        switch(name)
        {
            case "FreddyFazbear":
                return new FreddyFazbear(instance);
            case "Bonnie":
                return new Bonnie(instance);
            case "Chica":
                return new Chica(instance);
            case "Foxy":
                return new Foxy(instance);
            case "ToyFreddy":
                return new ToyFreddy(instance);
            case "ToyChica":
                return new ToyChica(instance);
            case "ToyBonnie":
                return new ToyBonnie(instance);
            case "Mangle":
                return new Mangle(instance);
            case "NightmareFreddy":
                return new NightmareFreddy(instance);
            case "NightmareBonnie":
                return new GoldenFreddy(instance);
            case "NightmareChica":
                return new FuntimeFoxy(instance);
            case "NightmareFoxy":
                return new NightmareFoxy(instance);
            case "CircusBaby":
                return new CircusBaby(instance);
            case "Ballora":
                return new NightmareBB(instance);
            case "FuntimeFreddy":
                return new FuntimeFreddy(instance);
            case "FuntimeFoxy":
                return new Ballora(instance);
            case "Springtrap":
                return new Springtrap(instance);
            case "ThePuppet":
                return new ThePuppet(instance);
            case "BB":
                return new BB(instance, false);
            case "JJ":
                return new BB(instance, true);
            default:
                // return new DummyAnimatronic(instance);
                throw new ArgumentException("Unexpected Animatronic Name!", "name");
        }
    }

    private string GetName()
    {
        if(this is FreddyFazbear)
            return "Freddy Fazbear";
        if(this is Bonnie)
            return "Bonnie";
        if(this is Chica)
            return "Chica";
        if(this is Foxy)
            return "Foxy";
        if(this is ToyFreddy)
            return "Toy Freddy";
        if(this is ToyChica)
            return "Toy Chica";
        if(this is ToyBonnie)
            return "Toy Bonnie";
        if(this is Mangle)
            return "Mangle";
        if(this is NightmareFreddy)
            return "Nightmare Freddy";
        if(this is GoldenFreddy)
            return "Nightmare Bonnie";
        if(this is FuntimeFoxy)
            return "Nightmare Chica";
        if(this is NightmareFoxy)
            return "Nightmare Foxy";
        if(this is CircusBaby)
            return "CircusBaby";
        if(this is NightmareBB)
            return "Ballora";
        if(this is FuntimeFreddy)
            return "Funtime Freddy";
        if(this is Ballora)
            return "Funtime Foxy";
        if(this is Springtrap)
            return "Springtrap";
        if(this is ThePuppet)
            return "The Puppet";
        if(this is BB && (this as BB)._jj)
            return "JJ";
        else if(this is BB)
            return "BB";
        if(this is NightmareBonnie)
            return "Nightmare_Bonnie";
        if(this is Lolbit)
            return "Lolbit";
        throw new InvalidOperationException("Unexpected Animatronic Type!");
    }

    protected void Strike()
    {
        Instance.Strike(GetName());
    }

    //public class DummyAnimatronic : Animatronic
    //{
    //    public DummyAnimatronic(UltraCustomNightScript instance) : base(instance) { }
    //}
}
