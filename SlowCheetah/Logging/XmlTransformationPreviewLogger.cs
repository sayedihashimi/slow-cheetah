// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah
{
    using System;
    using Microsoft.Web.XmlTransform;

    /// <summary>
    /// Shim for using an inernal <see cref="ITransformationLogger"/> in <see cref="XmlTransformer"/>
    /// </summary>
    public class XmlTransformationPreviewLogger : IXmlTransformationLogger
    {
        private readonly ITransformationLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlTransformationPreviewLogger"/> class.
        /// </summary>
        /// <param name="logger">The external logger</param>
        public XmlTransformationPreviewLogger(ITransformationLogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.logger = logger;
        }

        /// <inheritdoc/>
        public void LogError(string message, params object[] messageArgs)
        {
            this.logger.LogError(message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogError(string file, string message, params object[] messageArgs)
        {
            this.logger.LogError(file, 0, 0, message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            this.logger.LogError(file, lineNumber, linePosition, message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex)
        {
            this.logger.LogErrorFromException(ex);
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex, string file)
        {
            this.logger.LogErrorFromException(ex);
        }

        /// <inheritdoc/>
        public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
        {
            this.logger.LogErrorFromException(ex);
        }

        /// <inheritdoc/>
        public void LogMessage(string message, params object[] messageArgs)
        {
            this.logger.LogMessage(message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogMessage(MessageType type, string message, params object[] messageArgs)
        {
            this.logger.LogMessage(message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogWarning(string message, params object[] messageArgs)
        {
            this.logger.LogWarning(message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogWarning(string file, string message, params object[] messageArgs)
        {
            this.logger.LogWarning(file, 0, 0, message, messageArgs);
        }

        /// <inheritdoc/>
        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            this.logger.LogWarning(file, lineNumber, linePosition, message, messageArgs);
        }

        /// <inheritdoc/>
        public void StartSection(string message, params object[] messageArgs)
        {
            // this.LogMessage(message, messageArgs);
        }

        /// <inheritdoc/>
        public void StartSection(MessageType type, string message, params object[] messageArgs)
        {
            // this.LogMessage(message, messageArgs);
        }

        /// <inheritdoc/>
        public void EndSection(string message, params object[] messageArgs)
        {
            // this.LogMessage(message, messageArgs);
        }

        /// <inheritdoc/>
        public void EndSection(MessageType type, string message, params object[] messageArgs)
        {
            // this.LogMessage(message, messageArgs);
        }
    }
}
