﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bomberman.Gameplay.Multiplayer;
using BombermanTests.Mocks;
using Bomberman.Gameplay;

namespace BombermanTests.Network
{
    public class GameControllerNetworkMock : GameControllerNetwork
    {
        public GameControllerNetworkMock() :
            base(new GameMock(15, 11), new GameSettings(new SchemeMock("test", 90)))
        {
        }
    }

    public class GameControllerClientMock : GameControllerClient
    {
        public GameControllerClientMock() :
            base(new GameMock(15, 11), new GameSettings(new SchemeMock("test", 90)))
        {
        }
    }

    public class GameControllerServerMock : GameControllerServer
    {
        public GameControllerServerMock() :
            base(new GameMock(15, 11), new GameSettings(new SchemeMock("test", 90)))
        {
        }
    }
}
