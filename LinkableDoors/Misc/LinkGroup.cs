using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LinkableDoors
{
    public class LinkGroup : ILinkGroup
    {
        private List<ILinkData> children = new List<ILinkData>();
        private Dictionary<PositionTag, List<ILinkData>> tagGroup = new Dictionary<PositionTag, List<ILinkData>>();

        public float GetCommonFieldSum(PositionTag tag)
        {
            float result = 0;
            foreach(var a in this.GetTagGroup(tag))
            {
                result += a.commonField;
            }
            return result;
        }
        public void SetCommonField(PositionTag tag, float value)
        {
            foreach(var a in this.GetTagGroup(tag))
            {
                a.commonField = value;
            }
        }

        public IEnumerable<ILinkData> Children => this.children;

        public IEnumerable<ILinkData> GetTagGroup(PositionTag tag)
        {
            List<ILinkData> list;
            if (this.tagGroup.TryGetValue(tag, out list))
            {
                return list;
            }
            IEnumerable<ILinkData> result = this.children.SkipWhile(a => (a.PosTag & tag) == 0).TakeWhile(a => (a.PosTag & tag) != 0);
            this.tagGroup.Add(tag, result.ToList());
            return result;
        }

        public bool Any()
        {
            return this.children.Any();
        }
        public void Split(ILinkData point)
        {
            int index = this.children.IndexOf(point);
            ILinkGroup newGroup = new LinkGroup(this.children.Skip(index + 1));
            newGroup.RecalculateCenter();
            this.children.RemoveRange(index, this.children.Count - index);
            this.RecalculateCenter();
        }
        public void Concat(ILinkGroup other)
        {
            foreach(var a in other.Children)
            {
                this.Add(a);
            }
            this.RecalculateCenter();
        }
        public void Remove(ILinkData delData)
        {
            this.children.Remove(delData);
            delData.GroupParent = null;
        }
        public void Add(ILinkData newData)
        {
            this.children.Add(newData);
            newData.GroupParent = this;
        }
        public void RecalculateCenter()
        {
            this.tagGroup.Clear();
            if (!this.Any()) { return; }

            this.children.Sort((x,y) => {
                int result = (int)(x.Pos.x - y.Pos.x);
                return result != 0 ? result : (int)(y.Pos.z - x.Pos.z);
                });

            int count = this.children.Count();
            int center = count / 2;
            int index = center;
            int mod2 = count % 2;

            for (int i = 0; i < center; i++)
            {
                this.children[i].DistFromCenter = index;
                this.children[i].PosTag = PositionTag.LeftSide;
                index--;
            }
            for (int i = center + mod2; i < count; i++)
            {
                index++;
                this.children[i].PosTag = PositionTag.RightSide;
                this.children[i].DistFromCenter = index;
            }
            if (mod2 == 0)
            {
                this.children[center - 1].PosTag |= PositionTag.LeftBorder;
                this.children[center].PosTag |= PositionTag.RightBorder;
            }
            else
            {
                this.children[center].PosTag = PositionTag.Center;
                this.children[center].DistFromCenter = 0;
            }
#if DEBUG
            Log.Message("*****RecalculateCenter(): children=" + this.children.Count);
            foreach(var a in this.children)
            {
                Log.Message("pos=" + a.DrawPos + " flag=" + a.PosTag + " dist=" + a.DistFromCenter);
            }
#endif
        }

        public LinkGroup() { }
        public LinkGroup(ILinkData newData)
        {
            this.Add(newData);
            newData.PosTag = PositionTag.Center;
        }
        public LinkGroup(IEnumerable<ILinkData> newList)
        {
            this.children = newList.ToList();
            foreach (var a in newList)
            {
                a.GroupParent = this;
            }
        }
    }
}
