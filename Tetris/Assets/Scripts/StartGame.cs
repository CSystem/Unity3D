using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

    public Texture texture;
    public GameObject scene;

	// Use this for initialization
	void Start () {

        texture = (Texture)Resources.Load("image/Mammooth");
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {

        GUI.DrawTexture(new Rect(230,20, 128, 128),texture,ScaleMode.ScaleToFit,true,0);


        if (GUI.Button(new Rect(230, 200, 120, 50), "Start Game")) {

            Application.LoadLevel("Tetris");
        
        }


    
    }

}
