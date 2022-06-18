using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class UltraCustomNightScript
{
#pragma warning disable 414
    private const string TwitchHelpMessage = @"Use ""!{0} select 1 2"" to select the animatronic at row 1, column 2. Use ""!{0} cam 9"" to navigate to vent 9. Use ""!{0} door f"" to close the front door. Use ""!{0} vent l"" to seal the left vent.";
#pragma warning restore 414

    private List<ITP> _TPHandlers = new List<ITP>();
    private bool _canStrike = true, _started;

    public void CloseDoor(DoorPosition pos)
    {
        if(pos == DoorPosition.Front || pos == DoorPosition.Left || pos == DoorPosition.Right)
            _camSel.OnInteract();
        else
            _ventSel.OnInteract();
        GetComponentsInChildren<DoorSelectable>().First(s => s.Id == pos).OnInteract();
    }

    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();

        foreach(ITP tp in _TPHandlers)
            foreach(object o in tp.HandleTwitchCommand(command))
                yield return o;

        Match m;
        if(_started)
        {
            if((m = Regex.Match(command, @"^cam\s+([1-9]|1[01])$")).Success)
            {
                yield return null;
                int cam = int.Parse(m.Groups[1].Value);
                if(cam > 7)
                    _ventSel.OnInteract();
                else
                    _camSel.OnInteract();
                yield return new WaitForSeconds(0.1f);
                GetComponentsInChildren<CameraSelectable>().First(s => s.Id == cam).OnInteract();
                yield break;
            }
            if((m = Regex.Match(command, @"^door\s+([flr])$")).Success)
            {
                yield return null;
                _camSel.OnInteract();
                yield return new WaitForSeconds(0.1f);
                DoorPosition d;
                switch(m.Groups[1].Value)
                {
                    case "f":
                        d = DoorPosition.Front;
                        break;
                    case "l":
                        d = DoorPosition.Left;
                        break;
                    case "r":
                        d = DoorPosition.Right;
                        break;
                    default:
                        throw new Exception("bad regex");
                }
                CloseDoor(d);
                yield break;
            }
            if((m = Regex.Match(command, @"^vent\s+([flr])$")).Success)
            {
                yield return null;
                _camSel.OnInteract();
                yield return new WaitForSeconds(0.1f);
                DoorPosition d;
                switch(m.Groups[1].Value)
                {
                    case "f":
                        d = DoorPosition.VFront;
                        break;
                    case "l":
                        d = DoorPosition.VLeft;
                        break;
                    case "r":
                        d = DoorPosition.VRight;
                        break;
                    default:
                        throw new Exception("bad regex");
                }
                CloseDoor(d);
                yield break;
            }
        }
        else
        {
            if((m = Regex.Match(command, @"^select\s+([1-5])\s+([1-4])$")).Success)
            {
                yield return null;
                _setupScreen.transform.GetChild(0).GetChild(int.Parse(m.Groups[1].Value) - 1).GetChild(int.Parse(m.Groups[2].Value) - 1).GetComponent<KMSelectable>().OnInteract();
                yield break;
            }
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        Log("Module forced solved!");
        _canStrike = false;
        TimeAdjust /= 3f;
        StartCoroutine(Notify());
        while(!_isSolved)
            yield return true;
        yield break;
    }

    private IEnumerator Notify()
    {
        while(!_started)
            yield return true;
        foreach(ITP tp in _TPHandlers)
            foreach(object o in tp.HandleTwitchForcedSolve())
                yield return o;
        while(!_isSolved)
        {
            GetComponentsInChildren<CameraSelectable>().Where(s => s.gameObject.activeInHierarchy).PickRandom().OnInteract();
            yield return new WaitForSeconds(UnityEngine.Random.Range(6f, 15f) * TimeAdjust);
        }
    }
}
