﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldCell
    {
        /* Cells coordinates */
        private int x;
        private int y;

        public FieldCell(int x, int y)
        {
            Set(x, y);
        }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public virtual bool IsEmpty()
        {
            return false;
        }

        public virtual bool IsSolid()
        {
            return false;
        }

        public virtual bool IsBreakable()
        {
            return false;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }
    }
}
