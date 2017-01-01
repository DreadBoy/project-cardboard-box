using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cardboard
{

    public class Player
    {
        public Game game;
        NetworkConnection connection;

        float speed = 0.5f;
        float rotationspeed = 0.5f;

        int x, y;
        public Vector3 position
        {
            get
            {
                return new Vector3(x, 0, y);
            }
            set
            {
                x = (int)value.x;
                y = (int)value.z;
                spawnPlayerOnGridEvent.RaiseEvent(new SpawnPlayerOnGridArgs(position));
            }
        }

        float offsetx, offsety;
        public Vector3 offset
        {
            get
            {
                return new Vector3(offsetx, 0, offsety);
            }
            private set { }
        }

        int angle;
        public Quaternion rotation
        {
            get
            {
                return Quaternion.Euler(0, angle, 0);
            }
            private set { }
        }

        float offsetangle;
        public Quaternion offsetrotation
        {
            get
            {
                return Quaternion.Euler(0, offsetangle * 90, 0);
            }
            private set { }
        }

        public SmartEvent<SpawnPlayerOnGridArgs> spawnPlayerOnGridEvent = new SmartEvent<SpawnPlayerOnGridArgs>();

        public Player()
        {
        }

        public bool MovePlayer(int x, int y)
        {
            var newx = this.x + x;
            var newy = this.y + y;

            if (newx < 0 || newx >= game.gridsize)
                return false;
            if (newy < 0 || newy >= game.gridsize)
                return false;

            this.x = newx;
            this.y = newy;

            offsetx = -x;
            offsety = -y;

            return true;
        }

        public void RotatePlayer(int quarter)
        {
            angle += quarter * 90;

            offsetangle = -quarter;
        }

        public void Update(float deltaTime)
        {
            if (Math.Abs(offsetx) > 0)
                offsetx -= (offsetx / Math.Abs(offsetx)) * deltaTime * speed;

            if (Math.Abs(offsety) > 0)
                offsety -= (offsety / Math.Abs(offsety)) * deltaTime * speed;

            if (Math.Abs(offsetangle) > 0)
                offsetangle -= (offsetangle / Math.Abs(offsetangle)) * deltaTime * rotationspeed;
        }
    }

}