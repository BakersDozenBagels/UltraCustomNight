using System;
using System.Linq;
using UnityEngine;
using DP = UltraCustomNightScript.DoorPosition;

/// <summary>
/// Encapsulates the functionality for a selectable door in the Ultra Custom Night module.
/// </summary>
public class DoorSelectable : KMSelectable
{
    /// <summary>
    /// Holds the <see cref="Renderer"/> for this door 's outline. Set by Unity before <see cref="Awake/>.
    /// </summary>
    [SerializeField]
    private Renderer _outline;

    /// <summary>
    /// The id of this door.
    /// </summary>
    [SerializeField]
    private DP _id;
    /// <summary>
    /// The id rating of this door.
    /// </summary>
    public DP Id { get { return _id; } }

    /// <summary>
    /// Whether this door is closed.
    /// </summary>
    public bool IsClosed { get; private set; }

    /// <summary>
    /// Holds the <see cref="KMAudio"/> for this door. Set in <see cref="Start"/>.
    /// </summary>
    private KMAudio _audio;

    /// <summary>
    /// Enapsulates a callback for when a <see cref="doorSelectable"/> gets selected.
    /// </summary>
    public delegate void OnSelectHandler(DP id);

    /// <summary>
    /// This door's callback for when it gets selected.
    /// </summary>
    public OnSelectHandler OnDoorSelect { get; set; }

    /// <summary>
    /// Sets this door's outline to be the specified color.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to set to</param>
    private void SetOutlineColor(Color color)
    {
        _outline.material.color = color;
    }

    /// <summary>
    /// Generates a method to press this door.
    /// </summary>
    /// <returns>A <see cref="KMSelectable.OnInteractHandler"/> containing this object's press functionality.</returns>
    private OnInteractHandler HandlePress()
    {
        return delegate ()
        {
            AddInteractionPunch(0.1f);

            _audio.PlaySoundAtTransform(Constants.SOUND_DOOR, transform);
            SetOutlineColor(Color.green);
            IsClosed = true;
            if(OnDoorSelect != null)
                OnDoorSelect(Id);

            return false; // We do not have any children to try to select.
        };
    }

    #region Unity Lifecycle
    /// <summary>
    /// Called one frame after this <see cref="DoorSelectable"/> is instantiated.
    /// </summary>
    private void Start()
    {
        _audio = GetComponentInParent<KMAudio>();
        OnInteract += HandlePress();
        SetOutlineColor(Color.black);

        Func<DoorSelectable, bool> predicate = null;
        switch(Id)
        {
            case DP.Left:
            case DP.Right:
            case DP.Front:
                predicate = s => s.Id == DP.Left || s.Id == DP.Right || s.Id == DP.Front;
                break;
            case DP.VLeft:
            case DP.VRight:
            case DP.VFront:
                predicate = s => s.Id == DP.VLeft || s.Id == DP.VRight || s.Id == DP.VFront;
                break;
        }

        foreach(DoorSelectable sel in transform.parent.parent.parent.GetComponentsInChildren<DoorSelectable>().Where(predicate))
        {
            if(sel == this)
                continue;
            sel.OnDoorSelect += i => { SetOutlineColor(Color.black); IsClosed = false;  };
        }
    }
    #endregion
}