﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGameLibrary.Sprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Util;

namespace BreakoutTest
{
    class Paddle : DrawableSprite
    {
        //Service Dependencies
        GameConsole console;

        //Dependencies
        PaddleController controller;
        Ball ball;      //Need reference to ball for collision
        
        public Paddle(Game game, Ball b)
            : base(game)
        {
            this.Speed = 200;
            this.ball = b;
            controller = new PaddleController(game, ball);

            console = (GameConsole)this.Game.Services.GetService(typeof(IGameConsole));
            if (console == null) //ohh no no console
            {
                console = new GameConsole(this.Game);
                this.Game.Components.Add(console);  //add a new game console to Game
            }
        }

        protected override void LoadContent()
        {
            this.spriteTexture = this.Game.Content.Load<Texture2D>("paddleSmall");
#if DEBUG
            this.ShowMarkers = true;
#endif
            SetInitialLocation();
            base.LoadContent();
        }

        public void SetInitialLocation()
        {
            this.Location = new Vector2(300, 450);

        }

        Rectangle collisionRectangle;  //Rectangle for paddle collision uses just the top of the paddle

        public override void Update(GameTime gameTime)
        {
            //Update Collision Rect
            collisionRectangle = new Rectangle((int)this.Location.X, (int)this.Location.Y, this.spriteTexture.Width, 1);

            //Deal with ball
            switch (ball.State)
            {
                case BallState.OnPaddleStart:
                    //Move the ball with the paddle until launch
                    UpdateMoveBallWithPaddle();
                    break;
                case BallState.Playing:
                    UpdateCheckBallCollision();
                    break;
            }

            //Movement from controller
            controller.HandleInput(gameTime);
            this.Direction = controller.Direction;
            this.Location += this.Direction * (this.Speed * gameTime.ElapsedGameTime.Milliseconds / 1000);

            KeepPaddleOnScreen();
            base.Update(gameTime);
        }

        private void UpdateMoveBallWithPaddle()
        {
            ball.Speed = 0;
            ball.Direction = Vector2.Zero;
            ball.Location = new Vector2(this.Location.X + (this.LocationRect.Width/2 - ball.SpriteTexture.Width/2), this.Location.Y - ball.SpriteTexture.Height);
        }

        private void UpdateCheckBallCollision()
        {
            //Ball Collsion
            //Very simple collision with ball only uses rectangles
            if (collisionRectangle.Intersects(ball.LocationRect))
            {
                //TODO Change angle based on location of collision or direction of paddle
                ball.Direction.Y *= -1;
                UpdateBallCollisionBasedOnPaddleImpactLocation();
                UpdateBallCollisionRandomFuness();
                console.GameConsoleWrite("Paddle collision ballLoc:" + ball.Location + " paddleLoc:" + this.Location.ToString());
            }
        }

        private void UpdateBallCollisionRandomFuness()
        {
            //
        }

        private void UpdateBallCollisionBasedOnPaddleImpactLocation()
        {
            //Change angle based on paddle movement
            if (this.Direction.X > 0)
            {
                ball.Direction.X += .5f;
            }
            if (this.Direction.X < 0)
            {
                ball.Direction.X -= .5f;
            }
            //Change anlge based on side of paddle
            //First Third

            if ((ball.Location.X > this.Location.X) && (ball.Location.X < this.Location.X + this.spriteTexture.Width / 3))
            {
                console.GameConsoleWrite("1st Third");
                ball.Direction.X += .1f;
            }
            if ((ball.Location.X > this.Location.X + (this.spriteTexture.Width / 3)) && (ball.Location.X < this.Location.X + (this.spriteTexture.Width / 3) * 2))
            {
                console.GameConsoleWrite("2nd third");
            }
            if ((ball.Location.X > (this.Location.X + (this.spriteTexture.Width / 3) * 2)) && (ball.Location.X < this.Location.X + (this.spriteTexture.Width)))
            {
                console.GameConsoleWrite("3rd third");
                ball.Direction.X -= .1f;
            }
        }

        private void KeepPaddleOnScreen()
        {
            this.Location.X = MathHelper.Clamp(this.Location.X, 0, this.Game.GraphicsDevice.Viewport.Width - this.spriteTexture.Width);
        }
    }
}
