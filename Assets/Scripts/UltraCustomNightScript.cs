using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Enapsulates the functionality for the Ultra Custom Night module.
/// </summary>
[RequireComponent(typeof(KMSelectable), typeof(KMBossModule))]
[RequireComponent(typeof(KMAudio), typeof(KMBombModule))]
[RequireComponent(typeof(KMGameInfo))]
public class UltraCustomNightScript : MonoBehaviour
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
    private CamsHelper _cams;

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
    private GameObject _setupScreen, _camScreen, _ventScreen, _basicScreen, _camsScreen;
    [SerializeField]
    private KMSelectable _camSel, _ventSel;

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
    private string[] _ignored;
    /// <summary>
    /// The last camera that was selected.
    /// </summary>
    public int LastCamSelected { get; private set; }
    /// <summary>
    /// Whether this module has had its unique animatronics assigned.
    /// </summary>
    private bool _assignedUniques;
    /// <summary>
    /// Whether this module is allowed to use this animatronic.
    /// </summary>
    private bool _mangleAllowed, _nightmareBonnieAllowed, _circusBabyAllowed, _balloraAllowed, _funtimeFreddyAllowed, _funtimeFoxyAllowed, _puppetAllowed, _BBAllowed, _JJAllowed;

    /// <summary>
    /// The factor with which to scale all wait times.
    /// </summary>
    [HideInInspector]
#if UNITY_EDITOR
    public float TimeAdjust = 0.5f;
#else
    public float TimeAdjust = 4.5f;
#endif

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
        _cams = GetComponentInChildren<CamsHelper>();

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
        MangleThreshold = BombInfo.GetTime() / 20f;

        _ignored = _boss.GetIgnoredModules(_module);
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

        _camSel.OnInteract += () => { LastCamSelected = 0; _camSel.AddInteractionPunch(0.1f); _audio.PlaySoundAtTransform(Constants.SOUND_STATIC, _camSel.transform); UpdateScreen(1); return false; };
        _ventSel.OnInteract += () => { LastCamSelected = 0; _ventSel.AddInteractionPunch(0.1f); _audio.PlaySoundAtTransform(Constants.SOUND_STATIC, _ventSel.transform); UpdateScreen(2); return false; };

        foreach(CameraSelectable sel in GetComponentsInChildren<CameraSelectable>())
            sel.OnCameraSelect += i => { _cams.SetCam(i); LastCamSelected = i; };

        if(!_assignedUniques)
        {
            UltraCustomNightScript[] ucns = _instances.Where(u => u != null && !u._assignedUniques).ToArray();
            ucns.PickRandom()._mangleAllowed = true;
            ucns.PickRandom()._nightmareBonnieAllowed = true;
            ucns.PickRandom()._circusBabyAllowed = true;
            ucns.PickRandom()._balloraAllowed = true;
            ucns.PickRandom()._funtimeFreddyAllowed = true;
            ucns.PickRandom()._funtimeFoxyAllowed = true;
            ucns.PickRandom()._puppetAllowed = true;
            ucns.PickRandom()._BBAllowed = true;
            ucns.PickRandom()._JJAllowed = true;
            foreach(UltraCustomNightScript ucn in ucns)
                ucn._assignedUniques = true;
        }

        StartCoroutine(StartPlusOne());

        _module.OnActivate += () =>
        {
            IsZenModeActive = BombInfo.GetTime() < 1f;
            foreach(CharacterSelectable sel in GetComponentsInChildren<CharacterSelectable>())
                if((!_mangleAllowed || IsZenModeActive) && sel.gameObject.name == "Mangle")
                    sel.CurrentState = CharacterSelectable.State.ForcedOff;
        };
    }

    private IEnumerator StartPlusOne()
    {
        yield return null;

        foreach(CharacterSelectable sel in GetComponentsInChildren<CharacterSelectable>())
        {
            bool IsVrEnabled = KtaneVRChecker.VRC.IsEnabled;

            sel.OnUpdateState += GenerateToggle(sel);
            if((!_mangleAllowed || IsZenModeActive || IsTPActive) && sel.gameObject.name == "Mangle")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
            if((!_nightmareBonnieAllowed || IsTPActive) && sel.gameObject.name == "NightmareBonnie")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
            if((!_circusBabyAllowed || IsTPActive || IsVrEnabled) && sel.gameObject.name == "CircusBaby")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
            if(!_balloraAllowed && sel.gameObject.name == "Ballora")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
            if(!_funtimeFreddyAllowed && sel.gameObject.name == "FuntimeFreddy")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
            if(!_funtimeFoxyAllowed && sel.gameObject.name == "FuntimeFoxy")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
            if((!_puppetAllowed || IsTPActive || IsVrEnabled) && sel.gameObject.name == "ThePuppet")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
            if((!_BBAllowed || IsTPActive || IsVrEnabled) && sel.gameObject.name == "BB")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
            if((!_JJAllowed || IsTPActive || IsVrEnabled) && sel.gameObject.name == "JJ")
                sel.CurrentState = CharacterSelectable.State.ForcedOff;
        }

#if !UNITY_EDITOR
        while(_currentDR < 7)
        {
            GetComponentsInChildren<CharacterSelectable>().PickRandom().CurrentState = CharacterSelectable.State.ForcedOn;
        }
#endif

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
        int requiredSolves = BombInfo.GetSolvableModuleNames().Count(n => n != "Ultra Custom Night" && !_ignored.Contains(n));

        while(!_isSolved)
        {
            yield return new WaitForSeconds(1f);
            int newCount = BombInfo.GetSolvedModuleNames().Count(n => n != "Ultra Custom Night" && !_ignored.Contains(n));
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
        _cams.SetCam();
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
    /// Yield return <code>true</code> to move to the next item in the queue.
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
    public void Strike()
    {
        if(_isSolved)
            return;
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
        Debug.LogFormat("[Ultra Custom Night #{0}] {1}", _id, string.Format(message, args));
    }

    /// <summary>
    /// Sets a certain camera to have a certain property.
    /// </summary>
    /// <param name="flag">The flag to set.</param>
    /// <param name="value">The value to set the flag to.</param>
    public void SetCameraFlag(CameraFlag flag, bool value)
    {
        _cams.SetMiscObject(flag, value);
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

        foreach(CharacterSelectable sel in GetComponentsInChildren<CharacterSelectable>())
            if(sel.CurrentState == CharacterSelectable.State.On || sel.CurrentState == CharacterSelectable.State.ForcedOn)
                Animatronic.GetByName(sel.gameObject.name, this);

        _setupScreen.SetActive(false);
        _camsScreen.SetActive(true);

        UpdateScreen(1);
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
