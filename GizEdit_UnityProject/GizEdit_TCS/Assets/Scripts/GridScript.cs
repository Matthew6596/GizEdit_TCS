using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    public int gridSize = 10;
    public GameObject gridPrefab;
    public Transform player;

    float gridSpacing = 0.25f;

    Vector3 gridCenter;

    // Start is called before the first frame update
    void Start()
    {
        gridSpacing = gridPrefab.transform.localScale.x*10;

        //Create Entire Grid
        float gx = 0, gz = 0;
        for(int i=0; i<gridSize; i++)
        {
            for(int j=0; j<gridSize; j++)
            {
                GameObject _g = gridPrefab;
                _g.transform.position = new Vector3(gx, 0, gz);
                Instantiate(_g,transform);
                gx += gridSpacing;
            }
            gz += gridSpacing;
            gx = 0;
        }

        gridCenter = transform.position;
        transform.position = new Vector3((-gridSize*gridSpacing/2)+1.25f, 0, (-gridSize * gridSpacing / 2) + 1.25f);
    }

    // Update is called once per frame
    void Update()
    {
        //Make grid loop/follow player
        if (player.position.x > gridCenter.x + gridSpacing)
        {
            transform.Translate(gridSpacing,0,0);
            gridCenter.x += gridSpacing;
        }else if (player.position.x < gridCenter.x - gridSpacing)
        {
            transform.Translate(-gridSpacing, 0, 0);
            gridCenter.x -= gridSpacing;
        }
        else if (player.position.z < gridCenter.z - gridSpacing)
        {
            transform.Translate(0, 0, -gridSpacing);
            gridCenter.z -= gridSpacing;
        }
        else if (player.position.z > gridCenter.z + gridSpacing)
        {
            transform.Translate(0, 0, gridSpacing);
            gridCenter.z += gridSpacing;
        }

    }
}
