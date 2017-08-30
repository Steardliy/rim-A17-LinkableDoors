using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace LinkableDoors
{
    class LinkGroupManager : MapComponent
    {
        public LinkGroupManager(Map map) : base(map) {}

        private List<ILinkGroup> linkedGroup;

        public void Notify_LinkableSpawned(ILinkData newObj)
        {
            ILinkGroup newGroup = this.CheckAround(newObj);
            this.GroupRegister(newGroup);
        }
        public void Notify_LinkableDeSpawned(ILinkData delObj)
        {

        }
        private void GroupRegister(ILinkGroup newGropu)
        {
            this.linkedGroup.Add(newGropu);
        }
        private void GroupUnRegister(ILinkGroup delGropu)
        {
            this.linkedGroup.Remove(delGropu);
        }

        private ILinkGroup CheckAround(ILinkData current)
        {
            ILinkGroup result = new LinkGroup(current);
            current.GroupParent = result;
            int invert = -1;
            for (int i = 0; i < 4; i++)
            {
                IntVec3 pos = current.Pos + GenAdj.CardinalDirections[i];
                Building_LinkableDoor door = base.map.thingGrid.ThingAt<Building_LinkableDoor>(pos);
                CompLinkable comp = door?.TryGetComp<CompLinkable>();
                if (comp != null && comp.CanLinkFromOther() && (invert == -1 || invert == i))
                {
                    invert = ((i + 2) % 4);
                    result = comp.GroupParent.Concat(current.GroupParent);
                    current.Notify_Linked(comp, (int)Math.Pow(2, invert));
                    comp.Notify_Linked(current, (int)Math.Pow(2, i));
                }
            }
            return result;
        }
    }
}
