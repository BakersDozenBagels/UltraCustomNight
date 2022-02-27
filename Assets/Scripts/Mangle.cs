using System.Collections;
using UnityEngine;

class Mangle : Animatronic
{
    private float _time;
    private bool _alarmOn;

    public Mangle(UltraCustomNightScript instance) : base(instance)
    {
        if(instance.IsZenModeActive)
            return;
        instance.Log("Mangle is coming to attack! Watch out for your alarm clock.");
        instance.GameInfo.OnAlarmClockChange += s => Alarm(s);
        instance.AddCoroutineNow(Listen());
    }

    private IEnumerator Listen()
    {
        while(true)
        {
            yield return null;
            if(_alarmOn)
                _time += Time.deltaTime;
            if(_time >= Instance.MangleThreshold)
            {
                Strike();
                _time = 0f;
                Instance.Log("Strike from Mangle!");
            }
        }
    }

    private void Alarm(bool on)
    {
        _alarmOn = on;
    }
}
