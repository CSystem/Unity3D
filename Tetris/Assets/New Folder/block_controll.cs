using UnityEngine;
using System.Collections;

public class block_controll : MonoBehaviour {

    public string[] block;


    private bool[,] blockMatrix;
    private float fallSpeed;
    private float xPosition;
    private float yPosition;
    private int size;
    private int halfSize;
    private float halfSizeFloat;
    private bool dropped = false;

    float xPos;
    float yPos;
    float zPos;

	// Use this for initialization
	void Start () {
	
        //Sanity checking
        size = block.Length;

        //var width = block[0].Length;




        halfSize = size / 2;
        halfSizeFloat = (float)(size * .5); //halfsize is an integer for the array,but we need a float for position

        //convert block string array from the inspector to a bool 2D array for easier usage
        blockMatrix = new bool[size, size];

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                if (block[y][x] == "1"[0]) {
                    blockMatrix[x, y] = true;
                    var MBlock = Instantiate(manager.use.cube, new Vector3(x - halfSizeFloat, (size - y) + halfSizeFloat - size, 0.0f),Quaternion.identity) as Transform;
                    MBlock.parent = transform;
                
                }
                
            
            }
        
        
        }

        //for blocks with even sizes, we just add 0,but odd sizes need .5 added to the position to work right
        xPos = (float)(15 + (size % 2 == 0 ? 0.0 : .5));
        yPos = transform.position.y;
        zPos = transform.position.z;

        transform.position = new Vector3(xPos, yPos, zPos);

        xPosition = transform.position.x - halfSizeFloat;
        yPosition = 18 - 1;
        
        //
        xPos = transform.position.x;
        yPos = yPosition - halfSizeFloat;
        zPos = transform.position.z;

        transform.position = new Vector3(xPos, yPos, zPos);
        
        //fallSpeed = manager.use.blockNormalSpeed;
        fallSpeed = 2.0f;

        //check to see if this block would overlap existing blocks , in which case the game is over
        if (manager.use.CheckBlock(blockMatrix, xPosition, yPosition)) {

            manager.use.GameOver();
            return;
        
        }

        

	}


    IEnumerator CheckInput() {

        while (true) {

            var input = Input.GetAxis("Horizontal");
            if (input < 0.0) {

                yield return MoveHorizontal(-1);

            } else if(input > 0.0){

                yield return MoveHorizontal(1);

            }

            if (Input.GetButtonDown("Rotate")) {

                RotateBlock();
            
            }

            if (Input.GetButtonDown("Drop")) {
                
                fallSpeed = manager.use.blockDropSpeed;
                dropped = true;
                break;
            
            }

            yield return 0;
        }
    
    }


    IEnumerator MoveHorizontal(int dir) { 
    
        //check to see if block could be moved in the desired direction
        if (!manager.use.CheckBlock(blockMatrix, xPosition, yPosition)) {
            float xPos = transform.position.x;
            float yPos = transform.position.y;
            float zPos = transform.position.z;
            
            xPos += dir;
            xPosition += dir;

            transform.position = new Vector3(xPos,yPos,zPos);

            yield return (manager.use.blockMoveDelay);
            
        
        }
    
    }


    void RotateBlock() { 
        //rotate matrix 90 to the right and store the results in a temporary matrix
        var tempMatrix = new bool[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                tempMatrix[y, x] = blockMatrix[x, (size - 1) - y];
            
            }
        
        
        }
    
    //if the rotated block doesn't overlap existing blocks, copy the rotated matrix back and rotate on-screen block to match
        if (!manager.use.CheckBlock(tempMatrix, xPosition, yPosition)) {
            System.Array.Copy(tempMatrix, blockMatrix, size * size);
            transform.Rotate(Vector3.forward * - 90.0f);
        
        }



    }


	// Update is called once per frame
	void Update () {
	
	}
}
