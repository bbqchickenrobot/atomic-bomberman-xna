﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using Bomberman.Menu.Screens;

namespace Bomberman.Menu
{
    public class MenuController : Controller
    {
        public MenuController(Controller parentController)
            : base(parentController)
        {
        }

        protected override void OnStart()
        {
            StartScreen(new MainMenuScreen());
        }
    }
}
