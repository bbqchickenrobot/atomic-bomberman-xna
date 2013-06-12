﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Bomberman.Game;
using Assets;
using Bomberman.Network;
using BomberEngine.Debugging;
using BomberEngine.Core;

namespace Bomberman.Menu.Screens
{
    public class MultiplayerScreen : Screen
    {
        public enum ButtonId
        {
            Create,
            Refresh,
            Join,
            Back
        }

        private LocalServersDiscovery serverDiscovery;
        private List<ServerInfo> foundServers;

        private View containerView;
        private ButtonDelegate buttonDelegate;

        public MultiplayerScreen(ButtonDelegate buttonDelegate)
            : base((int)MenuController.ScreenID.Multiplayer)
        {
            int w = 100;
            int h = 20;

            this.buttonDelegate = buttonDelegate;

            Font font = Helper.GetFont(A.fnt_button);

            View rootView = new View(0, 0, width, height);

            // found servers
            containerView = new View();

            containerView.ResizeToFitViews();

            containerView.x = 0.5f * (rootView.width - containerView.width);
            containerView.y = 0.5f * (rootView.height - containerView.height);

            rootView.AddView(containerView);

            // buttons

            View buttonContainer = new View();

            TextButton button = new TextButton("Back", font, 0, 0, w, h);
            button.id = (int)ButtonId.Back;
            button.SetDelegate(OnButtonPressed);
            buttonContainer.AddView(button);

            button = new TextButton("Refresh", font, 0, 0, w, h);
            button.id = (int)ButtonId.Refresh;
            button.SetDelegate(OnButtonPressed);
            buttonContainer.AddView(button);

            button = new TextButton("Create", font, 0, 0, w, h);
            button.id = (int)ButtonId.Create;
            button.SetDelegate(buttonDelegate);
            buttonContainer.AddView(button);

            buttonContainer.LayoutHor(0);
            buttonContainer.ResizeToFitViews(true, true, 20);

            rootView.AddView(buttonContainer);

            buttonContainer.x = 0.5f * (rootView.width - buttonContainer.width);
            buttonContainer.y = rootView.height - buttonContainer.height;

            AddView(rootView);

            foundServers = new List<ServerInfo>();
        }

        protected override void OnStart()
        {
            StartDiscovery();
        }

        protected override void OnStop()
        {
            StopDiscovery(false);
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Local servers discovery

        private void StartDiscovery()
        {
            Debug.Assert(serverDiscovery == null);

            String name = CVars.sv_name.value;
            int port = CVars.sv_port.intValue;

            serverDiscovery = new LocalServersDiscovery(OnLocalServerFound, name, port);
            serverDiscovery.Start();

            foundServers.Clear();

            AddUpdatable(UpdateDiscovery);
            ScheduleCall(StopDiscoveryCall, 5.0f);

            Log.i("Started local servers discovery...");
            SetBusy();
        }

        private void UpdateDiscovery(float delta)
        {
            serverDiscovery.Update(delta);
        }

        private void StopDiscoveryCall(DelayedCall call)
        {
            StopDiscovery(true);
        }

        private void StopDiscovery(bool updateUI)
        {
            if (serverDiscovery != null)
            {
                serverDiscovery.Stop();
                serverDiscovery = null;

                RemoveUpdatable(UpdateDiscovery);

                if (updateUI)
                {
                    UpdateFoundServersUI();
                }

                Log.i("Stopped local servers discovery");
            }
        }

        private void OnLocalServerFound(ServerInfo info)
        {
            Log.d("Found local server: " + info.endPoint);
            foundServers.Add(info);
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region UI

        private void SetBusy()
        {
            ClearContainer();

            Font font = Helper.GetFont(A.fnt_button);
            TextView busyText = new TextView(font, "Searching for local servers...");
            containerView.AddView(busyText);
            busyText.x = 0.5f * (containerView.width - busyText.width);
            busyText.y = 0.5f * (containerView.height - busyText.height);
        }

        private void UpdateFoundServersUI()
        {
            ClearContainer();

            Font font = Helper.GetFont(A.fnt_button);

            if (foundServers.Count > 0)
            {
                View buttonContainer = new View(width, 0);

                for (int i = 0; i < foundServers.Count; ++i)
                {
                    ServerInfo info = foundServers[i];

                    TextButton serverButton = new TextButton(info.name + " - " + info.endPoint, font, 0, 0, 300, 20);
                    serverButton.id = (int)ButtonId.Join;
                    serverButton.data = info;
                    serverButton.SetDelegate(buttonDelegate);
                    buttonContainer.AddView(serverButton);
                    serverButton.x = 0.5f * (buttonContainer.width - serverButton.width);
                }

                buttonContainer.LayoutVer(0);
                buttonContainer.ResizeToFitViewsVer();

                containerView.AddView(buttonContainer);
                buttonContainer.x = 0.5f * (containerView.width - buttonContainer.width);
                buttonContainer.y = 0.5f * (containerView.height - buttonContainer.height);
            }
            else
            {
                TextView emptyText = new TextView(font, "No servers found");
                containerView.AddView(emptyText);
                emptyText.x = 0.5f * (containerView.width - emptyText.width);
                emptyText.y = 0.5f * (containerView.height - emptyText.height);
            }
        }

        private void ClearContainer()
        {
            if (containerView.ChildCount() > 0)
            {
                containerView.RemoveViews();
            }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        private void OnButtonPressed(Button button)
        {
            ButtonId buttonId = (ButtonId)button.id;
            if (buttonId == ButtonId.Back)
            {
                Finish();
            }
            else if (buttonId == ButtonId.Refresh)
            {
                StopDiscovery(false);
                StartDiscovery();
            }
        }
    }
}