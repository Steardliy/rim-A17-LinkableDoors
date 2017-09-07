﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LinkableDoors
{
    static class LinkGroupUtility
    {
        public static void Notify_LinkableSpawned(ILinkData newObj)
        {
            ILinkGroup result = new LinkGroup(newObj);
            LinkGroupUtility.AttachGroupAround(newObj);
        }
        public static void Notify_LinkableDeSpawned(ILinkData delObj)
        {
            LinkGroupUtility.DetachGroupAround(delObj);
        }

        public static int Invert(int i) { return (i + 2) % 4; }

        public static void DetachGroupAround(ILinkData delObj)
        {
            ILinkGroup parent = delObj.GroupParent;
            if(parent.Children.Count() <= 1)
            {
                return;
            }
            if (parent.Children.First() == delObj || parent.Children.Last() == delObj)
            {
                parent.Remove(delObj);
                parent.RecalculateCenter();
            }
            else
            {
                parent.Split(delObj);
            }
            delObj.Reset();
            ILinkGroup result = new LinkGroup(delObj);
        }

        public static void AttachGroupAround(ILinkData newObj)
        {
            LinkGroupUtility.CheckAround(newObj, (i, current) =>
            {
                int invert = LinkGroupUtility.Invert(i);
                if (current.CanLinkFromOther(i) && newObj.CanLinkFromOther(invert))
                {
                    current.GroupParent.Concat(newObj.GroupParent);
                    newObj.Notify_Linked(current, i);
                    current.Notify_Linked(newObj, invert);
                }
            });
        }
        public static bool ShouldSingle(IntVec3 pos, Map map)
        {
            int num = LinkGroupUtility.AlignQualityAgainst(pos + IntVec3.East, map);
            num += LinkGroupUtility.AlignQualityAgainst(pos + IntVec3.West, map);
            if (num == 2)
            {
                return true;
            }

            num = LinkGroupUtility.AlignQualityAgainst(pos + IntVec3.North, map);
            num += LinkGroupUtility.AlignQualityAgainst(pos + IntVec3.South, map);
            if (num == 2)
            {
                return true;
            }
            return false;
        }

        delegate void Func(int i, ILinkData current);
        private static void CheckAround(ILinkData newObj, Func func)
        {
            for (int i = 0; i < 4; i++)
            {
                IntVec3 pos = newObj.Pos + GenAdj.CardinalDirections[i];
                foreach (var thing in newObj.Map.thingGrid.ThingsListAtFast(pos))
                {
                    Building_LinkableDoor door = thing as Building_LinkableDoor;
                    ILinkData current = door?.TryGetComp<CompLinkable>();
                    if (current != null)
                    {
                        func(i, current);
                    }
                }
            }
        }

        private static int AlignQualityAgainst(IntVec3 c, Map map)
        {
            if (!c.InBounds(map))
            {
                return 0;
            }
            if (!c.Walkable(map))
            {
                return 1;
            }
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i] as Blueprint;
                if (thing != null)
                {
                    if (thing.def.entityDefToBuild.passability == Traversability.Impassable)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }
    }
}