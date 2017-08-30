using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace LinkableDoors
{
    static class LinkGroupUtility
    {
        private static List<ILinkGroup> linkedGroup = new List<ILinkGroup>();

        public static void Notify_LinkableSpawned(ILinkData newObj)
        {
            ILinkGroup newGroup = LinkGroupUtility.CheckAround(newObj);
            if (newGroup != null)
            {
                LinkGroupUtility.GroupRegister(newGroup);
            }
        }
        public static void Notify_LinkableDeSpawned(ILinkData delObj)
        {
            ILinkGroup parent = delObj.GroupParent;
            if (parent.Children.First() == delObj || parent.Children.Last() == delObj)
            {
                parent.Remove(delObj);
                if (parent.Any())
                {
                    parent.RecalculateCenter();
                    return;
                }
                LinkGroupUtility.GroupUnRegister(parent);
                return;
            }
            ILinkGroup newGroup = parent.Split(delObj);
            LinkGroupUtility.GroupRegister(newGroup);
        }
        private static void GroupRegister(ILinkGroup newGropu)
        {
            Log.Message("Register01:" + linkedGroup.Count + " children:" + newGropu.Children.Count());
            LinkGroupUtility.linkedGroup.Add(newGropu);
            Log.Message("Register02:" + linkedGroup.Count + " c:"+ linkedGroup.First().Children.Count() + " children:" + newGropu.Children.Count());
        }
        private static void GroupUnRegister(ILinkGroup delGropu)
        {
            Log.Message("UnRegister01:" + linkedGroup.Count + " children:" + delGropu.Children.Count());
            LinkGroupUtility.linkedGroup.Remove(delGropu);
            Log.Message("UnRegister02:" + linkedGroup.Count + " children:" + delGropu.Children.Count());
        }

        public static ILinkGroup CheckAround(ILinkData newObj)
        {
            ILinkGroup result = new LinkGroup(newObj);
            newObj.GroupParent = result;
            int invert = -1;
            for (int i = 0; i < 4; i++)
            {
                IntVec3 pos = newObj.Pos + GenAdj.CardinalDirections[i];
                Building_Door door = newObj.Map.thingGrid.ThingAt<Building_Door>(pos);
                ILinkData current = door?.TryGetComp<CompLinkable>();
                if (current != null && current.CanLinkFromOther() && (invert == -1 || invert == i))
                {
                    invert = ((i + 2) % 4);
                    LinkGroupUtility.GroupUnRegister(newObj.GroupParent);
                    current.GroupParent.Concat(newObj.GroupParent);
                    result = null;
                    newObj.Notify_Linked(current, (int)Math.Pow(2, invert));
                    current.Notify_Linked(newObj, (int)Math.Pow(2, i));
                }
            }
            return result;
        }
    }
}
