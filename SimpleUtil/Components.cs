using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;

namespace SimpleUtil
{
    public interface ITagFinder
    {
        ReadOnlyCollection<SnapshotSpan> FindTags2(SnapshotSpan span);
    }

    public static class SimpleUtilConstants
    {
        public const string Contract = "SimpleUtil_2.0";
    }

    [Export(SimpleUtilConstants.Contract, typeof(ITagFinder))]
    internal sealed class TagFinder : ITagFinder
    {
        private const string Target = "component a";
        private IEnumerable<SnapshotSpan> FindTags(SnapshotSpan span)
        {
            var snapshot = span.Snapshot;
            var i = 0;
            while (i < span.Length)
            {
                var position = i + span.Start.Position;
                if (IsMatch(snapshot, position))
                {
                    yield return new SnapshotSpan(snapshot, position, Target.Length);
                    i += span.Length;
                }
                else
                {
                    i += 1;
                }
            }
        }

        private bool IsMatch(ITextSnapshot snapshot, int position)
        {
            if (position + Target.Length >= snapshot.Length)
            {
                return false;
            }

            for (int i = 0; i < Target.Length; i++)
            {
                var source = snapshot[position + i];
                if (Char.ToLower(source) != Target[i])
                {
                    return false;
                }
            }

            return true;
        }

        ReadOnlyCollection<SnapshotSpan> ITagFinder.FindTags2(SnapshotSpan span)
        {
            return new ReadOnlyCollection<SnapshotSpan>(FindTags(span).ToList());
        }
    }
}
