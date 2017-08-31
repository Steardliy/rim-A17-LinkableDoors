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

        private int linkingFrom = 0;
        private int linkingNum = 0;
        private CompProperties_Linkable compDef;

        public virtual bool CanLinkFromOther(int i)
        {
            return this.linkableDimension(i) && this.linkingNum <= this.compDef.linkableLimit && !LinkGroupUtility.ShouldSingleDoor(this.Pos, this.Map);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.compDef = base.props as CompProperties_Linkable;
            LinkGroupUtility.Notify_LinkableSpawned(this);

        }
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            LinkGroupUtility.Notify_LinkableDeSpawned(this);
        }
        public virtual void Notify_Linked(ILinkData other, int type)
        {
            this.linkingFrom += type;
            this.linkingNum++;
        }

        public virtual void Notify_UnLinked(ILinkData other, int type)
        {
            this.linkingFrom -= type;
            this.linkingNum--;
        }
        private bool linkableDimension(int i)
        {
            if(this.linkingFrom == 0)
            {
                return true;
            }
            return this.linkingFrom == (int)Math.Pow(2, i);
        }
    }
}
