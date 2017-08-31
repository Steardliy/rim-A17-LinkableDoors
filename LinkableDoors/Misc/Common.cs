using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace LinkableDoors
{
    public interface ILinkGroup
    {
        IEnumerable<ILinkData> Children { get; }
        void Concat(ILinkGroup other);
        void Split(ILinkData point);
        void RecalculateCenter();

        bool Any();
        void Remove(ILinkData delObj);
        void Add(ILinkData newObj);
    }
    public interface ILinkData
    {
        ILinkGroup GroupParent { get; set; }
        IntVec3 Pos { get; }
        Vector3 DrawPos { get; }
        Map Map { get; }
        int LinkingFrom { get; }

        bool CanLinkFromOther(int i);
        void Reset();

        void Notify_Linked(ILinkData other, int linkedFrom);
        void Notify_UnLinked(ILinkData other, int unlinkedFrom);
    }
}
