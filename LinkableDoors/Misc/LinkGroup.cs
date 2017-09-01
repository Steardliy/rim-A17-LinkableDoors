using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LinkableDoors
{
    public class LinkGroup : ILinkGroup
    {
        private List<ILinkData> children = new List<ILinkData>();
        
        public IEnumerable<ILinkData> Children => this.children;
        
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
            if (!this.Any()) { return; }
           
            this.children.Sort((x,y) => {
                int result = (int)(x.Pos.x - y.Pos.x);
                return result != 0 ? result : (int)(x.Pos.z - y.Pos.z);
                });

            int count = this.children.Count();
            int center = count / 2;

            for(int i = 0; i < center; i++)
            {
                this.children[i].PosFlag = PositionFlag.LeftSide;
            }
            for (int i = center + 1; i < count; i++)
            {
                this.children[i].PosFlag = PositionFlag.RightSide;
            }
            if ((count % 2) == 0)
            {
                this.children[center - 1].PosFlag = PositionFlag.LeftBorder;
                this.children[center].PosFlag = PositionFlag.RightBorder;
            }
            else
            {
                this.children[center].PosFlag = PositionFlag.Center;
            }
        }

        public LinkGroup() { }
        public LinkGroup(ILinkData newData)
        {
            this.Add(newData);
            newData.PosFlag = PositionFlag.Center;
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
