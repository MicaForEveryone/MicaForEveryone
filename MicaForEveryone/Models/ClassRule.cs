﻿using Microsoft.Extensions.DependencyInjection;
using XclParser.Primitives;
using XclParser.Reflection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.Models;

namespace MicaForEveryone.Models
{
    [XclType(TypeName = "Class")]
    public class ClassRule : IRule
    {
        [XclConstructor]
        public ClassRule(string className)
        {
            ClassName = className;
        }

        public string Name => $"Class({ClassName})";

        public int Priority => 0;

        [XclParameter]
        public string ClassName { get; }

        [XclField]
        public TitlebarColorMode TitleBarColor { get; set; }

        [XclField]
        public BackdropType BackdropPreference { get; set; }

        [XclField]
        public CornerPreference CornerPreference { get; set; }

        [XclField]
        public bool ExtendFrameIntoClientArea { get; set; }

        [XclField]
        public bool EnableBlurBehind { get; set; }

        public bool IsApplicable(TargetWindow target) =>
            target.ClassName == ClassName;

        public override string ToString() => Name;

        public RulePaneItem GetPaneItem(UI.ViewModels.ISettingsViewModel parent)
        {
            var viewModel = Program.CurrentApp.Container.GetService<IRuleSettingsViewModel>();
            viewModel.ParentViewModel = parent;
            viewModel.Rule = this;
            return new RulePaneItem(ClassName, PaneItemType.Class, viewModel);
        }
    }
}
