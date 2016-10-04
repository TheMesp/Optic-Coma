﻿using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Optic_Coma
{
    public class Entity
    {
        public Texture2D Texture { get; set; }
        public static Vector2 currentPosition;
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)currentPosition.X,
                    (int)currentPosition.Y,
                    Texture.Width,
                    Texture.Height
                    );
            }
        }
        //Generic function that can be used to check for a collision.
        public bool checkCollision(Entity Collider)
        {
            if (BoundingBox.Intersects(Collider.BoundingBox))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Player : Entity
    {
        public Texture2D Texture { get; set; }
        public static Vector2 currentPosition;
        static float flashAngle = 0f;
        float playerAngle = 0f;
        Vector2 facingDirection;
        
        Vector2 initPosition;

        Vector2 mouseLoc;
        Texture2D flashLightTexture;

        public Player(Texture2D texture, Vector2 initPos, Texture2D flashlightTexture)
        {
            initPosition = initPos;
            currentPosition = initPos;
            Texture = texture;
            flashLightTexture = flashlightTexture;
        }

        public void Update()
        {
            MouseState curMouse = Mouse.GetState();

            mouseLoc = new Vector2(curMouse.X, curMouse.Y);
            mouseLoc.X = curMouse.X;
            mouseLoc.Y = curMouse.Y;

            facingDirection = mouseLoc - currentPosition;

            // using radians
            // measure clockwise from left
            flashAngle = (float)(Math.Atan2(facingDirection.Y, facingDirection.X)) + (float)Math.PI;

            if ((flashAngle > 0 && flashAngle <= Math.PI / 4) || (flashAngle > Math.PI * 7 / 4 && flashAngle <= 2 * Math.PI))
            {
                playerAngle = (float)Math.PI; //Right
            }
            else if (flashAngle > Math.PI / 4 && flashAngle <= Math.PI * 3 / 4)
            {
                playerAngle = -(float)Math.PI / 2; //Down
            }
            else if (flashAngle > Math.PI * 3 / 4 && flashAngle <= Math.PI * 5 / 4)
            {
                playerAngle = 0f; //Left
            }
            else if (flashAngle > Math.PI * 5 / 4 && flashAngle <= Math.PI * 7 / 4)
            {
                playerAngle = (float)Math.PI / 2; //Up
            }

        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.W))
                currentPosition.Y -= (4 * walkMult((float)Math.PI / 2, flashAngle, 1, false));
            if (keyState.IsKeyDown(Keys.A))
                currentPosition.X -= (4 * walkMult(0, flashAngle, 1, false));
            if (keyState.IsKeyDown(Keys.S))
                currentPosition.Y += (4 * walkMult(3 * (float)Math.PI / 2, flashAngle, 1, false));
            if (keyState.IsKeyDown(Keys.D))
                currentPosition.X += (4 * walkMult((float)Math.PI, flashAngle, 1, false));      
            spriteBatch.DrawString(font, "enemyAngle: " + Enemy.enemyAngle, new Vector2(700, 100), Color.White);
            spriteBatch.DrawString(font, "moveAmp: " + Enemy.moveAmp, new Vector2(700, 120), Color.White);
            spriteBatch.DrawString(font, "coolliding: " + Enemy.moveAmp, new Vector2(700, 140), Color.White);
            spriteBatch.Draw
            (
                Texture,
                new Rectangle
                (
                    (int)currentPosition.X,
                    (int)currentPosition.Y,
                    Texture.Width,
                    Texture.Height
                ),
                null,
                Color.White,
                playerAngle,
                new Vector2
                (
                    Texture.Width / 2,
                    Texture.Height / 2
                ),
                SpriteEffects.None,
                ScreenManager.Instance.EntityLayer
            );
            spriteBatch.Draw
            (
                flashLightTexture,
                new Rectangle
                (
                    (int)currentPosition.X,
                    (int)currentPosition.Y,
                    flashLightTexture.Width,
                    flashLightTexture.Height
                ),
                null,
                Color.White,
                flashAngle,
                new Vector2
                (
                    flashLightTexture.Width / 2,
                    flashLightTexture.Height / 2
                ),
                SpriteEffects.None,
                ScreenManager.Instance.FlashlightLayer
            );
        }
        public static float walkMult(float dir, float angle, float amp, bool useExp)
        {
            //dir, in this method, is equal to the angle in radians the character is moving.
            //angle is the "best" angle - the one that results in fastest movement.
            //amp is in regards to how powerful the slowing effect is.
            //First we check if the flash is roughly pointing the same way we are going.
            if (
                ((7 * Math.PI / 4 < dir || dir <= 1 * Math.PI / 4) && (7 * Math.PI / 4 < angle || angle <= 1 * Math.PI / 4)) ||//Both westward?
                ((1 * Math.PI / 4 < dir && dir <= 3 * Math.PI / 4) && (1 * Math.PI / 4 < angle && angle <= 3 * Math.PI / 4)) ||//Both northward?
                ((3 * Math.PI / 4 < dir && dir <= 5 * Math.PI / 4) && (3 * Math.PI / 4 < angle && angle <= 5 * Math.PI / 4)) ||//Both eastward?
                ((5 * Math.PI / 4 < dir && dir <= 7 * Math.PI / 4) && (5 * Math.PI / 4 < angle && angle <= 7 * Math.PI / 4))   //Both southward?
              )
            {
                if (useExp)
                    return (float)Math.Pow(1, amp);
                else
                    return 1 * amp;
                
            }
            else if //Then we check if the person is directly backpedalling.
              (
                ((7 * Math.PI / 4 < dir || dir <= 1 * Math.PI / 4) && (3 * Math.PI / 4 < angle && angle <= 5 * Math.PI / 4)) ||//Backpedaling west?
                ((1 * Math.PI / 4 < dir && dir <= 3 * Math.PI / 4) && (5 * Math.PI / 4 < angle && angle <= 7 * Math.PI / 4)) ||//Backpedaling north?
                ((3 * Math.PI / 4 < dir && dir <= 5 * Math.PI / 4) && (7 * Math.PI / 4 < angle || angle <= 1 * Math.PI / 4)) ||//Backpedaling east?
                ((5 * Math.PI / 4 < dir && dir <= 7 * Math.PI / 4) && (1 * Math.PI / 4 < angle && angle <= 3 * Math.PI / 4))   //Backpedaling south?
              )
            {
                if (useExp)
                    return (float)Math.Pow(0.5f, amp);
                else
                    return 0.5f * amp;
            }
            else //Must be sidestepping, then.
            {
                if (useExp)
                    return (float)Math.Pow(0.75f, amp);
                else
                    return 0.75f * amp;
            }
        }
    }
    class Enemy : Entity
    {
        public Texture2D Texture { get; set; }
        public static Vector2 currentPosition;
        public static float enemyAngle = 0f;
        public Vector2 InitPosition;
        Random random = new Random();
        int speed;
        int dir;
        public static float moveAmp;
        public int acceleration = 0;

        public Enemy(Texture2D texture, Vector2 initPosition)
        {
            Texture = texture;
            InitPosition = initPosition;
            currentPosition = InitPosition;
            speed = 2 + acceleration;
            moveAmp = -1;
        }

        public void Update()
        {
            enemyAngle = (float)(Math.Atan2(Player.currentPosition.Y - currentPosition.Y, Player.currentPosition.X - currentPosition.X)) + (float)Math.PI;
            //moveAmp += 0.001f;
            moveAmp = 2; //We can toy around with this later.
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            dir = random.Next(0, 4);
            if (dir == 0)
                currentPosition.Y -= (4 * Player.walkMult((float)Math.PI / 2, enemyAngle, moveAmp, false));
            else if (dir == 1)
                currentPosition.X -= (4 * Player.walkMult(0, enemyAngle, moveAmp, false));
            else if (dir == 2)
                currentPosition.Y += (4 * Player.walkMult(3 * (float)Math.PI / 2, enemyAngle, moveAmp, false));
            else
                currentPosition.X += (4 * Player.walkMult((float)Math.PI, enemyAngle, moveAmp, false));
            spriteBatch.Draw
            (
                Texture,
                new Rectangle
                (
                    (int)currentPosition.X,
                    (int)currentPosition.Y,
                    Texture.Width,
                    Texture.Height
                ),
                null,
                Color.White,
                enemyAngle,
                new Vector2
                (
                    Texture.Width / 2,
                    Texture.Height / 2
                ),
                SpriteEffects.None,
                ScreenManager.Instance.EntityLayer
            );
        }
    }
}
