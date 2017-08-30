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
        private CompProperties_Linkable compDef;
        public ILinkGroup GroupParent { get; set; }
        public int linkType = 0;

        public IntVec3 Pos => base.parent.Position;
        public Vector3 DrawPos => base.parent.DrawPos;
        public Map Map => base.parent.Map;

        public bool CanLinkFromOther()
        {
            //this.compDef.linkableNumber;
            //return (!this.linkData.Any() && !this.ShouldSingleDoor());
            return true;
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
        public void Notify_Linked(ILinkData other, int linkType)
        {

        }

        public void Notify_UnLinked(ILinkData other, int linkType)
        {

        }
    }
}
