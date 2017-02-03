﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET_Test
{
    public class FakeEventDataSpecificLeftComparer : FakeEventDataSpecificLeft
    {
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ Pan.GetHashCode();
                hash = hash * 16777619 ^ Ref.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            Type t = typeof(FakeEventDataSpecificRight);
            if (obj.GetType().BaseType == t)
            {
                return EvaluateOnCondition(this, (FakeEventDataSpecificRight)obj);
            }
            else
            {
                return CompareSameObjects((FakeEventDataSpecificLeft)obj, this);
            }
        }

        public static bool CompareSameObjects(FakeEventDataSpecificLeft a, FakeEventDataSpecificLeft b)
        {
            return a.Pan == b.Pan && a.Ref == b.Ref;
        }

        public static bool EvaluateOnCondition(dynamic x, dynamic y)
        {
            return x.Pan == y.Pan && x.Ref == y.Ref;
        }
    }
}
