﻿using System.Collections.Generic;
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
        void Remove(ILinkData delData);
        void Add(ILinkData newData);
    }
    public interface ILinkData
    {
        ILinkGroup GroupParent { get; set; }
        PositionFlag PosFlag { get; set; }
        IntVec3 Pos { get; }
        Vector3 DrawPos { get; }
        Map Map { get; }
        int LinkingFrom { get; }
        int DirectLinkCount { get; }

        bool CanLinkFromOther(int direction);
        void Reset();

        void Notify_Linked(ILinkData other, int direction);
        void Notify_UnLinked(ILinkData other, int direction);
    }

    public enum PositionFlag : byte
    {
        None,
        RightSide,
        RightBorder,
        LeftSide,
        LeftBorder,
        Center
    }
}
