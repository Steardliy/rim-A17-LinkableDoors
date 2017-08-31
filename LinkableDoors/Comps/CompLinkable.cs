using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    class CompLinkable : ThingComp, ILinkData
    {
        public ILinkGroup GroupParent { get; set; }
        public IntVec3 Pos => base.parent.Position;
        public Vector3 DrawPos => base.parent.DrawPos;
        public Map Map => base.parent.Map;
        public int LinkingFrom => this.linkDirectionsFrom;
        public int DirectLinkCount => this.directLinking.Count();

        private int linkDirectionsFrom = 0;
        private Dictionary<ILinkData, int> directLinking = new Dictionary<ILinkData, int>();
        private CompProperties_Linkable compDef => (CompProperties_Linkable)base.props;

        public virtual bool CanLinkFromOther(int direction)
        {
            return this.linkableDirections(direction) && this.GroupParent.Children.Count() <= this.compDef.linkableLimit && !LinkGroupUtility.ShouldSingleDoor(this.Pos, this.Map);
        }

        public void Reset()
        {
            foreach (var a in this.directLinking)
            {
                int invert = (a.Value + 2) % 4;
                a.Key.Notify_UnLinked(this, invert);
            }
            this.GroupParent = null;
            this.linkDirectionsFrom = 0;
            this.directLinking.Clear();
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
            this.linkDirectionsFrom += (int)Math.Pow(2, direction);
            this.directLinking.Add(other, direction);
        }

        public virtual void Notify_UnLinked(ILinkData other, int direction)
        {
            this.linkDirectionsFrom -= (int)Math.Pow(2, direction);
            this.directLinking.Remove(other);
        }
        private bool linkableDirections(int direction)
        {
            Log.Message("direc:" + direction + " linkDirectionsFrom:" + linkDirectionsFrom);
            if(this.linkDirectionsFrom == 0)
            {
                return true;
            }
            return this.linkDirectionsFrom == (int)Math.Pow(2, direction);
        }
    }
}
