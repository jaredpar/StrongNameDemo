using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SimpleUtil;

namespace PluginA
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(TextMarkerTag))]
    internal sealed class PluginATaggerProvider : ITaggerProvider
    {
        private readonly ITagFinder _tagFinder;

        [ImportingConstructor]
        internal PluginATaggerProvider([Import(SimpleUtilConstants.Contract)]ITagFinder tagFinder)
        {
            _tagFinder = tagFinder;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return (ITagger<T>)(object)new PluginATagger(_tagFinder);
        }
    }

    internal sealed class PluginATagger : ITagger<TextMarkerTag>
    {
        private readonly ITagFinder _tagFinder;

        internal PluginATagger(ITagFinder tagFinder)
        {
            _tagFinder = tagFinder;
        }

        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tag = new TextMarkerTag("PluginA");
            foreach (var span in spans)
            {
                foreach (var foundSpan in _tagFinder.FindTags(span))
                {
                    var tagSpan = new TagSpan<TextMarkerTag>(foundSpan, tag);
                    yield return tagSpan;
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "PluginA")]
    [Name("PluginA")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)] 
    internal sealed class PluginAFormat : ClassificationFormatDefinition
    {
        internal PluginAFormat()
        {
            this.DisplayName = "PluginA"; //human readable version of the name
            this.BackgroundColor = Colors.BlueViolet;
            this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    internal static class PluginAClassificationDefinition
    {
        /// <summary>
        /// Defines the "PluginA" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("PluginA")]
        internal static ClassificationTypeDefinition PluginAType = null;
    }
}
