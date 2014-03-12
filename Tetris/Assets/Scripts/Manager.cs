using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {
	
	public GameObject[] blocks;                         //save the block
	public Transform cube;                              //组成方块的小cube
	public Transform leftWall;                          //左边的墙
	public Transform rightWall;                         //右边的墙
	public int maxBlockSize = 4;                        //方块最大的尺寸   长条最大
	public int _fieldWidth = 10;                        //实际空场景的宽度
    public int _fieldHeight = 13;                       //实际空场景的高度  
	public float blockNormalFallSpeed = 2f;             //方块正常下落的速度
	public float blockDropSpeed = 30f;                  //按下下方向键后方块的下落速度
	public Texture cubeTexture;                         //cube的贴图
	
	private int fieldWidth;                             //墙加厚之后的场景宽度
    private int fieldHeight;                            //墙加厚之后的场景高度
	private bool[,] fields;                             //存储墙加厚之后整个场景的坐标
	private int[] cubeYposition;                        //存储所有坐标点的Y坐标
	private Transform[] cubeTransforms;                 //存储所有坐标点的物体
	private int clearTimes;                             //清除时间
	private float addSpeed = .3f;                       //方块下落速度的增量
	private int TimeToAddSpeed = 10;                    //到达这个时间就会增加方块的下落速度
	
	private int Score = 0;                              //当前分数
	private int Highest = 0;                            //最高分数
	private int blockRandom;                            //用来随机方块的随机数
	private GameObject nextBlock;                       //下一个方块对象
	private Block nextB;                                //下一个方块
	private int nextSize;                               //下一个方块的尺寸
	private string[] nextblock;                         //存储下一个方块的形状的字符串数组
	
	public static Manager manager;                      //实例一个Manager类，来实现单例模式

	// Use this for initialization
	void Start () {
	
		if (manager == null){
            //如果manager为空就实例化当前对象
			manager = this;
		}
		
		if (PlayerPrefs.HasKey("Highest")){
			Highest = PlayerPrefs.GetInt("Highest");
		}
		else{
			PlayerPrefs.SetInt("Highest", 0);
		}

        //改变地板的颜色
        GameObject.Find("floor").renderer.material.color = Color.grey;

        //产生随机一个方块的随机数
		blockRandom = Random.Range(0, blocks.Length);
		
        //如果直接使用真实宽度，方块会穿墙，所以把墙的两侧也设置为不可用
        //2D view use this so We don't need  _fieldWidth + maxBlockSize 
		fieldWidth = _fieldWidth + maxBlockSize * 2;//fieldWitdh = 18
		fieldHeight = _fieldHeight + maxBlockSize; //fieldHeight = 17

        //fieldWidth = _fieldWidth;
        //fieldHeight = _fieldHeight;

        //把所有墙加厚之后的坐标点存到fields里，如果被占用有block = true，如果为可用 = false
		fields = new bool[fieldWidth, fieldHeight];

        //把所有的坐标点的Y坐标存进去
		cubeYposition = new int[fieldHeight * fieldWidth];

        //存储所有坐标点上的物体
		cubeTransforms = new Transform[fieldHeight * fieldWidth];
		
		for (int i = 0;i < fieldHeight;i++){
			
			for (int j = 0 ;j < maxBlockSize;j++){
				
                //让左边的墙加厚方块最大尺寸的大小
				fields[j, i] = true;

                //让右边的墙体加厚
				fields[fieldWidth - j - 1, i] = true;
				
			}
			
		}
		

        //让地面的所有坐标点都变为不可用
		for (int i = 0;i < fieldWidth;i++){
			fields[i, 0] = true;
		}
		
        //设定墙体的位置，在Unity上设置参数总是不对
		leftWall.position = new Vector3(maxBlockSize - .5f, leftWall.position.y, leftWall.position.z);
		rightWall.position = new Vector3(fieldWidth - maxBlockSize + .5f, rightWall.position.y, rightWall.position.z);
        //让摄像机处于中间位置
		Camera.main.transform.position = new Vector3(fieldWidth/2, fieldHeight/2, -16.0f);
		
        //调用产生方块的函数
		CreateBlock(blockRandom);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
    /*
     * 创建方块
       每一个方块都绑定了一个block脚本
       实例化会自动调用脚本进行初始化
     */
	void CreateBlock(int random){

        //随机实例化一个方块
		Instantiate(blocks[random]);
        //产生一个随机数
		blockRandom = Random.Range(0, blocks.Length);
        //产生下一个方块的String
		nextBlock = blocks[blockRandom];
		nextB = (Block)nextBlock.GetComponent("Block");
		nextSize = nextB.block.GetLength(0);
		nextblock = new string[nextSize];

        //调用脚本将nextblock实例化
		nextblock = nextB.block;
	}
	
	public int GetFieldWidth(){

        //返回墙加厚之后的函数
		return fieldWidth;
	}
	
	public int GetFieldHeight(){

        //返回上面加厚之后的场景高度
		return fieldHeight;
	}
	
	public int GetBlockRandom(){

        //获得随机方块的随机数
		return blockRandom;
	}
	

    /*
        检查方块是否碰到墙壁或是其他方块
     
     */
	public bool CheckBlock(bool [,] blockMatrix, int xPos, int yPos){

		var size = blockMatrix.GetLength(0);
		
		for (int y = 0;y < size;y++){
			for (int x = 0;x < size;x++){
                //当方块矩阵与场景矩阵有相同的坐标点并且该坐标点都有block存在则返回true，判定发生碰撞
				if (blockMatrix[y, x] && fields[xPos + x, yPos - y]){
					return true;
				}
			}
		}
		
		return false;
	}
	

	public void SetBlock(bool[,] blockMatrix, int xPos, int yPos){
		
		int size = blockMatrix.GetLength(0);
		for (int y = 0;y < size;y++){
			for (int x = 0;x < size;x++){
				if (blockMatrix[y, x]){
					Instantiate(cube, new Vector3(xPos + x, yPos - y, 5.0f), Quaternion.identity);
					fields[xPos + x, yPos - y] = true;
				}
			}
		}

        for (int i = 0; i < fieldWidth; i++) {
            for (int j = 0; j < fieldHeight; j++) {

                if (fields[i, j]) {
                    Debug.Log("x = " + i + "y = " + j );
                
                }
            
            }
        
        }
            

            StartCoroutine(CheckRows(yPos - size, size));
		
	}
	
	IEnumerator CheckRows(int yStart, int size){
		yield return null;
		if (yStart < 1)yStart = 1;
		int count = 1;
		for (int y = yStart;y < yStart + size;y++){
			int x;
			for (x = maxBlockSize;x < fieldWidth - maxBlockSize;x++){
				if (!fields[x, y]){
					break;
				}
			}
			if (x == fieldWidth - maxBlockSize){
				yield return StartCoroutine(SetRows(y));
				Score += 10 * count;
				y--;
				count++;
			}
		}
		CreateBlock(blockRandom);
	}
	IEnumerator SetRows(int yStart){
		for (int y = yStart;y < fieldHeight - 1;y++){
			for (int x = maxBlockSize;x < fieldWidth - maxBlockSize;x++){
				fields[x, y] = fields[x, y + 1];
			}
		}
		
		for (int x = maxBlockSize;x < fieldWidth - maxBlockSize;x++){
			fields[x, fieldHeight - 1] = false;
		}
		
		var cubes = GameObject.FindGameObjectsWithTag("Cube");
		int cubeToMove = 0;
		for (int i = 0;i < cubes.Length;i++){
			GameObject cube = cubes[i];
			if (cube.transform.position.y > yStart){
				cubeYposition[cubeToMove] = (int)(cube.transform.position.y);
				cubeTransforms[cubeToMove++] = cube.transform;
			}
			else if (cube.transform.position.y == yStart){
				Destroy(cube);
			}
		}
		
		float t = 0;
		while (t <= 1f){
			t += Time.deltaTime * 5f;
			for(int i = 0;i < cubeToMove;i++){
				cubeTransforms[i].position = new Vector3(cubeTransforms[i].position.x, Mathf.Lerp(cubeYposition[i], cubeYposition[i] - 1, t),
					cubeTransforms[i].position.z);
			}
		    yield return null;
		}
		
		if (++clearTimes == TimeToAddSpeed){
			blockNormalFallSpeed += addSpeed;
			clearTimes = 0;
		}
		
	}
	
	public void GameOver(){
		if (Score > PlayerPrefs.GetInt("Highest")){
			PlayerPrefs.SetInt("Highest", Score);
		}
		print("Game Over!!!");
        //load the GameOver scene
        Application.LoadLevel("GameOver");
        
	}
	
	void OnGUI(){

        

        //draw the score use GUI
		GUI.Label(new Rect(20, 30, 80, 40),"Score:");
		GUI.Label(new Rect(80, 30, 100, 40),Score.ToString());
		GUI.Label(new Rect(20, 50, 80, 40),"Highest:");
		GUI.Label(new Rect(80, 50, 80, 40),Highest.ToString());
		
        
        //draw the nextBlock use the GUI.Button
		for (int y = 0;y < nextSize;y++){
			for (int x = 0;x < nextSize;x++){
				if (nextblock[y][x] == '1'){
					GUI.Button(new Rect(20 + 30 * x, 100 + 30 * y, 30, 30), cubeTexture);
				}
			}
		}
	
        
          
         }
	
}
