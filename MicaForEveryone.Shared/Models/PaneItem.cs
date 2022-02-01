using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using MicaForEveryone.ViewModels;

namespace MicaForEveryone.Models
{
    public enum PaneItemType
    {
        General,
        Global,
        Process,
        Class,
    }

    public interface IPaneItem
    {
        PaneItemType ItemType { get; }
    }

    public class GeneralPaneItem : IPaneItem
    {
        public GeneralPaneItem(IGeneralSettingsViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public PaneItemType ItemType { get; } = PaneItemType.General;
        public IGeneralSettingsViewModel ViewModel { get; }
    }

    public class RulePaneItem : IPaneItem
    {
        public RulePaneItem(string title, PaneItemType type, IRuleSettingsViewModel viewModel)
        {
            Title = title;
            ItemType = type;
            ViewModel = viewModel;
        }

        public string Title { get; }
        public PaneItemType ItemType { get; }
        public IRuleSettingsViewModel ViewModel { get; }
    }
}
