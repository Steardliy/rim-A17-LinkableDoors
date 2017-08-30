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
        
    }
    public interface ILinkData
    {
        ILinkGroup GroupParent { get; set; }
        IntVec3 Pos { get; }
        Vector3 DrawPos { get; }
        Map map { get; }

        bool CanLinkFromOther();

        void Notify_Linked(ILinkData other, int linkType);
        void Notify_UnLinked(ILinkData other, int linkType);
    }
}
