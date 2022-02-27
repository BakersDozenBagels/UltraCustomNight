using System.Collections;

public interface ITP
{
    IEnumerable HandleTwitchCommand(string command);
    IEnumerable HandleTwitchForcedSolve();
}