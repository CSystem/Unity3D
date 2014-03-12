using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	// Use this for initialization
	void Start () {
	


	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {

        GUIStyle font = new GUIStyle();
        font.fontSize = 40; //set the font size
        font.normal.background = null;
        font.normal.textColor = Color.black;

        GUI.Label(new Rect(220, 50, 500,400),"GameOver",font);


        if (GUI.Button(new Rect(230, 200, 120, 50), "Replay")) {

            Application.LoadLevel("Tetris");
        
        }
        else if (GUI.Button(new Rect(230, 280, 120, 50), "Quit")) {

            Application.Quit();    
        
        }

    }
}
