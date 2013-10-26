﻿using System;
using BomberEngine.Core.Input;
using BomberEngine.Core.Events;

namespace BomberEngine.Core.Visual
{
    public delegate void ButtonDelegate(Button button);
    
    public class Button : View
    {
        public ButtonDelegate buttonDelegate;
        public Object data;

        public Button()
            : this(0, 0, 0, 0)
        {
        }

        public Button(float width, float height)
            : this(0, 0, width, height)
        {
        }

        public Button(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            focusable = true;
        }

        public override bool HandleEvent(Event evt)
        {
            if (base.HandleEvent(evt))
            {
                return true;
            }

            if (evt.code == Event.KEY)
            {
                KeyEvent keyEvent = evt as KeyEvent;
                if (keyEvent.IsConfirmKey())
                {
                    if (keyEvent.IsPressed)
                    {
                        OnPress();
                        return true;                        
                    }

                    if (keyEvent.IsReleased)
                    {
                        OnRelease();
                        return true;
                    }
                }
            }

            return false;
        }

        protected virtual void OnPress()
        {
            if (buttonDelegate != null)
            {
                buttonDelegate(this);
            }
        }

        protected virtual void OnRelease()
        {
        }
    }
}
