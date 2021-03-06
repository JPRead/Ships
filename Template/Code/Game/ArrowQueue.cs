﻿using Microsoft.Xna.Framework;
using Engine7;

namespace Template
{
    internal class ArrowQueue
    {
        /// <summary>
        /// Number of items in the queue 
        /// </summary>
        private int count;
        /// <summary>
        /// Index of the front of the queue
        /// </summary>
        private int f;
        /// <summary>
        /// Index of the end of the queue
        /// </summary>
        private int e;
        /// <summary>
        /// The queue itself
        /// </summary>
        private Arrow[] queue;
        /// <summary>
        /// Extra arrow sprite at the end of queue
        /// </summary>
        private Sprite endArrow;

        /// <summary>
        /// Circular queue specifically made to display the player's path.
        /// </summary>
        /// <param name="origin">Origin for first arrow</param>
        /// <param name="target">Target for first arrow</param>
        public ArrowQueue(Vector2 origin, Vector2 target)
        {
            queue = new Arrow[99];
            queue[0] = new Arrow(origin, target);
            f = 0;
            e = 1;
            count = 1;

            endArrow = new Sprite();
            endArrow.Frame.Define(Tex.Triangle);
            GM.engineM.AddSprite(endArrow);
            endArrow.ScaleBoth = 0.5f;
            endArrow.Wash = Color.Beige;
            endArrow.Alpha = 0.75f;
        }

        /// <summary>
        /// Add a sprite to the queue
        /// </summary>
        /// <param name="next"></param>
        public void Enqueue(Vector2 next)
        {
            if (count < queue.Length)
            {
                count++;
                queue[e] = new Arrow(queue[e - 1].Target, next);
                endArrow.Position2D = next;
                RotationHelper.FaceDirection(endArrow, Vector2.Normalize(next - queue[e - 1].Target), DirectionAccuracy.free, 0);
                e = (e + 1) % queue.Length;
            }
        }

        /// <summary>
        /// Remove a sprite from the queue - DOES NOT RETURN
        /// </summary>
        public void Dequeue()
        {
            if(count > 0)
            {
                count--;
                queue[f].Kill();
                f = (f + 1) % queue.Length;
            }
        }

        /// <summary>
        /// Empty queue
        /// </summary>
        public void Reset()
        {
            for(int i = 0; i < queue.Length; i++)
            {
                Dequeue();
            }
            endArrow.Kill();
            queue = new Arrow[0];
        }
    }
}