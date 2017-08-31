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
        ILinkGroup Concat(ILinkGroup other);
        ILinkGroup Split(ILinkData point);
        Vector3 RecalculateCenter();

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

        bool CanLinkFromOther(int i);

        void Notify_Linked(ILinkData other, int linkedFrom);
        void Notify_UnLinked(ILinkData other, int unlinkedFrom);
    }
}
