using System;
using UnityEngine;

/// <summary>
/// Encapsulates the functionality for a selectable character in the Ultra Custom Night module.
/// </summary>
public class CharacterSelectable : KMSelectable
{
    /// <summary>
    /// Holds the <see cref="Renderer"/> for this character's outline. Set by Unity before <see cref="Awake/>.
    /// </summary>
    [SerializeField]
    private Renderer _outline;

    /// <summary>
    /// The difficulty rating of this animatronic.
    /// </summary>
    [SerializeField]
    private int _difficulty;
    /// <summary>
    /// The difficulty rating of this animatronic.
    /// </summary>
    public int Difficulty { get { return _difficulty; } }

    /// <summary>
    /// Holds the <see cref="KMAudio"/> for this character. Set in <see cref="Start"/>.
    /// </summary>
    private KMAudio _audio;

    /// <summary>
    /// Enapsulates a callback for when a <see cref="CharacterSelectable"/> changes state.
    /// </summary>
    /// <param name="on"><code>true</code> if the character is being turned on.</param>
    public delegate void OnUpdateStateHandler(bool on);

    /// <summary>
    /// This characters callback for when it changes state.
    /// </summary>
    public OnUpdateStateHandler OnUpdateState { get; set; }

    /// <summary>
    /// Backing field for <see cref="CurrentState"/>. Use that instead.
    /// </summary>
    private State _currentState;
    /// <summary>
    /// Holds the current state of the selection for this character. Setting this value will automatically update visually.
    /// </summary>
    public State CurrentState {
        get
        {
            return _currentState;
        }
        set
        {
            if(value != State.ForcedOff && (_currentState == State.ForcedOn || _currentState == State.ForcedOff))
                return;
            State prev = _currentState;
            _currentState = value;
            SetOutlineColor();

            if(OnUpdateState != null && !(value == State.ForcedOff && prev == State.Off))
                OnUpdateState(value == State.ForcedOn || value == State.On);
        }
    }

    /// <summary>
    /// Updates the current outline color to be correct.
    /// </summary>
    private void SetOutlineColor()
    {
        switch(CurrentState)
        {
            case State.On:
                SetOutlineColor(OutlineColor.Green);
                break;
            case State.Off:
                SetOutlineColor(OutlineColor.Black);
                break;
            case State.ForcedOn:
                SetOutlineColor(OutlineColor.Red);
                break;
            case State.ForcedOff:
                SetOutlineColor(OutlineColor.Gray);
                break;
        }
    }

    /// <summary>
    /// Sets this character's outline to be the specified color.
    /// </summary>
    /// <param name="color">The <see cref="OutlineColor"/> to set to</param>
    private void SetOutlineColor(OutlineColor color)
    {
        _outline.material.color = GetColor(color);
    }

    /// <summary>
    /// Converts from an <see cref="OutlineColor"/> to a <see cref="Color"/>.
    /// </summary>
    /// <param name="c">The <see cref="OutlineColor"/> to convert.</param>
    /// <returns>A <see cref="Color"/> representing the enum value.</returns>
    private Color GetColor(OutlineColor c)
    {
        switch(c)
        {
            case OutlineColor.Black:
                return new Color(0.1f, 0.1f, 0.1f);
            case OutlineColor.Green:
                return new Color(0.1f, 0.9f, 0.1f);
            case OutlineColor.Red:
                return new Color(0.9f, 0.1f, 0.1f);
            case OutlineColor.Gray:
                return new Color(0.5f, 0.5f, 0.5f);
            default:
                throw new InvalidOperationException("Bad OutlineColor " + c);
        }
    }

    /// <summary>
    /// Generates a method to press this character.
    /// </summary>
    /// <returns>A <see cref="KMSelectable.OnInteractHandler"/> containing this object's press functionality.</returns>
    private OnInteractHandler HandlePress()
    {
        return delegate ()
        {
            AddInteractionPunch(0.1f);

            if(CurrentState != State.ForcedOn && CurrentState != State.ForcedOff) // We don't want to change a forced state.
            {
                _audio.PlaySoundAtTransform(Constants.SOUND_BEEP, transform);

                if(CurrentState == State.On)
                    CurrentState = State.Off;
                else if(CurrentState == State.Off)
                    CurrentState = State.On;
            }
            else
                _audio.PlaySoundAtTransform(Constants.SOUND_ERROR, transform);

            return false; // We do not have any children to try to select.
        };
    }

    #region Unity Lifecycle
    /// <summary>
    /// Called immediately after this <see cref="CharacterSelectable"/> is instantiated.
    /// </summary>
    private void Awake()
    {
        CurrentState = State.Off;
    }

    /// <summary>
    /// Called one frame after this <see cref="CharacterSelectable"/> is instantiated.
    /// </summary>
    private void Start()
    {
        _audio = GetComponentInParent<KMAudio>();
        OnInteract += HandlePress();
    }
    #endregion

    /// <summary>
    /// Represents a possible color for an outline.
    /// </summary>
    private enum OutlineColor
    {
        Red,
        Green,
        Black,
        Gray
    }

    /// <summary>
    /// Represents a possible state of selection for a character.
    /// </summary>
    public enum State
    {
        On,
        Off,
        ForcedOn,
        ForcedOff
    }
}