using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameGUIManager : MonoBehaviour {
    public bool activeMenu { get; set; } // this tells us whether the win/lose screens are active so we can prevent pausing if so.
    public bool optionsOpen { get; set; } // this tells us whether the options menu is opened or not
    public bool scoresOpen { get; set; }
    public bool showCursor = false;

    public void OnGUI()
    {
        if(showCursor)
            Cursor.visible = true;
        else
            Cursor.visible = false;
    }
}
