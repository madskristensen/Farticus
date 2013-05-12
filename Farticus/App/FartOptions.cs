using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace LigerShark.Farticus
{
    class FartOptions : DialogPage
    {
        private bool _enabled = true;

        [LocDisplayName("On build error")]
        [Description("Select which fart to play when the build has errors")]
        [Category("Farticus")]
        public Farts SelectedErrorFart { get; set; }

        [LocDisplayName("On build warning")]
        [Description("Select which fart to play when the build has warnings")]
        [Category("Farticus")]
        public Farts SelectedWarningFart { get; set; }

        [LocDisplayName("On build message")]
        [Description("Select which fart to play when the build has messages")]
        [Category("Farticus")]
        public Farts SelectedMessageFart { get; set; }

        [LocDisplayName("Enable auto-farts")]
        [Description("When enabled, farts are played when the build fails")]
        [Category("Farticus")]
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public override void ResetSettings()
        {
            Enabled = true;
            base.ResetSettings();
        }
    }
}
