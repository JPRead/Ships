using System;
using Engine7;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
/*
step 1 create menuInitial to define display attributes for selection deselection
step 2 create menu list
step 3 add basic left/right control scheme and delay
step 4 add marker
step 5 add initials display and select method
*/
namespace Template
{
    /// <summary>
    /// generates the score entry menu and associated controls
    /// </summary>
    internal class ScoreEntry : Menu
    {
        /// <summary>
        /// event for controller
        /// </summary>
        private Event evControl;
        /// <summary>
        /// input delay timer
        /// </summary>
        private Event tiDelay;
        /// <summary>
        /// storage for intials
        /// </summary>
        string initials = "";
        /// <summary>
        /// constructor
        /// </summary>
        public ScoreEntry()
            :base("scoreentry", MenuType.sprite)
        {
            GM.menuM.AddMenu(this);
            //< is rub character and > is end character
            string choice = FontBank.ALPHA + " <>" ;

            List<Sprite> alpha = GM.textM.CreateCharacterSpritesList(FontBank.gradius, choice , 1.5f, Color.White);

            //position each of the sprites across the screen and add to menu
            for (int i = 0; i < alpha.Count; i++)
            {
                alpha[i].X = 30 + i * 40;
                alpha[i].Y = GM.screenSize.Bottom - 100;

                alpha[i].Layer = RenderLayer.hud;
                AddItem(new MenuInitial(alpha[i]));
            }

            //create a tag to highlight selection
            TagFollowSelection(new Marker());

            Show();


            GM.eventM.AddEvent(evControl = new Event(GM.eventM.MaximumRate, "name controller", Controls));
            GM.eventM.AddTimer(tiDelay = new Event(0.1f, "input delay"));

        }

        /// <summary>
        /// input controls for the menu
        /// </summary>
        private void Controls()
        {
            if (GM.inputM.Down(new KeyPadPair(Keys.Left, 1, Buttons.LeftThumbstickLeft)) && GM.eventM.Elapsed(tiDelay))
            { 
                Previous();
            }
            if (GM.inputM.Down(new KeyPadPair(Keys.Right, 1, Buttons.LeftThumbstickRight)) && GM.eventM.Elapsed(tiDelay))
            {
                Next();
            }
            if (GM.inputM.Pressed(new KeyPadPair(Keys.LeftControl, 1, Buttons.A)))
            {
                ChoiceMade();
            }
            GM.textM.Draw(FontBank.gradius, initials, GM.screenSize.Center.X, 200, TextAtt.Centred);
        }

        /// <summary>
        /// activated if selection is activated
        /// </summary>
        private void ChoiceMade()
        {
            switch (SelectedName)
            {
                
                default://add initial if space available
                    if (initials.Length < 4)
                        initials += SelectedName;
                    break;
                case "<": //rub
                    initials = StringHelper.ReduceLengthBy(initials, 1);
                    break;
                case ">": //end
                    //tidy up and broadcast initials
                    GM.eventM.Remove(evControl);
                    GM.eventM.Remove(tiDelay);
                    MessageBus.Instance.BroadcastMessage(ExtraMessageTypes.NameEntryComplete, initials);
                    break;
            }
        }
    }
}