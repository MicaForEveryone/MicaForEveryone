using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MicaForEveryone.UI.Triggers
{
    public class VisibilityTrigger : StateTriggerBase
    {
        private FrameworkElement _element;
        private Visibility _trigger;

        public FrameworkElement Target
        {
            get => _element;
            set
            {
                _element = value;
                RefreshState();
            }
        }

        public Visibility ActiveOn
        {
            get => _trigger;
            set
            {
                _trigger = value;
                RefreshState();
            }
        }

        private void RefreshState()
        {
            SetActive(Target?.Visibility == ActiveOn);
        }
    }
}
