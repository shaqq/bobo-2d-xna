using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTBD.ScreenManager;
using System.IO;
using System.Xml.Serialization;


namespace BTBD.GameScreens
{
    class PauseScreen : MenuScreen
    {
        //private SaveGameData data;
        public PauseScreen(MainGameScreen screen)
            : base("Pause")
        {
            MenuItem resumeItem = new MenuItem("Resume");
            MenuItem quitItem = new MenuItem("Quit");

            resumeItem.Selected += ResumeGameEvent;
            quitItem.Selected += QuitSelected;

            MenuItems.Add(resumeItem);
            MenuItems.Add(quitItem);
            pausedScreen = screen;
        }

        MainGameScreen pausedScreen;

        void QuitSelected(object sender, EventArgs e)
        {
            ScreenManager.QuitToScreen(new MainMenuScreen());
        }

        void ResumeGameEvent(object sender, EventArgs e)
        {
            this.Exit();
            pausedScreen.UnpauseEvent(this, new EventArgs());
        }        
    }
}
