using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace LigerShark.Farticus
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideBindingPath()]
    [Guid(PackageGuids.guidFarticusPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(FartOptions), "Farticus", "General", 101, 101, true, new[] { "The original Visual Studio fart app" })]
    public sealed class FarticusPackage : AsyncPackage
    {
        private int _successfulBuilds;
        private bool _hasBuildFailed;

        private readonly List<string> _messages = new List<string>()
        {
            "Yes! This is my favorite sound in the whole world",
            "Excellent. Don't hold back!",
            "Great job! It smells like roses in here now",
            "Sometimes you just have to let 'em rip",
            "Ahhh, that felt right",
            "Ooops, I'm not sure about that one",
            "Wow, someone has been practicing",
            "You might want to be a little careful",
            "What a stinker. The paint is coming off the walls"
        };

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            VS.Events.BuildEvents.ProjectBuildDone += OnBuildDone;

            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService mcs)
            {
                var fartCmd = new CommandID(PackageGuids.guidFarticusCmdSet, PackageIds.cmdidRandomFart);
                var menuItem = new MenuCommand(OnFartButtonClick, fartCmd);
                mcs.AddCommand(menuItem);
            }
        }

        private void OnFartButtonClick(object sender, EventArgs e)
        {
            FartPlayer.PlayFart(Farts.RandomFart);

            var rn = new Random();
            var index = rn.Next(0, _messages.Count);

            VS.StatusBar.ShowMessageAsync(_messages[index]).FireAndForget();
        }

        private void OnBuildDone(ProjectBuildDoneEventArgs e)
        {
            JoinableTaskFactory.Run(async () =>
            {

                await JoinableTaskFactory.SwitchToMainThreadAsync();
                var options = (FartOptions)GetDialogPage(typeof(FartOptions));

                if (options.Enabled)
                {
                    if (e.IsSuccessful)
                    {
                        _successfulBuilds++;

                        if (_hasBuildFailed)
                        {
                            SetBuildMessage();
                        }
                    }
                    else
                    {
                        FartPlayer.PlayFart(options.SelectedErrorFart);
                        _hasBuildFailed = true;
                        _successfulBuilds = 0;
                    }
                }

            });
        }

        private void SetBuildMessage()
        {
            var message = string.Format(" - {0} successful build since last fart", _successfulBuilds);

            if (_successfulBuilds > 1)
            {
                // Pluralize
                message = string.Format(" - {0} successful builds since last fart", _successfulBuilds);
            }

            VS.StatusBar.ShowMessageAsync(message).FireAndForget();
        }
    }
}