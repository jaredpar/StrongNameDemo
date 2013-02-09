using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SimpleUtil;

namespace PluginB
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(TextMarkerTag))]
    internal sealed class PluginBTaggerProvider : ITaggerProvider
    {
        private readonly ITagFinder _tagFinder;

        [ImportingConstructor]
        internal PluginBTaggerProvider([Import(SimpleUtilConstants.Contract)]ITagFinder tagFinder)
        {
            _tagFinder = tagFinder;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return (ITagger<T>)(object)new PluginBTagger(_tagFinder);
        }
    }

    internal sealed class PluginBTagger : ITagger<TextMarkerTag>
    {
        private readonly ITagFinder _tagFinder;

        internal PluginBTagger(ITagFinder tagFinder)
        {
            _tagFinder = tagFinder;
        }

        public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tag = new TextMarkerTag("PluginB");
            foreach (var span in spans)
            {
                foreach (var foundSpan in _tagFinder.FindTags2(span))
                {
                    var tagSpan = new TagSpan<TextMarkerTag>(foundSpan, tag);
                    yield return tagSpan;
                }
            }
        }

#pragma warning disable 67
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning disable 67
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "PluginB")]
    [Name("PluginB")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    internal sealed class PluginBFormat : ClassificationFormatDefinition
    {
        internal PluginBFormat()
        {
            this.DisplayName = "PluginB"; //human readable version of the name
            this.BackgroundColor = Colors.Green;
            this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    internal static class PluginBClassificationDefinition
    {
        /// <summary>
        /// Defines the "PluginB" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("PluginB")]
        internal static ClassificationTypeDefinition PluginBType = null;
    }
}
