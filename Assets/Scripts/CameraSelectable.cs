using System;
using UnityEngine;

/// <summary>
/// Encapsulates the functionality for a selectable camera in the Ultra Custom Night module.
/// </summary>
public class CameraSelectable : KMSelectable
{
    /// <summary>
    /// Holds the <see cref="Renderer"/> for this character's outline. Set by Unity before <see cref="Awake/>.
    /// </summary>
    [SerializeField]
    private Renderer _outline;

    /// <summary>
    /// The id of this camera.
    /// </summary>
    [SerializeField]
    private int _id;
    /// <summary>
    /// The id of this camera.
    /// </summary>
    public int Id { get { return _id; } }

    /// <summary>
    /// Holds the <see cref="KMAudio"/> for this camera. Set in <see cref="Start"/>.
    /// </summary>
    private KMAudio _audio;

    /// <summary>
    /// Enapsulates a callback for when a <see cref="CameraSelectable"/> gets selected.
    /// </summary>
    public delegate void OnSelectHandler(int id);

    /// <summary>
    /// This camera's callback for when it gets selected.
    /// </summary>
    public OnSelectHandler OnCameraSelect { get; set; }

    /// <summary>
    /// Sets this Camera's outline to be the specified color.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to set to</param>
    public void SetOutlineColor(Color color)
    {
        _outline.material.color = color;
    }

    /// <summary>
    /// Generates a method to press this Camera.
    /// </summary>
    /// <returns>A <see cref="KMSelectable.OnInteractHandler"/> containing this object's press functionality.</returns>
    private OnInteractHandler HandlePress()
    {
        return delegate ()
        {
            AddInteractionPunch(0.1f);

            _audio.PlaySoundAtTransform(Constants.SOUND_STATIC, transform);
            SetOutlineColor(Color.green);
            if(OnCameraSelect != null)
                OnCameraSelect(Id);

            return false; // We do not have any children to try to select.
        };
    }

    #region Unity Lifecycle
    /// <summary>
    /// Called one frame after this <see cref="CameraSelectable"/> is instantiated.
    /// </summary>
    private void Start()
    {
        _audio = GetComponentInParent<KMAudio>();
        OnInteract += HandlePress();
        SetOutlineColor(Color.black);

        foreach(CameraSelectable sel in transform.parent.parent.parent.GetComponentsInChildren<CameraSelectable>())
        {
            if(sel == this)
                continue;
            sel.OnCameraSelect += i => SetOutlineColor(Color.black);
        }
    }
    #endregion
}