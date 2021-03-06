﻿using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;

namespace LigerShark.Farticus
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidFarticusPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(FartOptions), "Farticus", "General", 101, 101, true, new[] { "The original Visual Studio fart app" })]
    public sealed class FarticusPackage : AsyncPackage
    {
        private DTE2 _dte;
        private BuildEvents _events;
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

            _dte = await GetServiceAsync(typeof(DTE)) as DTE2;
            Assumes.Present(_dte);

            _events = _dte.Events.BuildEvents;
            _events.OnBuildDone += OnBuildDone;

            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService mcs)
            {
                CommandID fartCmd = new CommandID(PackageGuids.guidFarticusCmdSet, PackageIds.cmdidRandomFart);
                MenuCommand menuItem = new MenuCommand(OnFartButtonClick, fartCmd);
                mcs.AddCommand(menuItem);
            }
        }

        private void OnFartButtonClick(object sender, EventArgs e)
        {
            FartPlayer.PlayFart(Farts.RandomFart);

            Random rn = new Random();
            int index = rn.Next(0, _messages.Count);

            _dte.StatusBar.Text = _messages[index];
        }

        private void OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            JoinableTaskFactory.Run(async () =>
            {

                await JoinableTaskFactory.SwitchToMainThreadAsync();
                FartOptions options = (FartOptions)GetDialogPage(typeof(FartOptions));

                if (options.Enabled)
                {
                    bool isSuccess = _dte.Solution.SolutionBuild.LastBuildInfo == 0;
                    Fart(isSuccess, options);

                    if (isSuccess)
                    {
                        _successfulBuilds++;

                        if (_hasBuildFailed)
                            SetBuildMessage();
                    }
                    else
                    {
                        _hasBuildFailed = true;
                        _successfulBuilds = 0;
                    }
                }

            });
        }

        private void Fart(bool isSuccess, FartOptions options)
        {
            bool hasWarnings = _dte.ToolWindows.ErrorList.ErrorItems.Count > 0;

            if (!isSuccess)
                FartPlayer.PlayFart(options.SelectedErrorFart);

            else if (isSuccess && hasWarnings)
                FartPlayer.PlayFart(options.SelectedWarningFart);
        }

        private void SetBuildMessage()
        {
            string message = string.Format(" - {0} successful build since last fart", _successfulBuilds);

            if (_successfulBuilds > 1)
            {
                // Pluralize
                message = string.Format(" - {0} successful builds since last fart", _successfulBuilds);
            }

            _dte.StatusBar.Text += message;
        }
    }
}