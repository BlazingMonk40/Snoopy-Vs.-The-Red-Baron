using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class Grid_3D
{
    private int width;
    private int height;
    private int depth;
    private int gridSize;

    private int[,,] gridArray;
    GameObject gameObject;

    public Grid_3D(GameObject gameObject, int width, int height, int depth, int gridSize)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.gridSize = gridSize;
        this.gameObject = gameObject;

        gridArray = new int[width, depth, height];

        int xx = (int)gameObject.transform.position.x;
        int yy = (int)gameObject.transform.position.y;
        int zz = (int)gameObject.transform.position.z;
        Vector3 offset = new Vector3(xx, yy, zz) - new Vector3(gridSize*width, gridSize*height, gridSize*depth) * .5f;

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
           //Debug.DrawLine(, Color.cyan, 100f);
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                //Debug.DrawLine(, Color.cyan, 100f);
                for (int z = 0; z < gridArray.GetLength(2); z++)
                {
                    Debug.DrawLine(GetWorldPosition(0, y, z) + offset, GetWorldPosition(x + 1, y, z) + offset, Color.blue, 100f);
                    Debug.DrawLine(GetWorldPosition(x, 0, z) + offset, GetWorldPosition(x, y + 1, z) + offset, Color.green, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y, 0) + offset, GetWorldPosition(x, y, z + 1) + offset, Color.red, 100f);
                    
                    //Debug.Log(x + ", " + y + ", " + z);
                    //Debug.DrawLine(GetWorldPosition(this.width, this.height, z), GetWorldPosition(this.width, this.height, this.depth), Color.cyan, 100f);
                }
            }
        }

    }

    private Vector3 GetWorldPosition(int x, int y, int z)
    {
        return new Vector3(x, y, z) * gridSize;
    }

    //public static TextMesh CreateWorldText(Transform parent)
}
