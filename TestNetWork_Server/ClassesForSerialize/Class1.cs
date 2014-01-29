using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace ClassesForSerialize
{
    [Serializable]
    public class StartPacket
    {
        public String username { get; set; }
        public int port { get; set; }

        public StartPacket()
        {

        }
    }

    [Serializable]
    public class PacketToServer
    {
        public PlayerInfo PlayerInfo { get; set; }
        public BulletInfo BulletInfo { get; set; }
        public int Port { get; set; }

        public PacketToServer()
        {

        }
    }

    [Serializable]
    public class PacketFromServer
    {
        public List<PlayerInfo> PlayersInfo { get; set; }
        public List<BulletInfo> BulletInfo { get; set; }

        public PacketFromServer()
        {

        }
    }

    public class Player
    {
        public PlayerInfo PlayerInfo { get; set; }

        public Player()
        {
            PlayerInfo = new PlayerInfo();

            PlayerInfo.Direction = 2;
        }

        public void draw(SpriteBatch spriteBatch)
        {
            if (PlayerInfo.Direction == 1)
            {
                spriteBatch.Draw(GameConstants.playerR, PlayerInfo.Position, null, Color.White);
            }
            else
            {
                spriteBatch.Draw(GameConstants.playerL, PlayerInfo.Position, null, Color.White);
            }

        }
        public void move()
        {

            if (GameConstants.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
            {
                PlayerInfo.Direction = 1;
                PlayerInfo.Position.X += PlayerInfo.Speed;
            }
            if (GameConstants.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                PlayerInfo.Direction = 2;

                PlayerInfo.Position.X -= PlayerInfo.Speed;
            }

            if (PlayerInfo.Position.Y == GameConstants.groundLevel && GameConstants.keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                PlayerInfo.VerticalSpeed = 10;
            }

            if (PlayerInfo.VerticalSpeed > -9000)
            {
                PlayerInfo.VerticalSpeed -= GameConstants.g;
            }

            if (PlayerInfo.Position.Y - PlayerInfo.VerticalSpeed < GameConstants.groundLevel)
            {
                PlayerInfo.Position.Y -= PlayerInfo.VerticalSpeed;
            }
            else
            {
                PlayerInfo.Position.Y = GameConstants.groundLevel;
            }


        }
    }


    [Serializable]
    public class PlayerInfo
    {
        public float Speed { get; set; }
        public Vector2 Position;
        public float VerticalSpeed { get; set; }
        public int Direction { get; set; }
        public Rectangle Rectangle { get; set; }
        public int HP { get; set; }
        public int teamNumber { get; set; }

        public PlayerInfo()
        {
            Speed = GameConstants.moveSpeed;
            VerticalSpeed = 0;
            HP = 100;
            Position = new Vector2(200, 130);
        }
    }

    public static class GameConstants
    {
        public static GameTime gameTime;
        public static KeyboardState keyboardState;
        public static MouseState mouseState;
        public static float g;
        public static Texture2D bullet;
        public static Texture2D playerR;
        public static Texture2D playerL;
        public static int groundLevel = 255;
        public static float moveSpeed = 5f;
        public static float bulletSpeed = 10f;
        public static int Damage = 10;
    }


    public class Bullet
    {
        // public Texture2D Texture { get; set; }
        public BulletInfo BulletInfo { get; set; }

        public Bullet()
        {
            BulletInfo = new BulletInfo();
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameConstants.bullet, BulletInfo.Position, null, Color.White);
        }

        public void move()
        {
            BulletInfo.move();
        }
    }

    [Serializable]
    public class BulletInfo
    {
        public Vector2 Position { get; set; }
        public float Speed { get; set; }
        public int Damage { get; set; }
        public int teamNumber { get; set; }

        public BulletInfo()
        {
            Speed = GameConstants.bulletSpeed;
            Damage = GameConstants.Damage;
        }

        public void move()
        {
            Position = new Vector2(Position.X + Speed, Position.Y);
        }
    }
}