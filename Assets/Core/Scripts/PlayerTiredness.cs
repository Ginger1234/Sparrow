using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using managers;

namespace ui_bars
{
    public class PlayerTiredness : Bar
    {
        public void Sleep(float sleepRestore)
        {
            MainGameManager.Instance.Player.Sleep(true);
            MainGameManager.Instance.MainMenu.SlowDownGame();
            Increase(sleepRestore);
        }
    }
}
