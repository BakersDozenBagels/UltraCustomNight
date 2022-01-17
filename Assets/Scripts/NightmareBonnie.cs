using System.Collections;
using Rng = UnityEngine.Random;

class NightmareBonnie : Animatronic
{
    private int _currentState = 3;
    private bool _alarmOn;

    public NightmareBonnie(UltraCustomNightScript instance) : base(instance)
    {
        Instance.Log("Nightmare Bonnie is coming to attack! Listen close.");
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
                if(!_alarmOn)
                {
                    Instance.Log("Strike from Nightmare Bonnie!");
                    Instance.Strike();
                }
                _currentState = 3;
                break;
        }

        Instance.Log("Nightmare Bonnie knocked {0} time(s).", _currentState);
        yield return WaitFor(Rng.Range(2f, 3f));

        Instance.AddCoroutineNow(WaitToMove());
    }
}
