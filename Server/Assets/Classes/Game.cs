using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace Cardboard
{
    public class Game
    {
        System.Random random = new System.Random();

        List<Player> players = new List<Player>();
        List<INetworkConnection> connections = new List<INetworkConnection>();

        public enum State
        {
            lobby,
            game
        }
        public State state { get; private set; }

        //Player[,] grid;
        public int gridsize
        {
            get; private set;
        }

        public SmartEvent<GridCompiledArgs> gridCompiledEvent = new SmartEvent<GridCompiledArgs>();
        public SmartEvent<spawnPlayerOnGridArgs> spawnPlayerOnGridEvent = new SmartEvent<spawnPlayerOnGridArgs>();
        public SmartEvent<changeStateArgs> changeStateEvent = new SmartEvent<changeStateArgs>();

        public Game(int gridsize)
        {
            //grid = new Player[gridsize, gridsize];
            this.gridsize = gridsize;
            gridCompiledEvent.RaiseEvent(new GridCompiledArgs(gridsize));

            state = State.lobby;
            changeStateEvent.RaiseEvent(new changeStateArgs(state));
        }

        public bool SpawnPlayerOnGrid(Player player)
        {
            //you already need reference to that player
            if (players.Find(pl => pl == player) == null)
                return false;

            List<int> x_all = Enumerable.Range(0, gridsize).ToList();
            List<int> y_all = Enumerable.Range(0, gridsize).ToList();

            while (x_all.Count > 0 && y_all.Count > 0)
            {
                var x = random.Next(x_all.Count);
                var y = random.Next(y_all.Count);

                if (players.Find(p => p.IsOnSpot(x, y)) != null)
                    //if (grid[x, y] == null)
                    return false;

                x_all.Remove(x);
                y_all.Remove(y);
                return SpawnPlayerOnGrid(player, x, y);


            }

            return false;
        }

        public bool SpawnPlayerOnGrid(Player player, int x, int y)
        {
            //you already need reference to that player
            if (players.Find(pl => pl == player) == null)
                return false;

            if (x < 0 || x >= gridsize)
                return false;
            if (y < 0 || y >= gridsize)
                return false;

            if (players.Find(p => p.IsOnSpot(x, y)) != null)
                //if (grid[x, y] != null)
                return false;

            player.position = new UnityEngine.Vector3(x, 0, y);
            spawnPlayerOnGridEvent.RaiseEvent(new spawnPlayerOnGridArgs(player));
            return true;
        }

        public Player PlayerConnect(INetworkConnection connection)
        {
            //Abort, abort!
            if (players.Count != connections.Count)
                return null;

            var player = new Player();
            players.Add(player);
            connections.Add(connection);
            player.game = this;
            connection.player = player;

            connection.CommandReceived.Event += CommandReceived_Event;

            return player;
        }

        private void CommandReceived_Event(object sender, CommandArgs e)
        {
            e.player.ReceiveCommand(e.command);
        }

        public void PlayerDisconnect(INetworkConnection connection)
        {
            //Abort, abort!
            if (players.Count != connections.Count)
                return;

            var index = connections.IndexOf(connection);

            players.RemoveAt(index);
            connections.RemoveAt(index);

        }

    }

}