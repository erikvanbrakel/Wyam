﻿using System;
using Wyam.Common.Configuration;
using Wyam.Common.Execution;
using Wyam.Common.Modules;
using Wyam.Core.Modules.Control;
using Wyam.Core.Modules.Extensibility;
using Wyam.Core.Modules.IO;
using Wyam.Html;

namespace Wyam.Blog.Pipelines
{
    /// <summary>
    /// Renders blog post pages. This needs to come after the tags
    /// pipeline so that the listing of tags on each blog post page
    /// will have the correct counts.
    /// </summary>
    public class Posts : Pipeline
    {
        /// <summary>
        /// Renders the posts from the <see cref="RawPosts"/> pipeline.
        /// </summary>
        public const string Render = nameof(Render);

        /// <summary>
        /// Gets excerpts for each document.
        /// </summary>
        public const string Excerpts = nameof(Excerpts);

        /// <summary>
        /// Writes the documents to the file system.
        /// </summary>
        public const string WriteFiles = nameof(WriteFiles);

        /// <summary>
        /// Orders the posts by their published date. We need to do this
        /// again since the order would have gotten messed up by the concurrent Razor rendering.
        /// </summary>
        public const string OrderByPublished = nameof(OrderByPublished);

        internal Posts()
            : base(GetModules())
        {
        }

        private static IModuleList GetModules() => new ModuleList
        {
            {
                Render,
                new ModuleCollection
                {
                    new Documents(Blog.RawPosts),
                    new Razor.Razor()
                        .WithLayout("/_PostLayout.cshtml")
                }
            },
            {
                Excerpts,
                new ModuleCollection
                {
                    new Excerpt()
                        .WithMetadataKey(BlogKeys.Excerpt),
                    new Excerpt("div#content")
                        .WithMetadataKey(BlogKeys.Content)
                        .WithOuterHtml(false)
                }
            },
            {
                WriteFiles,
                new WriteFiles(".html")
            },
            {
                OrderByPublished,
                new OrderBy((doc, ctx) => doc.Get<DateTime>(BlogKeys.Published)).Descending()
            }
        };
    }
}