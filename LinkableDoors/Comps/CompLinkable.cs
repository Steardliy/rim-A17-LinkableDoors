using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    class CompLinkable : ThingComp, ILinkData
    {
        public ILinkGroup GroupParent { get; set; }
        public PositionTag PosTag { get; set; }
        public int DistFromCenter { get; set; }
        public float commonField { get; set; }

        public IntVec3 Pos => base.parent.Position;
        public Vector3 DrawPos => base.parent.DrawPos;
        public Map Map => base.parent.Map;
        public bool IsSingle => this.directLinks.Count() == 0;
        public Rot4 LineDirection
        {
            get
            {
                int val = this.directLinks.FirstOrDefault().Value;
                return (val == 0 || val == 2) ? Rot4.East : Rot4.North;
            }
        }

        public LinkCallBack CallBack { get; set; }
        private Dictionary<ILinkData, int> directLinks = new Dictionary<ILinkData, int>();
        private CompProperties_Linkable compDef => (CompProperties_Linkable)base.props;
        
        public virtual bool CanLinkFromOther(int direction)
        {
            return this.linkableDirections(direction) && this.GroupParent.Children.Count() < this.compDef.linkableLimit && !LinkGroupUtility.ShouldSingle(this.Pos, this.Map);
        }
        public void Reset()
        {
            foreach (var a in this.directLinks)
            {
                a.Key.Notify_UnLinked(this);
            };
            this.directLinks.Clear();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            LinkGroupUtility.Notify_LinkableSpawned(this);

        }
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            LinkGroupUtility.Notify_LinkableDeSpawned(this);
        }
        public virtual void Notify_Linked(ILinkData other, int direction)
        {
            this.directLinks.Add(other, direction);
        }

        public virtual void Notify_UnLinked(ILinkData other)
        {
            this.directLinks.Remove(other);
        }
        private bool linkableDirections(int direction)
        {
            if (!this.directLinks.Any())
            {
                return true;
            }
            return this.directLinks.ContainsValue(direction);
        }
    }
}
