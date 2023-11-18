using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Enapsulates the functionality for the Ultra Custom Night module.
/// </summary>
[RequireComponent(typeof(KMSelectable), typeof(KMBossModule))]
[RequireComponent(typeof(KMAudio), typeof(KMBombModule))]
[RequireComponent(typeof(KMGameInfo))]
public partial class UltraCustomNightScript : MonoBehaviour
{
    /// <summary>
    /// Holds every instance of this module. Added to in <see cref="Awake"/>.
    /// </summary>
    private static readonly List<UltraCustomNightScript> _instances = new List<UltraCustomNightScript>();

    /// <summary>
    /// Holds the prefab for Circus Baby.
    /// </summary>
    public GameObject CircusBabyPrefab;
    /// <summary>
    /// Holds the prefab for Ballora.
    /// </summary>
    public GameObject BalloraPrefab;
    /// <summary>
    /// Holds the prefab for The Puppet.
    /// </summary>
    public GameObject ThePuppetPrefab;
    /// <summary>
    /// Holds the prefab for BB and JJ.
    /// </summary>
    public GameObject BBJJPrefab;
    /// <summary>
    /// Holds the prefab for Nightmare BB.
    /// </summary>
    public GameObject NBBPrefab;
    /// <summary>
    /// Holds the prefab for the special animatronics.
    /// </summary>
    public GameObject SpecialPrefab;
    /// <summary>
    /// Holds the text for Funtime Foxy.
    /// </summary>
    public TextMesh FuntimeFoxyText;
    /// <summary>
    /// The time threshold for Mangle.
    /// </summary>
    public float MangleThreshold { get; private set; }

    /// <summary>
    /// Set in <see cref="Awake"/>.
    /// </summary>
    private KMSelectable _selectable;

    /// <summary>
    /// Set in <see cref="Awake"/>.
    /// </summary>
    private KMBossModule _boss;
    /// <summary>
    /// Set in <see cref="Awake"/>.
    /// </summary>
    private KMAudio _audio;
    /// <summary>
    /// Set in <see cref="Awake"/>.
    /// </summary>
    private KMBombModule _module;
    /// <summary>
    /// Set in <see cref="Awake"/>.
    /// </summary>
    public CamsHelper Cams { get; private set; }

    /// <summary>
    /// Set by TP via Reflection.
    /// </summary>
    private bool TwitchPlaysActive;
    /// <summary>
    /// Whether the bomb was started in Zen mode. Set during lights-on.
    /// </summary>
    public bool IsZenModeActive { get; private set; }
    public bool IsTPActive { get { return TwitchPlaysActive; } }
    /// <summary>
    /// Set in <see cref="Awake"/>.
    /// </summary>
    public KMBombInfo BombInfo { get; private set; }
    /// <summary>
    /// Set in <see cref="Awake"/>.
    /// </summary>
    public KMGameInfo GameInfo { get; private set; }

    [SerializeField]
    private GameObject _setupScreen, _camScreen, _ventScreen, _basicScreen, _camsScreen, _strikeIndicator;
    [SerializeField]
    private KMSelectable _camSel, _ventSel;
    [SerializeField]
    private Texture[] _strikeTextures;

    /// <summary>
    /// Whether this module is solved.
    /// </summary>
    private bool _isSolved;
    /// <summary>
    /// Whether the module has started.
    /// </summary>
    private bool _hasStarted;
    /// <summary>
    /// Used to generate <see cref="_id"/>.
    /// </summary>
    private static int _idc;
    /// <summary>
    /// This module's unique logging id.
    /// </summary>
    private int _id = ++_idc;
    /// <summary>
    /// The currently selected animatronics' total difficulty rating.
    /// </summary>
    private int _currentDR;
    /// <summary>
    /// All module names that this module should ignore.
    /// </summary>
    public string[] Ignored { get; private set; }
    /// <summary>
    /// The last camera that was selected.
    /// </summary>
    public int LastCamSelected { get; set; }
    /// <summary>
    /// Whether this module has had its unique animatronics assigned.
    /// </summary>
    private bool _assignedUniques;
    /// <summary>
    /// Whether this module is allowed to use this animatronic.
    /// </summary>
    private bool _mangleAllowed, _circusBabyAllowed, _balloraAllowed, _funtimeFreddyAllowed, _nightmareBBAllowed;

    /// <summary>
    /// The factor with which to scale all wait times.
    /// </summary>
    [HideInInspector]
#if UNITY_EDITOR
    public float TimeAdjust = 21f; //0.5f;
#else
    public float TimeAdjust = 4.5f;
#endif

    [SerializeField]
    private GameObject _goldenFreddy;

    #region Unity Lifecycle
    /// <summary>
    /// Called immediately after this <see cref="UltraCustomNightScript"/> is instantiated.
    /// </summary>
    private void Awake()
    {
        _instances.Add(this);
        _selectable = GetComponent<KMSelectable>();
        _boss = GetComponent<KMBossModule>();
        _audio = GetComponent<KMAudio>();
        _module = GetComponent<KMBombModule>();
        BombInfo = GetComponent<KMBombInfo>();
        GameInfo = GetComponent<KMGameInfo>();
        Cams = GetComponentInChildren<CamsHelper>();

        _setupScreen.SetActive(true);
        _camScreen.SetActive(true);
        _ventScreen.SetActive(true);
        _basicScreen.SetActive(true);
        _camsScreen.SetActive(true);
    }

    /// <summary>
    /// Called one frame after this <see cref="UltraCustomNightScript"/> is instantiated.
    /// </summary>
    private void Start()
    {
        OnCameraChange += b => { };
        GameObject light = transform.GetChild(8).gameObject;
        light.SetActive(false);
        GameInfo.OnLightsChange += s => light.SetActive(!s);

        BombInfo.OnBombExploded += () => { if(Destroy != null) Destroy(); };

        MangleThreshold = BombInfo.GetTime() / 20f;

        Ignored = _boss.GetIgnoredModules(_module);
        StartCoroutine(WatchForSolves());

        _selectable.Children = _setupScreen.GetComponentsInChildren<KMSelectable>();
        _selectable.UpdateChildren();

        foreach(DoorSelectable sel in GetComponentsInChildren<DoorSelectable>())
        {
            sel.OnDoorSelect += i =>
            {
                Predicate<DoorPosition> predicate = null;
                switch(i)
                {
                    case DoorPosition.Left:
                    case DoorPosition.Right:
                    case DoorPosition.Front:
                        predicate = s => s == DoorPosition.Left || s == DoorPosition.Right || s == DoorPosition.Front;
                        break;
                    case DoorPosition.VLeft:
                    case DoorPosition.VRight:
                    case DoorPosition.VFront:
                        predicate = s => s == DoorPosition.VLeft || s == DoorPosition.VRight || s == DoorPosition.VFront;
                        break;
                }
                _closed.RemoveAll(predicate);
                _closed.Add(i);
            };
        }

        _camSel.OnInteract += () => { OnCameraChange(false); LastCamSelected = 0; _camSel.AddInteractionPunch(0.1f); _audio.PlaySoundAtTransform(Constants.SOUND_STATIC, _camSel.transform); UpdateScreen(1); return false; };
        _ventSel.OnInteract += () => { OnCameraChange(false); LastCamSelected = 0; _ventSel.AddInteractionPunch(0.1f); _audio.PlaySoundAtTransform(Constants.SOUND_STATIC, _ventSel.transform); UpdateScreen(2); return false; };

        foreach(CameraSelectable sel in GetComponentsInChildren<CameraSelectable>())
            sel.OnCameraSelect += i => { Cams.SetCam(i); LastCamSelected = i; OnCameraChange(true); };

        if(!_assignedUniques)
        {
            UltraCustomNightScript[] ucns = _instances.Where(u => u != null && !u._assignedUniques).ToArray();
            ucns.PickRandom()._mangleAllowed = true;
            ucns.PickRandom()._circusBabyAllowed = true;
            ucns.PickRandom()._balloraAllowed = true;
            ucns.PickRandom()._funtimeFreddyAllowed = true;
            ucns.PickRandom()._nightmareBBAllowed = true;
            foreach(UltraCustomNightScript ucn in ucns)
                ucn._assignedUniques = true;
        }

        StartCoroutine(StartPlusOne());

        _module.OnActivate += () =>
        {
            IsZenModeActive = BombInfo.GetTime() < 1f;

            foreach(CharacterSelectable sel in GetComponentsInChildren<CharacterSelectable>())
            {
                bool IsVrEnabled = KtaneVRChecker.VRC.IsEnabled;

                if((!_mangleAllowed || IsZenModeActive || IsTPActive) && sel.gameObject.name == "Mangle")
                    sel.CurrentState = CharacterSelectable.State.ForcedOff;
                if((!_circusBabyAllowed || IsTPActive || IsVrEnabled) && sel.gameObject.name == "CircusBaby")
                    sel.CurrentState = CharacterSelectable.State.ForcedOff;
                if(!_funtimeFreddyAllowed && sel.gameObject.name == "FuntimeFreddy")
                    sel.CurrentState = CharacterSelectable.State.ForcedOff;
                if(!_balloraAllowed && sel.gameObject.name == "FuntimeFoxy")
                    sel.CurrentState = CharacterSelectable.State.ForcedOff;
                if((!_nightmareBBAllowed || IsTPActive || IsVrEnabled) && sel.gameObject.name == "Ballora")
                    sel.CurrentState = CharacterSelectable.State.ForcedOff;
                if((IsVrEnabled || IsTPActive) && sel.gameObject.name == "ThePuppet")
                    sel.CurrentState = CharacterSelectable.State.ForcedOff;
                if(IsTPActive && (sel.gameObject.name == "BB" || sel.gameObject.name == "JJ"))
                    sel.CurrentState = CharacterSelectable.State.ForcedOff;

                sel.OnUpdateState += GenerateToggle(sel);
            }

#if !UNITY_EDITOR
        if(!IsZenModeActive)
        {
            while(_currentDR < 7)
            {
                GetComponentsInChildren<CharacterSelectable>().PickRandom().CurrentState = CharacterSelectable.State.ForcedOn;
            }
        }
#endif
        };
    }

    private IEnumerator StartPlusOne()
    {
        yield return null;

        _camScreen.SetActive(false);
        _ventScreen.SetActive(false);
        _basicScreen.SetActive(false);
        _camsScreen.SetActive(false);
    }
    #endregion

    /// <summary>
    /// Generates a listener for a <see cref="CharacterSelectable"/>.
    /// </summary>
    /// <param name="sel">The specific character to listen for.</param>
    /// <returns>The listener.</returns>
    private CharacterSelectable.OnUpdateStateHandler GenerateToggle(CharacterSelectable sel)
    {
        return delegate (bool on)
        {
            if(on)
                _currentDR += sel.Difficulty;
            else
                _currentDR -= sel.Difficulty;

            _setupScreen.GetComponentInChildren<TextMesh>().text = _currentDR + "/20";
        };
    }

    /// <summary>
    /// Generates a coroutine to listen for solves of other modules on the bomb.
    /// </summary>
    /// <returns>The coroutine.</returns>
    private IEnumerator WatchForSolves()
    {
        int requiredSolves = BombInfo.GetSolvableModuleNames().Count(n => n != "Ultra Custom Night" && !Ignored.Contains(n));

        while(!_isSolved)
        {
            yield return new WaitForSeconds(1f);
            int newCount = BombInfo.GetSolvedModuleNames().Count(n => n != "Ultra Custom Night" && !Ignored.Contains(n));
            if(!_hasStarted && newCount >= 1)
            {
                _hasStarted = true;
                Activate();
            }
            if(newCount >= requiredSolves)
            {
                Log("Enough modules have been solved. Therefore, you are done. Good job.");
                _module.HandlePass();
                StopAllCoroutines();
                _isSolved = true;
                if(Destroy != null)
                    Destroy();
            }
        }
    }

    /// <summary>
    /// Updates which screen is displayed.
    /// </summary>
    /// <param name="id"><code>1</code> for cams, <code>2</code> for vents.</param>
    private void UpdateScreen(int id)
    {
        _camScreen.SetActive(false);
        _ventScreen.SetActive(false);
        _basicScreen.SetActive(true);
        Cams.SetCam();
        List<KMSelectable> sels;
        switch(id)
        {
            case 1:
                _camScreen.SetActive(true);
                sels = _camScreen.GetComponentsInChildren<KMSelectable>().ToList();
                sels.AddRange(_basicScreen.GetComponentsInChildren<KMSelectable>().ToList());
                sels.AddRange(_camScreen.GetComponentsInChildren<DoorSelectable>().Cast<KMSelectable>().ToList());
                _selectable.Children = sels.ToArray();
                foreach(CameraSelectable sel in GetComponentsInChildren<CameraSelectable>())
                    sel.SetOutlineColor(Color.black);
                _selectable.UpdateChildren();
                break;
            case 2:
                _ventScreen.SetActive(true);
                sels = _ventScreen.GetComponentsInChildren<KMSelectable>().ToList();
                sels.AddRange(_basicScreen.GetComponentsInChildren<KMSelectable>().ToList());
                sels.AddRange(_ventScreen.GetComponentsInChildren<DoorSelectable>().Cast<KMSelectable>().ToList());
                _selectable.Children = sels.ToArray();
                foreach(CameraSelectable sel in GetComponentsInChildren<CameraSelectable>())
                    sel.SetOutlineColor(Color.black);
                _selectable.UpdateChildren();
                break;
        }
    }

    #region Animatronic Hooks
    /// <summary>
    /// Holds every Coroutine to be ran sequentially.
    /// </summary>
    private Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();

    /// <summary>
    /// Starts a Coroutine to be ran when the module is ready.
    /// </summary>
    /// <remarks>
    /// Yield return <see langword="true">true</see> to move to the next item in the queue.
    /// </remarks>
    /// <param name="routine">The Coroutine to start.</param>
    public void AddCoroutineToQueue(IEnumerator routine)
    {
        if(_isSolved)
            return;
        _coroutineQueue.Enqueue(routine);
    }

    /// <summary>
    /// Starts a Coroutine immediately.
    /// </summary>
    /// <param name="routine">The Coroutine to start.</param>
    public void AddCoroutineNow(IEnumerator routine)
    {
        if(_isSolved)
            return;
        StartCoroutine(routine);
    }

    /// <summary>
    /// Causes the module to register a strike.
    /// </summary>
    public void Strike(string callerName = null)
    {
        if(_isSolved || !_canStrike)
            return;

        if(callerName != null)
        {
            _strikeIndicator.SetActive(true);
            _strikeIndicator.GetComponent<Renderer>().material.mainTexture = _strikeTextures.First(t => t.name == callerName);
        }

        _module.HandleStrike();
    }

    /// <summary>
    /// Logs a message from this module in a format in acordance with the LFA.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Any format arguments to insert.</param>
    public void Log(string message, params object[] args)
    {
        if(_isSolved)
            return;
        UnityEngine.Debug.LogFormat("[Ultra Custom Night #{0}] {1}", _id, string.Format(message, args));
    }

    /// <summary>
    /// Sets a certain camera to have a certain property.
    /// </summary>
    /// <param name="flag">The flag to set.</param>
    /// <param name="value">The value to set the flag to.</param>
    public void SetCameraFlag(CameraFlag flag, bool value)
    {
        Cams.SetMiscObject(flag, value);
    }

    /// <summary>
    /// Plays a sound.
    /// </summary>
    /// <param name="name">The sound to play.</param>
    public void PlaySound(string name)
    {
        if(_isSolved)
            return;
        _audio.PlaySoundAtTransform(name, transform);
    }

    /// <summary>
    /// Plays a game sound.
    /// </summary>
    /// <param name="sfx">The sound effect to play.</param>
    internal void PlayGameSound(KMSoundOverride.SoundEffect sfx)
    {
        if(_isSolved)
            return;
        _audio.PlayGameSoundAtTransform(sfx, transform);
    }

    private readonly List<DoorPosition> _closed = new List<DoorPosition>();

    /// <summary>
    /// Gets whether a certain door is closed.
    /// </summary>
    /// <param name="pos">The door to check.</param>
    /// <returns>Whether the door is closed.</returns>
    public bool GetDoorClosed(DoorPosition pos)
    {
        return _closed.Contains(pos);
    }

    /// <summary>
    /// A public version of Instatiate.
    /// </summary>
    /// <param name="orig">The object to copy.</param>
    /// <returns>The new copy.</returns>
    public GameObject PublicInstantiate(GameObject orig)
    {
        return Instantiate(orig);
    }

    public void SetGoldenFreddy(bool on)
    {
        _goldenFreddy.SetActive(on);
    }

    public Action<bool> OnCameraChange;

    public new event Action Destroy;
    #endregion

    /// <summary>
    /// Called after one module has been solved. Starts all active <see cref="Animatronic"/>s.
    /// </summary>
    private void Activate()
    {
        StartCoroutine(HandleAnimatronicQueue());

#if !UNITY_EDITOR
        while(_currentDR < 20)
            GetComponentsInChildren<CharacterSelectable>().Where(s => s.CurrentState == CharacterSelectable.State.Off).PickRandom().CurrentState = CharacterSelectable.State.On;
        
        TimeAdjust *= Mathf.Pow(0.99f, _currentDR - 20);
        if(IsTPActive)
            TimeAdjust *= 3f;
#endif

        bool allOn = true;
        //bool allOn = false;

        foreach(CharacterSelectable sel in GetComponentsInChildren<CharacterSelectable>())
        {
            Animatronic a;
            if(sel.CurrentState == CharacterSelectable.State.On || sel.CurrentState == CharacterSelectable.State.ForcedOn)
            {
                if((a = Animatronic.GetByName(sel.gameObject.name, this)) is ITP)
                    _TPHandlers.Add(a as ITP);
            }
            else
                allOn = false;
        }

        if(allOn)
            new SpecialAnimatronics(this);

        _setupScreen.SetActive(false);
        _camsScreen.SetActive(true);

        UpdateScreen(1);
        SetGoldenFreddy(false);
        _strikeIndicator.SetActive(false);

        _started = true;
    }

    /// <summary>
    /// Runs the queue of Coroutines activated by the active <see cref="Animatronic"/>s.
    /// </summary>
    /// <returns>The combined Coroutine.</returns>
    private IEnumerator HandleAnimatronicQueue()
    {
        while(!_isSolved)
        {
            if(_coroutineQueue.Count > 0)
            {
                if(!_coroutineQueue.Peek().MoveNext())
                {
                    _coroutineQueue.Dequeue();
                    continue;
                }

                object o = _coroutineQueue.Peek().Current;
                if(o != null && o.Equals(true))
                {
                    _coroutineQueue.Enqueue(_coroutineQueue.Dequeue());
                    yield return null;
                }
                else
                {
                    yield return o;
                }
            }
            else
                yield return null;
        }
    }

    public enum DoorPosition
    {
        Left,
        Right,
        Front,
        VLeft,
        VRight,
        VFront
    }
}
