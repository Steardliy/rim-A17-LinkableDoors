using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    public class LinkGroup : ILinkGroup
    {
        private Vector3 center = default(Vector3);
        private List<ILinkData> children = new List<ILinkData>();

        public Vector3 Center => this.center;
        public IEnumerable<ILinkData> Children => this.children;
        
        public bool Any()
        {
            return this.children.Any();
        }
        public void Split(ILinkData point)
        {
            int index = this.children.IndexOf(point);
            Log.Message("index:" + index + " point:"+ point.DrawPos.ToString() + " thisc:" + this.Children.Count());
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
            Log.Message("childrenCount:" + this.Children.Count());
            if (!this.Any()) { return; }
            this.children.Sort((x,y) => {
                int result = (int)(x.Pos.x - y.Pos.x);
                return result != 0 ? result : (int)(x.Pos.z - y.Pos.z);
                });
            Vector3 min = this.children.First().DrawPos;
            Vector3 max = this.children.Last().DrawPos;
            Log.Message("min:" + min + " max:" + max);
            this.center =  (min + max) / 2;
        }

        public LinkGroup() { }
        public LinkGroup(ILinkData newData)
        {
            this.Add(newData);
            this.center = newData.DrawPos;
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
