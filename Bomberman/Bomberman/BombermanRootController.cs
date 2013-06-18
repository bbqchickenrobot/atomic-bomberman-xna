﻿using System;
using Assets;
using BomberEngine.Consoles;
using BomberEngine.Core.Input;
using BomberEngine.Game;
using Bomberman.Game;
using Bomberman.Menu;
using Bomberman.Multiplayer;
using Bomberman.Networking;
using Microsoft.Xna.Framework.Content;
using System.Net;

namespace Bomberman
{
    public class BombermanRootController : RootController
    {
        private MenuController menuController;
        private MultiplayerManager multiplayerManager;

        public BombermanRootController(ContentManager contentManager)
            : base(contentManager)
        {
            multiplayerManager = new MultiplayerManager();
        }

        protected override void OnStart()
        {
            BombermanAssetManager manager = (BombermanAssetManager)Application.Assets();
            manager.AddPackToLoad(AssetPacks.Packs.ALL);
            manager.LoadImmediately();

            StartMainMenu();
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            multiplayerManager.Update(delta);
        }

        protected override void OnControllerStop(Controller controller)
        {
            if (controller is MenuController)
            {
                MenuController.ExitCode exitCode = (MenuController.ExitCode)controller.exitCode;
                if (exitCode == MenuController.ExitCode.Quit)
                {
                    Application.sharedApplication.Stop();
                    return;
                }

                switch (exitCode)
                {
                    case MenuController.ExitCode.SingleStart:
                    {
                        GameSettings settings = new GameSettings("x");
                        StartController(new GameController(settings));
                        break;
                    }

                    case MenuController.ExitCode.MultiplayerStart:
                    {
                        StartController(new MultiplayerController());
                        break;
                    }
                }
            }
            else if (controller is MultiplayerController)
            {
                MultiplayerController.ExitCode exitCode = (MultiplayerController.ExitCode)controller.exitCode;
                if (exitCode == MultiplayerController.ExitCode.Cancel)
                {
                    StartMainMenu();
                    return;
                }

                switch (exitCode)
                {
                    case MultiplayerController.ExitCode.Create:
                    {
                        GameSettings settings = new GameSettings("x");
                        settings.multiplayer = GameSettings.Multiplayer.Server;
                        StartController(new GameController(settings));
                        break;
                    }

                    case MultiplayerController.ExitCode.Join:
                        break;
                }
            }
            else if (controller is GameController)
            {
                StartMainMenu();
            }
        }

        protected override CConsole CreateConsole()
        {
            CConsole console = base.CreateConsole();
            CVars.Register(console);
            return console;
        }

        public override bool OnKeyPressed(KeyEventArg e)
        {
            if (base.OnKeyPressed(e))
            {
                return true;
            }

            if (e.key == KeyCode.Oem8)
            {
                ToggleConsole();
                return true;
            }

            return false;
        }

        private void StartMainMenu()
        {
            menuController = new MenuController();
            StartController(menuController);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Multiplayer

        public void StartGameServer(ServerListener listener)
        {
            String appId = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            multiplayerManager.CreateServer(appId, port);
            multiplayerManager.SetServerListener(listener);
            multiplayerManager.Start();
        }

        public void StartGameClient(IPEndPoint remoteEndPoint, ClientListener listener)
        {
            String appId = CVars.sv_appId.value;
            int port = CVars.sv_port.intValue;

            multiplayerManager.CreateClient(appId, remoteEndPoint, port);
            multiplayerManager.SetClientListener(listener);
            multiplayerManager.Start();
        }

        public void StopPeer()
        {
            multiplayerManager.Stop();
        }

        // TODO: move server discovery here

        #endregion
    }
}
