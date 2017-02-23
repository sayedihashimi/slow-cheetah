﻿// Copyright (c) Sayed Ibrahim Hashimi. All rights reserved.
// Licensed under the Apache License, Version 2.0. See  License.md file in the project root for full license information.

namespace SlowCheetah
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using Microsoft.Web.XmlTransform;
    using SlowCheetah.Exceptions;

    /// <summary>
    /// Transforms XML files utilizing Microsoft Web XmlTransform library
    /// </summary>
    public class XmlTransformer : ITransformer
    {
        private IXmlTransformationLogger logger = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlTransformer"/> class.
        /// </summary>
        public XmlTransformer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlTransformer"/> class with an external logger.
        /// </summary>
        /// <param name="logger">External logger. Passed into the transformation</param>
        /// <param name="useSections">Wheter or not to use sections while logging</param>
        public XmlTransformer(ITransformationLogger logger, bool useSections)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.logger = new XmlShimLogger(logger, useSections);
        }

        /// <inheritdoc/>
        public bool Transform(string source, string transform, string destination)
        {
            // Parameter validation
            Contract.Requires(!string.IsNullOrWhiteSpace(source));
            Contract.Requires(!string.IsNullOrWhiteSpace(transform));
            Contract.Requires(!string.IsNullOrWhiteSpace(destination));

            // File validation
            if (!File.Exists(source))
            {
                throw new FileNotFoundException("File to transform not found", source);
            }

            if (!File.Exists(transform))
            {
                throw new FileNotFoundException("Transform file not found", transform);
            }

            using (XmlTransformableDocument document = new XmlTransformableDocument())
            using (XmlTransformation transformation = new XmlTransformation(transform, this.logger))
            {
                document.PreserveWhitespace = true;
                document.Load(source);

                var success = transformation.Apply(document);
                if (success)
                {
                    document.Save(destination);
                }

                return success;
            }
        }
    }
}
