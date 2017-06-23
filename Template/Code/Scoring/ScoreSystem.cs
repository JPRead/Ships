using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine7;
/*
    step 1 : just talk through this (already provided)
*/
namespace Template
{
    /// <summary>
    /// holds highscores
    /// </summary>
    public class ScoreSystem : HighScore
    {
        /// <summary>
        /// holds the high score table
        /// </summary>
        public ScoreSystem() 
            :base(scoreOrder.HighScore) 
        {
            
        }
            
        /// <summary>
        /// returns a default set of scores
        /// </summary>
        /// <returns></returns>
        internal static ScoreSystem Default()
        {
            ScoreSystem s = new ScoreSystem();
            s.DefineColumn("Score", 999999);
            s.DefineColumn("Name", "XXXX");

            s.NewScore(1000, "DAVE");
            s.NewScore(800, "KIA");
            s.NewScore(600, "JEN");
            s.NewScore(400, "CHAI");

            return s;
        }
        /// <summary>
        /// adds a new score to the score table
        /// </summary>
        /// <param name="score">time to add</param>
        /// <param name="name">name of player</param>
        public void NewScore(int score, string name)
        {
            List<object> ns = new List<object>();
            ns.Add(score);
            ns.Add(name);
            AddScore(ns);
        }

        /// <summary>
        /// displays the scores using sprites
        /// </summary>
        public void ShowScores()
        {
            //get length of heading text
            GM.textM.DrawAsSprites(
                FontBank.gradius,
                "TOP SCORES",
                GM.screenSize.Center.X,
                GM.screenSize.Top + 300,
                TextAtt.Top,
                1000,
                null);

            GM.textM.DrawAsSprites(
                FontBank.gradius,
                this.ToString(),
                GM.screenSize.Center.X - 120,
                GM.screenSize.Top + 400, 
                TextAtt.TopLeft,
                1000,
                null);
        }

    }
}
