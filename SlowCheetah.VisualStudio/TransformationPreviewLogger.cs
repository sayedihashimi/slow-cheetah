// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah.VisualStudio
{
    using System;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using SlowCheetah;

    /// <summary>
    /// Logger for XDT transformation on Preview Transform
    /// </summary>
    public class TransformationPreviewLogger : ITransformationLogger
    {
        private ErrorListProvider errorListProvider;
        private IVsHierarchy hierachy;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationPreviewLogger"/> class.
        /// </summary>
        /// <param name="errorListProvider">The VS Package</param>
        /// <param name="hierachy">The current project hierarchy</param>
        public TransformationPreviewLogger(ErrorListProvider errorListProvider, IVsHierarchy hierachy)
        {
            if (errorListProvider == null)
            {
                throw new ArgumentNullException(nameof(errorListProvider));
            }

            if (hierachy == null)
            {
                throw new ArgumentNullException(nameof(hierachy));
            }

            this.errorListProvider = errorListProvider;
            this.hierachy = hierachy;
        }

        /// <inheritdoc/>
        public void LogError(string message, params object[] messageArgs)
        {
            this.AddError(TaskErrorCategory.Error, string.Format(message, messageArgs), null, 0, 0);
        }

        /// <inheritdoc/>
        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            this.AddError(TaskErrorCategory.Error, string.Format(message, messageArgs), file, lineNumber, linePosition);
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex)
        {
            this.AddError(ex, TaskErrorCategory.Error, null, 0, 0);
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
        {
            this.AddError(ex, TaskErrorCategory.Error, file, lineNumber, linePosition);
        }

        /// <inheritdoc/>
        public void LogMessage(string message, params object[] messageArgs)
        {
            this.AddError(TaskErrorCategory.Message, string.Format(message, messageArgs), null, 0, 0);
        }

        /// <inheritdoc/>
        public void LogWarning(string message, params object[] messageArgs)
        {
            this.AddError(TaskErrorCategory.Warning, string.Format(message, messageArgs), null, 0, 0);
        }

        /// <inheritdoc/>
        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            this.AddError(TaskErrorCategory.Warning, string.Format(message, messageArgs), file, lineNumber, linePosition);
        }

        private void AddError(TaskErrorCategory errorCategory, string text, string file, int lineNumber, int linePosition)
        {
            this.ShowError(new ErrorTask(), errorCategory, text, file, lineNumber, linePosition);
        }

        private void AddError(Exception ex, TaskErrorCategory errorCategory, string file, int lineNumber, int linePosition)
        {
            this.ShowError(new ErrorTask(ex), errorCategory, null, file, lineNumber, linePosition);
        }

        private void ShowError(ErrorTask newError, TaskErrorCategory errorCategory, string text, string file, int lineNumber, int linePosition)
        {
            newError.Category = TaskCategory.Misc;
            newError.ErrorCategory = errorCategory;
            if (file != null)
            {
                newError.Text = text;
            }

            if (file != null)
            {
                newError.Document = file;
            }

            if (lineNumber > 0)
            {
                newError.Line = lineNumber;
            }

            if (linePosition > 0)
            {
                newError.Column = linePosition;
            }

            newError.Navigate += (sender, e) =>
            {
                this.errorListProvider.Navigate(newError, new Guid(EnvDTE.Constants.vsViewKindCode));
            };

            this.errorListProvider.Tasks.Add(newError);
            this.errorListProvider.Show();
        }
    }
}
