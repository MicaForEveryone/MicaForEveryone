using MicaForEveryone.PInvoke;
using System.Diagnostics;

namespace MicaForEveryone.Models;

public sealed class ProcessRule : Rule
{
    public required string ProcessName { get; set; }

    public override bool Equals(Rule? other)
    {
        return other is not null 
            && other is ProcessRule pRule 
            && ProcessName.Equals(pRule.ProcessName, StringComparison.CurrentCultureIgnoreCase) 
            && base.Equals(other);
    }

    public override unsafe bool IsRuleApplicable(Windowing.HWND hWnd)
    {
        uint processId = 0;
        if (Windowing.GetWindowThreadProcessId(hWnd, &processId) == 0)
        {
            return false;
        }
        Process proc = Process.GetProcessById((int)processId);
        return proc.ProcessName.Equals(ProcessName, StringComparison.CurrentCultureIgnoreCase);
    }
}