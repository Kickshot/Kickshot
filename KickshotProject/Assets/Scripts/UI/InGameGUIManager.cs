using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameGUIManager : MonoBehaviour {
    public bool activeMenu { get; set; } // this tells us whether the win/lose screens are active so we can prevent pausing if so.
}
