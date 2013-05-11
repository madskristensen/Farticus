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
        private static DTE2 _dte;
        private static readonly List<string> _messages = new List<string>()
        {
            "This is my favorite",
            "Don't hold back!",
            "Thanks, just what I needed",
            "Sometimes you just have to let 'em rip",
            "Ahhh, the felt right",
            "Ooops, I'm not sure about that one"
        };

        protected override void Initialize()
        {
            base.Initialize();

            _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;
            _dte.Events.BuildEvents.OnBuildProjConfigDone += OnBuildDone;

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
        
            if (null != mcs)
            {
                CommandID fartCmd = new CommandID(GuidList.guidFarticusCmdSet, (int)PkgCmdIDList.cmdidRandomFart);
                MenuCommand menuItem = new MenuCommand(OnFartButtonClick, fartCmd);
                mcs.AddCommand(menuItem);
            }
        }

        private void OnBuildDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
        {
            if (!Success)
            {
                FartOptions options = (FartOptions)GetDialogPage(typeof(FartOptions));
                FartPlayer.PlayFart(options);
            }
        }

        private void OnFartButtonClick(object sender, EventArgs e)
        {
            FartPlayer.PlayRandomFart();

            Random rn = new Random(DateTime.Now.Millisecond);
            int index = rn.Next(0, _messages.Count);

            _dte.StatusBar.Text = _messages[index];
        }
    }
}