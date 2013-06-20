﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Assets.Types;
using Assets;
using Bomberman.Networking;
using Bomberman.Game;
using Bomberman.Content;

namespace Bomberman.Multiplayer
{
    public class MultiplayerLobbyScreen : Screen
    {   
        public enum ButtonId
        {
            Back,
            Start,
            Ready,
        }

        private View clientsView;
        private ServerInfo serverInfo;
        private bool isServer;

        public MultiplayerLobbyScreen(ServerInfo serverInfo, ButtonDelegate buttonDelegate, bool isServer)
        {
            this.serverInfo = serverInfo;

            View contentView = new View(512, 363);
            contentView.alignX = View.ALIGN_CENTER;
            contentView.x = 0.5f * width;
            contentView.y = 48;

            Font font = Helper.GetFont(A.fnt_button);

            contentView.AddView(new View(215, 145));

            TextView serverName = new TextView(font, serverInfo.name);
            serverName.alignX = View.ALIGN_CENTER;
            serverName.x = 113;
            serverName.y = 155;

            contentView.AddView(serverName);

            clientsView = new View(286, contentView.height);
            clientsView.alignX = View.ALIGN_MAX;
            clientsView.x = contentView.width;
            contentView.AddView(clientsView);

            AddView(contentView);

            View buttons = new View();
            buttons.x = 0.5f * width;
            buttons.y = 432;
            buttons.alignX = View.ALIGN_CENTER;
            buttons.alignY = View.ALIGN_MAX;

            Button button = new TextButton("BACK", 0, 0, 100, 20);
            button.SetDelegate(buttonDelegate);
            button.id = (int)ButtonId.Back;
            buttons.AddView(button);
            SetBackButton(button);

            String label = isServer ? "START" : "READY";
            ButtonId buttonId = isServer ? ButtonId.Start : ButtonId.Ready;

            button = new TextButton(label, 0, 0, 100, 20);
            button.SetDelegate(buttonDelegate);
            button.id = (int)buttonId;
            buttons.AddView(button);

            buttons.LayoutHor(10);
            buttons.ResizeToFitViews();
            AddView(buttons);
        }

        public void AddPlayer(Object name)
        {

        }
    }
}
