﻿// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.VisualStudio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using NuGet.VisualStudio;
    using SlowCheetah.VisualStudio.Properties;
    using TPL = System.Threading.Tasks;

    /// <summary>
    /// Manages installations of the SlowCheetah NuGet package in the project
    /// </summary>
    public class SlowCheetahNuGetManager
    {
        private static readonly string PackageName = Settings.Default.SlowCheetahNugetPkgName;
        private static readonly Version LastUnsupportedVersion = new Version(2, 5, 15);

        private readonly IServiceProvider package;

        private readonly HashSet<string> installTasks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly object syncObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SlowCheetahNuGetManager"/> class.
        /// </summary>
        /// <param name="package">VS Package</param>
        public SlowCheetahNuGetManager(IServiceProvider package)
        {
            this.package = package;
        }

        /// <summary>
        /// Checks the SlowCheetah NuGet package on current project.
        /// If no version is installed, prompts for install of latest version;
        /// if an older version is detected, shows update information.
        /// </summary>
        /// <param name="hierarchy">Hierarchy of the project to be verified</param>
        public void CheckSlowCheetahInstallation(IVsHierarchy hierarchy)
        {
            if (hierarchy == null)
            {
                throw new ArgumentNullException(nameof(hierarchy));
            }

            Project currentProject = PackageUtilities.GetAutomationFromHierarchy<Project>(hierarchy, (uint)VSConstants.VSITEMID.Root);

            // Whether or not an even older version of SlowCheetah (before NuGet) is installed
            bool isOldScInstalled = IsOldSlowCheetahInstalled(hierarchy as IVsBuildPropertyStorage);

            // If an old version is installed or the nuget package is installed but not updated, show update information
            if (isOldScInstalled || (this.IsSlowCheetahInstalled(currentProject) && !this.IsSlowCheetahUpdated(currentProject)))
            {
                INugetPackageHandler nugetHandler = NugetHandlerFactory.GetHandler(this.package);
                nugetHandler.ShowUpdateInfo();
            }
            else
            {
                this.BackgroundInstallSlowCheetah(currentProject);
            }
        }

        private static void InstallSlowCheetahPackage(IVsPackageInstaller2 installer, Project project)
        {
            installer.InstallLatestPackage(null, project, PackageName, false, false);
        }

        private static IVsPackageInstallerServices GetInstallerServices(IServiceProvider package)
        {
            var componentModel = (IComponentModel)package.GetService(typeof(SComponentModel));
            IVsPackageInstallerServices installerServices = componentModel.GetService<IVsPackageInstallerServices>();
            return installerServices;
        }

        private static bool IsOldSlowCheetahInstalled(IVsBuildPropertyStorage buildPropertyStorage)
        {
            string propertyValue;
            buildPropertyStorage.GetPropertyValue("SlowCheetahImport", null, (uint)_PersistStorageType.PST_PROJECT_FILE, out propertyValue);
            if (!string.IsNullOrEmpty(propertyValue))
            {
                return true;
            }

            buildPropertyStorage.GetPropertyValue("SlowCheetahTargets", null, (uint)_PersistStorageType.PST_PROJECT_FILE, out propertyValue);
            if (!string.IsNullOrEmpty(propertyValue))
            {
                return true;
            }

            return false;
        }

        private bool IsSlowCheetahInstalled(Project project)
        {
            IVsPackageInstallerServices installerServices = GetInstallerServices(this.package);
            return installerServices.IsPackageInstalled(project, PackageName);
        }

        private bool IsSlowCheetahUpdated(Project project)
        {
            // Checks for older SC versions that require more complex update procedure
            IVsPackageInstallerServices installerServices = GetInstallerServices(this.package);
            IVsPackageMetadata scPackage =
                    installerServices.GetInstalledPackages().First(pkg => string.Equals(pkg.Id, PackageName, StringComparison.OrdinalIgnoreCase));
            if (scPackage != null)
            {
                Version ver;
                if (Version.TryParse(scPackage.VersionString, out ver))
                {
                    return ver > LastUnsupportedVersion;
                }
            }

            return false;
        }

        private void BackgroundInstallSlowCheetah(Project project)
        {
            string projName = project.UniqueName;
            bool needInstall = true;
            lock (this.syncObject)
            {
                needInstall = this.installTasks.Add(projName);
            }

            if (needInstall)
            {
                if (this.HasUserAcceptedWarningMessage())
                {
                    // Gets the general output pane to inform user of installation
                    IVsOutputWindowPane outputWindow = (IVsOutputWindowPane)this.package.GetService(typeof(SVsGeneralOutputWindowPane));
                    if (outputWindow != null)
                    {
                        outputWindow.OutputString(string.Format(Resources.Resources.NugetInstall_InstallingOutput, project.Name));
                    }

                    // Installs the latest version of the SlowCheetah NuGet package
                    var componentModel = (IComponentModel)this.package.GetService(typeof(SComponentModel));
                    IVsPackageInstaller2 packageInstaller = componentModel.GetService<IVsPackageInstaller2>();
                    TPL.Task.Run(() =>
                    {
                        string message = Resources.Resources.NugetInstall_FinishedOutput;
                        try
                        {
                            InstallSlowCheetahPackage(packageInstaller, project);
                        }
                        catch
                        {
                            message = Resources.Resources.NugetInstall_ErrorOutput;
                            throw;
                        }
                        finally
                        {
                            lock (this.syncObject)
                            {
                                this.installTasks.Remove(projName);
                            }

                            ThreadHelper.Generic.BeginInvoke(() => outputWindow?.OutputString(string.Format(message, project.Name)));
                        }
                    });
                }
            }
        }

        private bool HasUserAcceptedWarningMessage()
        {
            IVsUIShell shell = this.package.GetService(typeof(SVsUIShell)) as IVsUIShell;
            if (shell != null)
            {
                // Show a message box requesting the install of the SC package
                string title = Resources.Resources.NugetInstall_Title;
                string message = Resources.Resources.NugetInstall_Text;
                Guid compClass = Guid.Empty;
                int result;
                if (ErrorHandler.Succeeded(shell.ShowMessageBox(0, ref compClass, title, message, null, 0, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_WARNING, 1, out result)))
                {
                    return result == (int)VSConstants.MessageBoxResult.IDYES;
                }
            }

            return false;
        }
    }
}
