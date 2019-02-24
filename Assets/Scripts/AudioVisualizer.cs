using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{

    //An AudioSource object so the music can be played  
    private AudioSource aSource;
    //A float array that stores the audio samples  
    private float[] samples = new float[256];
    //A renderer that will draw a line at the screen  
    private LineRenderer lRenderer;
    //A reference to the cube prefab  
    public GameObject cube;
    //The transform attached to this game object  
    private Transform goTransform;
    //The position of the current cube. Will also be the position of each point of the line.  
    private Vector3 cubePos;
    //An array that stores the Transforms of all instantiated cubes  
    private Transform[] cubesTransform;
    //The velocity that the cubes will drop  
    private Vector3 gravity = new Vector3(0.0f, 0.5f, 0.0f);

    private GameObject[] tempCube;

    public const float Y_MAX = 50f;

    public Gradient gradient;

    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;

    public Camera camera;

    void Awake()
    {
        //Get and store a reference to the following attached components:  
        //AudioSource  
        this.aSource = GetComponent<AudioSource>();
        //LineRenderer  
        this.lRenderer = GetComponent<LineRenderer>();
        //Transform  
        this.goTransform = GetComponent<Transform>();
    }

    void Start()
    {
        //The line should have the same number of points as the number of samples  
        lRenderer.SetVertexCount(samples.Length);

        camera.backgroundColor = Color.black;
        // lRenderer.material.color = Color.white;
        //LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lRenderer.material = new Material(Shader.Find("Sprites/Default"));

        //The cubesTransform array should be initialized with the same length as the samples array  
        cubesTransform = new Transform[samples.Length];
        //Center the audio visualization line at the X axis, according to the samples array length  
        goTransform.position = new Vector3(-samples.Length / 2, goTransform.position.y, goTransform.position.z);

        tempCube = new GameObject[samples.Length];

        //Create a temporary GameObject, that will serve as a reference to the most recent cloned cube  
        //public GameObject tempCube;

        //For each sample  
        for (int i = 0; i < samples.Length; i++)
        {
            //Instantiate a cube placing it at the right side of the previous one  
            tempCube[i] = (GameObject)Instantiate(cube, new Vector3(goTransform.position.x + i, goTransform.position.y, goTransform.position.z), Quaternion.identity);
            //Get the recently instantiated cube Transform component
            cubesTransform[i] = tempCube[i].GetComponent<Transform>();
            //Make the cube a child of this game object  
            cubesTransform[i].parent = goTransform;
        }

        #region
        //gradient = new Gradient();
        //colorKey = new GradientColorKey[2];
        //colorKey[0].color = Color.red;
        //colorKey[0].time = 0.0f;
        //colorKey[1].color = Color.blue;
        //colorKey[1].time = 1.0f;

        //// Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        //alphaKey = new GradientAlphaKey[2];
        //alphaKey[0].alpha = 1.0f;
        //alphaKey[0].time = 0.0f;
        //alphaKey[1].alpha = 0.0f;
        //alphaKey[1].time = 1.0f;

       //gradient.SetKeys(colorKey, alphaKey);


        #endregion
    }

    void Update()
    {
        //Obtain the samples from the frequency bands of the attached AudioSource  
        aSource.GetSpectrumData(this.samples, 0, FFTWindow.BlackmanHarris);

        float totalDisplacement = 0f;

        //For each sample  
        for (int i = 0; i < samples.Length; i++)
        {
            //Debug.Log("Samples:  [" + i + "]: " + samples[i]);

            /*Set the cubePos Vector3 to the same value as the position of the corresponding 
             * cube. However, set it's Y element according to the current sample.*/
            cubePos.Set(cubesTransform[i].position.x, Mathf.Clamp(samples[i] * (50 + i * i), 0, Y_MAX * 1.25f), cubesTransform[i].position.z);

            //If the new cubePos.y is greater than the current cube position  
            if (cubePos.y >= cubesTransform[i].position.y)
            {
                //Set the cube to the new Y position  
                cubesTransform[i].position = cubePos;
            }
            else
            {
                //The spectrum line is below the cube, make it fall  
                cubesTransform[i].position -= gravity;
            }

            #region
            Color tempColor = new Color(0f, 0f, 0f , 0f);

            //if (cubesTransform[i].position.y > 1)
            //{
            //    Debug.Log("Position: " + cubesTransform[i].position.y);
            //}

            //if (i >= 0 && i < 22)
            //{
            //    tempColor.r = tempColor.a = cubesTransform[i].position.y / Y_MAX; 
            //}

            //else if (i >= 22 && i < 42)
            //{
            //    tempColor.g = tempColor.a = cubesTransform[i].position.y / Y_MAX;
            //}

            //else
            //{
            //    tempColor.b = tempColor.a = cubesTransform[i].position.y / Y_MAX;
            //}
            tempColor = gradient.Evaluate(cubesTransform[i].position.y / Y_MAX);
            tempCube[i].GetComponent<Renderer>().material.color = tempColor;
            #endregion

            /*Set the position of each vertex of the line based on the cube position. 
             * Since this method only takes absolute World space positions, it has 
             * been subtracted by the current game object position.*/
            //lRenderer.SetPosition(i, cubePos - goTransform.position);
            //goTransform.position = new Vector3(goTransform.position.x, goTransform.position.y * Y_MAX, goTransform.position.z);
            totalDisplacement += cubesTransform[i].position.y;
            Vector3 tempVector = new Vector3(cubePos.x - goTransform.position.x, (cubePos.y - goTransform.position.y) * Y_MAX / 5, cubePos.z - goTransform.position.z);
            lRenderer.SetPosition(i, tempVector);           
        }
    
        lRenderer.GetComponent<Renderer>().material.color = gradient.Evaluate((totalDisplacement / Y_MAX) / samples.Length);
            // camera.backgroundColor = gradient.Evaluate((totalDisplacement / Y_MAX) / samples.Length);
        // lRenderer.material.color = gradient.Evaluate((totalDisplacement / Y_MAX) / samples.Length);
    }
}
