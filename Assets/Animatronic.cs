using System;
/// <summary>
/// Encapsulates the funtionality of a single enemy on and Ultra Custom Night module.
/// </summary>
public abstract class Animatronic
{
    /// <summary>
    /// Holds the <see cref="UltraCustomNightScript"/> that this <see cref="Animatronic"/> is attached to.
    /// </summary>
    protected readonly UltraCustomNightScript _instance;

    /// <summary>
    /// Creates a new <see cref="Animatronic"/>.
    /// </summary>
    /// <param name="instance">The <see cref="UltraCustomNightScript"/> that this <see cref="Animatronic"/> is attached to.</param>
    private Animatronic(UltraCustomNightScript instance)
    {
        if(instance == null)
            throw new ArgumentNullException("instance", "Null instance passed to an Animatronic.");

        _instance = instance;
    }

    /// <summary>
    /// Creates a new instance of a derivative of <see cref="Animatronic"/> by its name.
    /// </summary>
    /// <param name="name">The type to create.</param>
    /// <param name="instance">The instance to pass to the module.</param>
    /// <returns></returns>
    public static Animatronic GetByName(string name, UltraCustomNightScript instance)
    {
        switch(name)
        {
            case "abc":
                return new Animatronic(instance);
            default:
                throw new ArgumentException("Unexpected Animatronic Name!", "name");
        }
    }
}
