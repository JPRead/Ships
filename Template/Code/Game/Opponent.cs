using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Engine7;
using Template.Game;

namespace Template
{
    /// <summary>
    /// Ship for the AI
    /// </summary>
    internal class Opponent : Ship
    {
        /// <summary>
        /// Aggressiveness of the AI - 0 is minimum aggressiveness, 1 is maximum
        /// </summary>
        private float aggressiveness;
        /// <summary>
        /// Used to track alignment variable from the previous tick
        /// </summary>
        private float alignmentLastTick;
        /// <summary>
        /// Current state of the AI - 0 idle, 1 attacking, 2 retreating, 3 boarding, 4 ramming
        /// </summary>
        private int state;
        /// <summary>
        /// Timer for 1 second delay
        /// </summary>
        internal Event stateTick;
        ///<summary>
        /// Used to easily get attributes from GameSetup.player
        /// </summary>
        internal Player player;
        /// <summary>
        /// Used with RotationHelpers to check when perpendicular to objects
        /// </summary>
        Sprite sideFaceSprite;
        /// <summary>
        /// True if boarding has begun
        /// </summary>
        private bool boardingStarted;

        /// <summary>
        /// Contains state machine for the AI
        /// </summary>
        /// <param name="startPos">Position to spawn at</param>
        public Opponent(Vector2 startPos)
        {
            //Init values
            Position2D = startPos;
            aggressiveness = 0.5f;
            GM.eventM.AddTimer(stateTick = new Event(1, "State Tick"));
            player = GameSetup.Player;
            alignmentLastTick = 0;
            boardingStarted = false;
            sideFaceSprite = new Sprite();
            sideFaceSprite.Frame.Define(Tex.SingleWhitePixel);
            GM.engineM.AddSprite(sideFaceSprite);
            sideFaceSprite.Visible = false;
            UpdateCallBack += Tick;
            FuneralCallBack += Death;
        }

        private int StateMachine()
        {
            if(state == 0)
            {
                shotTypeLeft = 0;
                shotTypeRight = 0;
                return 1;
            }

            if (state == 1)
            {
                float playerHealth = 0;
                float totalHealth = 0;
                for (int i = 0; i <= 6; i++)
                {
                    playerHealth += player.hitBoxArray[i].Health;
                    totalHealth += hitBoxArray[i].Health;
                }

                //Decide on shot type
                if (player.isRepairing && player.hitBoxSailBack.Health + player.hitBoxSailBack.Health + player.hitBoxSailBack.Health < 50)
                {
                    shotTypeLeft = 1;
                    shotTypeRight = 1;
                }
                if (GM.r.FloatBetween(0.8f, 1) < aggressiveness && GameSetup.WeatherController.RainAmount * 2 < aggressiveness)
                {
                    shotTypeLeft = 2;
                    shotTypeRight = 2;
                }
                if(GM.r.FloatBetween(0.7f, 1) < aggressiveness)
                {
                    shotTypeLeft = 3;
                    shotTypeRight = 3;
                }
                else
                {
                    shotTypeLeft = 0;
                    shotTypeRight = 0;
                }

                if (totalHealth < playerHealth - (200 * aggressiveness) || sinkAmount > 500 - (200 * (1 - aggressiveness)))
                    return 2;
                if (CrewNum > player.CrewNum + (50 * (1 - aggressiveness)) + 10)
                {
                    return 3;
                }
                return 1;
            }
            if(state == 2)
            {
                float totalHealth = 0;
                for (int i = 0; i <= 6; i++)
                {
                    totalHealth += hitBoxArray[i].Health;
                }
                if (totalHealth >= 600 + 100 * aggressiveness)
                    return 0;
                    

                isRepairing = true;
                return 2;
            }
            if(state == 3)
            {
                shotTypeLeft = 4;
                shotTypeRight = 4;

                //Leave this state when boarding is complete
                if (!boardingStarted && (isBoarded || isBoarding))
                {
                    boardingStarted = true;
                }
                //if(boardingStarted && !(isBoarded|| isBoarding))
                //{
                //    boardingStarted = false;
                //    return 0;
                //}
                if (boardingStarted)
                {
                    if (CrewNum - player.CrewNum < -20 * aggressiveness)
                    {
                        cutBoardingRopes();
                        return 0;
                    }
                    else
                    {
                        shotTypeLeft = 3;
                        shotTypeRight = 3;
                    }

                    if (!(isBoarded || isBoarding))
                    {
                        boardingStarted = false;
                        return 0;
                    }
                }
                else if (isBoarded || isBoarding)
                {
                    boardingStarted = true;
                }
                return 3;
            }
            return 0;
        }

        private void Death()
        {
            GameSetup.BackToTitle("You win.");
        }

        /// <summary>
        /// Code to run each tick
        /// </summary>
        private void Tick()
        {
            //DEBUG
            GM.textM.Draw(FontBank.arcadePixel, 
                "Hull Front  " + hitBoxHullFront.Health + "~Hull Back   " + hitBoxHullBack.Health + "~Hull Left   " + hitBoxHullLeft.Health + "~Hull Right  " + hitBoxHullRight.Health +
                "~Sail Front  " + hitBoxSailFront.Health + "~Sail Middle " + hitBoxSailMiddle.Health + "~Sail Back   " + hitBoxSailBack.Health, GM.screenSize.Width - 100, 100, TextAtt.TopRight);
            GM.textM.Draw(FontBank.arcadePixel, "Crew: " + CrewNum, GM.screenSize.Width - 150, 50, TextAtt.TopRight);
            GM.textM.Draw(FontBank.arcadePixel, "State: " + state, GM.screenSize.Width - 150, 75, TextAtt.TopRight);

            if (GM.eventM.Elapsed(stateTick))
            {
                state = StateMachine();
            }

            if(state == 1 || state == 3) //Attacking or boarding
            {
                sailAmount = 2;
                Point movePoint = Point.Zero;

                //Direction vectors for front and right of player
                Vector2 playerFront = RotationHelper.Direction2DFromAngle(player.RotationAngle, 0);
                Vector2 playerRight = RotationHelper.Direction2DFromAngle(player.RotationAngle, 90);
                Vector2 directionToPlayer = Vector2.Normalize(Position2D - player.Position2D);

                ////1 if front of ships opposite, -1 otherwise.
                //float angleBetweenFront = RotationHelper.BearingTo(RotationHelper.Direction2DFromAngle(RotationAngle, 0), playerFront, DirectionAccuracy.free, 0) - RotationAngle;
                //int frontOpposite = -1;
                //if (angleBetweenFront > 135 || angleBetweenFront < -135)
                //{
                //    frontOpposite = 1;
                //}

                float angleFromPlayer = RotationHelper.BearingTo(Position2D, player.Position2D, DirectionAccuracy.free, 0) - RotationAngle;
                if (angleFromPlayer < -180)
                {
                    angleFromPlayer += 360;
                }

                //1 if side of ships opposite, -1 otherwise.
                int playerOnRight = -1;
                float angleBetweenSide = RotationHelper.BearingTo(Vector2.Normalize(Position2D - RotationHelper.Direction2DFromAngle(RotationAngle,0 )), playerRight, DirectionAccuracy.free, 0) - RotationAngle;
                if (angleFromPlayer > 0)
                {
                    playerOnRight = 1;
                }
                
                sideFaceSprite.Position2D = Position2D;
                sideFaceSprite.RotationAngle = RotationAngle + (90 * playerOnRight);
                float alignment = RotationHelper.AngularDirectionTo(sideFaceSprite, player.Position, 0, false);
                
                bool readyToFire = false;
                if (alignmentLastTick != alignment/* && ((angleFromPlayer < -80 && angleFromPlayer > -100) || angleFromPlayer > 80 && angleFromPlayer < 100)*/)
                {
                    readyToFire = true;
                }
                alignmentLastTick = alignment;

                if ((state == 1 && Vector2.DistanceSquared(Position2D, player.Position2D) > 100000) || (state == 3 && Vector2.DistanceSquared(Position2D, player.Position2D) > 125000)) //Out of range
                {
                    movePoint = PointHelper.PointFromVector2(player.Position2D + (playerRight * 100 * playerOnRight));
                }
                else //Within range
                {
                    sailAmount = 1;
                    //movePoint = PointHelper.PointFromVector2(player.Position2D + (playerRight * 100) + (playerFront * 1000 * frontOpposite));
                    movePoint = PointHelper.PointFromVector2(Position2D + 100 * RotationHelper.Direction2DFromAngle(RotationAngle, 90 * alignment * playerOnRight));

                    if (readyToFire || isBoarded || isBoarding)
                    {
                        if (angleFromPlayer > 10 && angleFromPlayer < 170)
                        {
                            Fire(false, shotTypeLeft);
                        }
                        else if (angleFromPlayer < -10 && angleFromPlayer > -170)
                        {
                            Fire(true, shotTypeRight);
                        }
                    }
                }
                //DEBUG
                GM.textM.Draw(FontBank.arcadePixel, "Alignment" + angleFromPlayer + "~Rotation" + RotationAngle, GM.screenSize.Width - 150, 10, TextAtt.TopRight);

                MoveToPoint(movePoint);
            }
            if(state == 2) //Retreating
            {
                MoveToPoint(PointHelper.PointFromVector2(Position2D + (Vector2.Normalize(Position2D - player.Position2D) * 200)));
            }
            if (state == 3)//Boarding
            {
                //if (CrewNum - player.CrewNum < -20 * aggressiveness)
                //{
                //    cutBoardingRopes();
                //}
            }
        }
    }
}