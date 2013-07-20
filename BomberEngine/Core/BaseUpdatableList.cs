﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Util;

namespace BomberEngine.Core
{
    public abstract class BaseUpdatableList<T> : BaseList<T>, IUpdatable where T : class, IUpdatable
    {   
        protected BaseUpdatableList()
        {
        }

        protected BaseUpdatableList(T nullElement)
            : base(nullElement, 0)
        {
        }

        protected BaseUpdatableList(T nullElement, int capacity)
            : base(nullElement, capacity)
        {   
        }

        public virtual void Update(float delta)
        {
            int elementsCount = list.Count;
            for (int i = 0; i < elementsCount; ++i) // do not update added items on that tick
            {
                list[i].Update(delta);
            }

            if (removedCount > 0)
            {
                ClearRemoved();
            }
        }
    }
}
