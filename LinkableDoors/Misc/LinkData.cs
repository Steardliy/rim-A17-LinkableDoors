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

        public IEnumerable<ILinkData> Children => this.children;
        
        public bool Any()
        {
            return this.Children.Any();
        }
        public ILinkGroup Concat(ILinkGroup group)
        {
            foreach(var a in group.Children)
            {
                this.Add(a);
                a.GroupParent = this;
            }
            this.center = this.RecalculateCenter();
            return this;
        }
        private void Add(ILinkData newData)
        {
            this.children.Add(newData);
        }


        private Vector3 RecalculateCenter()
        {
            foreach (var a in this.Children)
            {
                a.Pos
            }
        }

        public LinkGroup() { }
        public LinkGroup(ILinkData newData)
        {
            this.Add(newData);
        }
        public LinkGroup(IEnumerable<ILinkData> newList)
        {
            this.children = newList.ToList();
        }
    }
}
