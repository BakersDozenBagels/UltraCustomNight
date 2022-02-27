using System.Collections;
using Rng = UnityEngine.Random;

class Bonnie : Animatronic, ITP
{
    private int _currentState = 3;
    private bool _alarmOn;

    public Bonnie(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Bonnie is coming to attack! Listen close.");
        Instance.AddCoroutineNow(WaitToMove());
        instance.GameInfo.OnAlarmClockChange += s => Alarm(s);
    }

    private void Alarm(bool on)
    {
        _alarmOn |= on;
    }

    private IEnumerator WaitToMove()
    {
        yield return WaitFor(Rng.Range(10f, 25f));
        Instance.AddCoroutineToQueue(Move());
    }

    private IEnumerator Move()
    {
        switch(_currentState)
        {
            case 3:
                Instance.PlaySound(Constants.SOUND_NIGHTMARE_BONNIE_BANG1);
                _currentState = 1;
                break;
            case 1:
                Instance.PlaySound(Constants.SOUND_NIGHTMARE_BONNIE_BANG2);
                _currentState = 2;
                break;
            case 2:
                Instance.PlaySound(Constants.SOUND_NIGHTMARE_BONNIE_BANG3);
                _alarmOn = false;
                yield return WaitFor(15f);
                if(!_alarmOn && !ForcedSolve)
                {
                    Instance.Log("Strike from Bonnie!");
                    Strike();
                }
                _currentState = 3;
                break;
        }

        Instance.Log("Bonnie knocked {0} time(s).", _currentState);
        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }

    public IEnumerable HandleTwitchCommand(string command)
    {
        yield break;
    }

    public IEnumerable HandleTwitchForcedSolve()
    {
        ForcedSolve = true;
        yield break;
    }
}
