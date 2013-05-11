using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace LigerShark.Farticus
{
    class FartOptions : DialogPage
    {
        private bool _enabled = true;

        [LocDisplayName("On build failed")]
        [Description("Select which fart to play when the build fails")]
        [Category("Farticus")]
        public Farts SelectedFart { get; set; }

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
