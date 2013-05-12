using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace LigerShark.Farticus
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidFarticusPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideOptionPage(typeof(FartOptions), "Farticus", "General", 101, 101, true, new[] { "The original Visual Studio fart app" })]
    public sealed class FarticusPackage : ExtensionPointPackage
    {
        private DTE2 _dte;
        private BuildEvents _events;

        private readonly List<string> _messages = new List<string>()
        {
            "Yes! This is my favorite type of fart",
            "Excellent. Don't hold back!",
            "Great job! It smells like roses in here now",
            "Sometimes you just have to let 'em rip",
            "Ahhh, that felt right",
            "Ooops, I'm not sure about that one",
            "Wow, someone has been practicing",
            "You might want to be a little careful",
            "What a stinker. The paint is coming off the walls"
        };
        private int NumSuccessfulBuilds = 0;

        protected override void Initialize()
        {
            base.Initialize();

            _dte = GetService(typeof(DTE)) as DTE2;
            _events = _dte.Events.BuildEvents;
            _events.OnBuildProjConfigDone += OnProjBuildDone;
            _events.OnBuildDone += OnBuildDone;
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (null != mcs)
            {
                CommandID fartCmd = new CommandID(GuidList.guidFarticusCmdSet, (int)PkgCmdIDList.cmdidRandomFart);
                MenuCommand menuItem = new MenuCommand(OnFartButtonClick, fartCmd);
                mcs.AddCommand(menuItem);
            }
        }

        private void OnProjBuildDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
        {
            FartOptions options = (FartOptions)GetDialogPage(typeof(FartOptions));

            if (options.Enabled)
            {
                bool hasWarnings = _dte.ToolWindows.ErrorList.ErrorItems.Count > 0;

                if (!Success)
                    FartPlayer.PlayFart(options.SelectedErrorFart);

                else if (Success && hasWarnings)
                    FartPlayer.PlayFart(options.SelectedWarningFart);
            }
        }

        void OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            if (_dte.Solution.SolutionBuild.LastBuildInfo == 0)
            {
                // last build was a success
                NumSuccessfulBuilds++;
                if (NumSuccessfulBuilds > 1) // don't show the msg for the first successful build
                {
                    _dte.StatusBar.Text += string.Format(" - {0} successful builds since last fart.", NumSuccessfulBuilds - 1);
                }
            }
            else
            {
                _dte.StatusBar.Text += " - you just farted";
                NumSuccessfulBuilds = 0;                
            }
        }
      
        private void OnFartButtonClick(object sender, EventArgs e)
        {
            FartPlayer.PlayFart(Farts.RandomFart);

            Random rn = new Random();
            int index = rn.Next(0, _messages.Count);

            _dte.StatusBar.Text = _messages[index];
        }
    }
}