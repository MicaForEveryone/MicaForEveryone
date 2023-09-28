using MicaForEveryone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicaForEveryone.App.Helpers;

public static class RuleInfoHelper
{
    public static string GetIcon(Rule rule)
    {
        if (rule is GlobalRule)
            return "\uED35";
        if (rule is ProcessRule)
            return "\uECAA";
        if (rule is ClassRule)
            return "\uE737";
        throw new ArgumentException("Invalid rule type.", nameof(rule));
    }

    public static string GetRuleName(Rule rule)
    {
        if (rule is GlobalRule)
            return "_Global";
        if (rule is ProcessRule processRule)
            return processRule.ProcessName;
        if (rule is ClassRule classRule)
            return classRule.ClassName;
        throw new ArgumentException("Invalid rule type.", nameof(rule));
    }
}
