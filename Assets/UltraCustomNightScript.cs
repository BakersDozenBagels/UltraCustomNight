using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enapsulates the functionality for the Ultra Custom Night module.
/// </summary>
[RequireComponent(typeof(KMSelectable), typeof(KMBossModule))]
[RequireComponent(typeof(KMAudio), typeof(KMBombModule))]
public class UltraCustomNightScript : MonoBehaviour
{
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
    /// Whether this module is solved.
    /// </summary>
    private bool _isSolved;
    /// <summary>
    /// Used to generate <see cref="_id"/>.
    /// </summary>
    private static int _idc;
    /// <summary>
    /// This module's unique logging id.
    /// </summary>
    private int _id = ++_idc;

    #region Unity Lifecycle
    /// <summary>
    /// Called immediately after this <see cref="UltraCustomNightScript"/> is instantiated.
    /// </summary>
    private void Awake()
    {
        _selectable = GetComponent<KMSelectable>();
        _boss = GetComponent<KMBossModule>();
        _audio = GetComponent<KMAudio>();
        _module = GetComponent<KMBombModule>();
    }

    /// <summary>
    /// Called one frame after this <see cref="UltraCustomNightScript"/> is instantiated.
    /// </summary>
    private void Start()
    {
        
    }
    #endregion

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
        _coroutineQueue.Enqueue(routine);
    }

    /// <summary>
    /// Starts a Coroutine immediately.
    /// </summary>
    /// <param name="routine">The Coroutine to start.</param>
    public void AddCoroutineNow(IEnumerator routine)
    {
        StartCoroutine(routine);
    }

    /// <summary>
    /// Causes the module to register a strike.
    /// </summary>
    public void Strike()
    {
        _module.HandleStrike();
    }

    /// <summary>
    /// Logs a message from this module in a format in acordance with the LFA.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Any format arguments to insert.</param>
    public void Log(string message, params object[] args)
    {
        Debug.LogFormat("[Ultra Custom Night #{0}] {1}", _id, string.Format(message, args));
    }
    #endregion

    /// <summary>
    /// Called after one module has been solved. Starts all active <see cref="Animatronic"/>s.
    /// </summary>
    private void Activate()
    {
        StartCoroutine(HandleAnimatronicQueue());

        foreach(CharacterSelectable c in GetComponentsInChildren<CharacterSelectable>())
        {
            Animatronic.GetByName(c.name, this);
        }
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
                if(o.Equals(true))
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
}
