using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
public class manager : MonoBehaviour {

    
    
    
    public static GameObject[] array;       //block
    public static GameObject obj;
    public bool isDrop = false;
    public int _fieldWidth = 16;
    public int _fieldHeight = 20;           
    public int maxBlockSize = 5;            //the maxSize of the block
    public Transform cube;                  //box
    public int rowsClearedToSpeedup = 10;
    public float blockNormalSpeed = 2.0f;	//方块正常速度
    public float blockDropSpeed = 30.0f;	//方块下降速度
    public float blockMoveDelay = .1f;	//方块移动延迟
    public float speedupAmount = .5f;			//加速数量


    private bool[ ,] field;
    private int fieldWidth;
    private int fieldHeight;
    private Transform[] cubeReferences;
    private int[] cubePositions;
    static manager use;
    private int rowsCleared = 0;

	// Use this for initialization
	void Start () {
        //load the cubePrefab to the GameObject[] array
       // array = (GameObject[])Resources.LoadAll("prefab/");

        if (!use)
        {

            use = this;

        }
        else {

            return;
        
        }



        fieldWidth = _fieldWidth + maxBlockSize * 2; //30
        fieldHeight = _fieldHeight + maxBlockSize; //18

        //save the 
        field = new bool[fieldWidth,fieldHeight];

        for(int i = 0; i < fieldWidth - 1; i++) {
        
            for(int j = 0; j < fieldHeight - 1; j++) {

                field[i, j] = true;
                field[fieldWidth - 1 - j, i] = true;
            
            }
            
        
        }

        for(int i = 0; i < fieldWidth; i++) {

            field[i, 0] = true;
        
        }


        cubeReferences = new Transform[fieldWidth * fieldHeight];
        cubePositions = new int[fieldWidth * fieldHeight];
	}

    void SpawnBlock() {

        Instantiate(array[Random.Range(0, array.Length)]);

    
    }


    int FieldHeight() {

        return fieldHeight;
    
    }

    int FieldWidth() {

        return fieldWidth;
    
    }

    bool CheckBlock(bool[,] blockMatrix, float xPos, float yPos) {

        var size = blockMatrix.GetLength(0);

        for (var y = size - 1; y >= 0; y--) {

            for (var x = 0; x < size; x++) {

                if (blockMatrix[x, y]) {

                    var first = xPos + x;
                    var last = yPos - y;

                    Instantiate(cube,new Vector3(first, last), Quaternion.identity);
                    field[(int)first, (int)last] = true;
                
                }

            }
        
        }


            return false;
    }


    IEnumerator SetBlock(bool[,] blockMatrix, float xPos, float yPos) {

        int size = blockMatrix.GetLength(0);

        for (var y = 0; y < size; y++) {
            for (var x = 0; x < size; x++) {
                if (blockMatrix[x, y]) { 
                    var first = xPos + x;
                    var last = yPos - y;

                    Instantiate(cube, new Vector3(first, last, 0.0f), Quaternion.identity);
                    field[(int)first, (int)last] = true;
                    
                }
            
            }
            
        
        }
        
        yield return CheckRows((int)(yPos - size), size);
        SpawnBlock(); 
    }

   IEnumerator CheckRows(int yStart, int size) {

        int x,y;

        if (yStart < 1) yStart = 1;
        for ( y = yStart; y < yStart + size; y++) {
            for ( x = maxBlockSize; x < fieldWidth - maxBlockSize; x++) {
                if (!field[x, y]) break;
            
            }

            if (x == fieldWidth - maxBlockSize) { 
                
                yield return CollapseRows(y);
                y--;
            
            }

        }

    }


    IEnumerator CollapseRows(int yStart) {

        int x, y;
        //move rows down in array, which effectively deletes the current row (yStart)
        for (y = yStart; y < fieldHeight - 1; y++) {

            for (x = maxBlockSize; x < fieldWidth - maxBlockSize; x++) {

                field[x, fieldHeight - 1] = false;
            
            }
        }
            //make sure top line is cleared
            for (x = maxBlockSize; x < fieldWidth - maxBlockSize; x++) {
                field[x, fieldHeight - 1] = false;
                    
            }
        
            //Destroy on-screen cubes on the deleted row, and store references to cubes that are above it

         var cubes = GameObject.FindGameObjectsWithTag("Cube");
         var cubesToMove = 0;
         
        
         foreach(var cube in cubes ){
             if (cube.transform.position.y > yStart) {
                 cubePositions[cubesToMove] = (int)cube.transform.position.y;
                 cubeReferences[cubesToMove++] = cube.transform;

             }
             else if (cube.transform.position.y == yStart) {
                 Destroy(cube);
             }
         
         }

        //move the appropriate cubes down one square
        //the third parameter in Mathf.lerp is clamped to 1.0,which makes the transform.position.y positioned exactly when done,
        //which is important for the game logic (see the code just above)

         float t = 0.0f;
         while (t <= 1.0) {
             t += (float)(Time.deltaTime * 5.0);
             for (int i = 0; i < cubesToMove; i++) {
                 
                 float yPos = Mathf.Lerp((float)cubePositions[i], (float)(cubePositions[i] - 1), t);
                 float zPos = cubeReferences[i].position.z;
                 float xPos = cubeReferences[i].position.x;

                 cubeReferences[i].transform.position = new Vector3(xPos,yPos,zPos);
                 
                 //this statement can't be used
                 // cubeReferences[i].transform.position.y = Mathf.Lerp((float)cubePositions[i], (float)(cubePositions[i] - 1), t);
             
                 
             }
             yield return 0;
         
         }
        
        //Make blocks drop faster when enough rows are cleared
         if (++rowsCleared == rowsClearedToSpeedup) {
             blockNormalSpeed += speedupAmount;
             rowsCleared = 0;
            
         
         }


    }

    void GameOver() {

        Debug.Log("Game Over!");
    
    }

    void PrintField() {

        var fieldChars = "";
        for (int y = fieldHeight - 1; y >= 0; y--) {
            for (int x = 0; x < fieldWidth; x++) {
                fieldChars += field[x, y] ? "1" : "0";
            
            }
            fieldChars += "\n";
        }

        Debug.Log(fieldChars);
    }

	// Update is called once per frame
	void Update () {

        if (isDrop) {
            //use random to get a gameobject and instantiate 
            //obj = (GameObject)GameObject.Instantiate(array[1]);
           // obj.transform.position = new Vector3(1,-30,4);    
        
        }



	}



}
