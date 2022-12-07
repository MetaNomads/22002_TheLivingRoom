using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.GazeInteraction;

public class GenerateShape : MonoBehaviour
{
    // generates architecture in 1 DIRECTION base on single user position and viewpoint
    // viewpoint will be retrieved from headset focus point
    // for now viewpoint distance is arbitrarily determined from inital rotation and random focus distance


    //all public values can be easily and dynamically changed as the experience progresses

    public GameObject observer;
    float originalViewDistance;
    public float linePercent = 0.7f;
    float originalLinePercent;

    public float rectHeight = 40f;
    public float rectWidthMult = 1.5f;
    //the rect which will be divided into diamonds has proportions y x y*this value

    public int rectDivisions = 5;

    public float randomA = .05f;
    float originalRandomA;
    float[] randomsA;
    // amount of randomness compared to line length from user to viewpoint for A diamonds
    // so if == 0.05f, diamond extrusion can vary by as much as 5% of the total line length
    // could be change to be a percent of just the range of extrusion (so origin - plane for diamonds A)

    public float middleScale = 2f;

    public Material tubeMaterial;

    Vector3 origin;
    Vector3 viewpoint;
    Vector3[] startEnd;
    Vector3 planePoint;
    Vector3[] rectOutline;
    Vector3[,] crossHatch;

    Vector3[] diamondNodes;
    Vector3[] middleNodes;

    Vector3[,] diamondsA;
    Vector3[,] diamondsMirror;
    Vector3[,] diamondsMiddle;

    // Start is called before the first frame update
    void Start()
    {
        // originalViewDistance = viewDistance;
        // originalLinePercent = linePercent;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(observer.GetComponent<GazeInteractor>().getGazePoint());

        // origin = observer.transform.position;
        // forward = observer.transform.position.forward; //as the spawn point will always be the direction the user is facing

        // viewpoint = getViewPoint();
        // startEnd = getPointRange(origin, viewpoint, linePercent);
        // if (originalForward != forward || originalViewDistance != viewDistance || originalLinePercent != linePercent) {
        //     // this janky logic is because getPlaneOrigin has a call to random right now, when this is a real input this should be improved
        //     planePoint = getPlaneOrigin(startEnd);
        //     originalForward = forward;
        //     originalLinePercent = linePercent;
        //     originalViewDistance = viewDistance;
        // }
        // rectOutline = getPlaneOutline(rectHeight, rectWidthMult, planePoint, origin);
        // crossHatch = getCrossHatch(rectOutline, rectDivisions, rectHeight, rectWidthMult);
        // diamondNodes = getDiamondNodes(rectOutline, rectDivisions);
        // middleNodes = getMiddleNodes(rectOutline, rectDivisions);
        // middleNodes = sortMiddleNodes(middleNodes, rectOutline);
        // diamondsA = createFirstDiamonds(middleNodes, startEnd, planePoint,
        //                             rectDivisions, rectHeight, rectWidthMult, rectOutline);
        // diamondsMirror = createFirstDiamonds(middleNodes, startEnd, planePoint,
        //                             rectDivisions, rectHeight, rectWidthMult, rectOutline, true);
        // diamondsMiddle = createMiddleDiamonds(middleNodes, rectHeight, rectWidthMult, rectOutline,
        //                             rectDivisions, planePoint);
        // groupNodesForMesh(diamondsA, diamondsMirror, diamondsMiddle, middleNodes);
    }

    public void test()
    {
        Debug.Log("test");
    }


    public void Generate()
    {
        origin = observer.transform.position;
        viewpoint = getViewPoint();

        originalRandomA = randomA;

        startEnd = getPointRange(origin, viewpoint, linePercent);
        planePoint = getPlaneOrigin(startEnd);
        rectOutline = getPlaneOutline(rectHeight, rectWidthMult, planePoint, origin);
        crossHatch = getCrossHatch(rectOutline, rectDivisions, rectHeight, rectWidthMult);
        diamondNodes = getDiamondNodes(rectOutline, rectDivisions);
        middleNodes = getMiddleNodes(rectOutline, rectDivisions);
        middleNodes = sortMiddleNodes(middleNodes, rectOutline);

        diamondsA = createFirstDiamonds(middleNodes, startEnd, planePoint,
                                    rectDivisions, rectHeight, rectWidthMult, rectOutline);
        diamondsMirror = createFirstDiamonds(middleNodes, startEnd, planePoint,
                                    rectDivisions, rectHeight, rectWidthMult, rectOutline, true);
        diamondsMiddle = createMiddleDiamonds(middleNodes, rectHeight, rectWidthMult, rectOutline,
                                    rectDivisions, planePoint);

        groupNodesForMesh(diamondsA, diamondsMirror, diamondsMiddle, middleNodes);
    }


    // private void OnDrawGizmos () {
    //     Gizmos.color = Color.black;
    //     Gizmos.DrawSphere(origin, .2f);
    //     Gizmos.DrawSphere(viewpoint, .2f);
    //     Gizmos.DrawLine(origin, viewpoint);


    //     if (startEnd != null) {
    //         Gizmos.DrawSphere(startEnd[0], .2f);
    //         Gizmos.DrawSphere(startEnd[1], .2f);
    //     }
    //     if (planePoint != null) {
    //         Gizmos.DrawCube(planePoint, new Vector3(1, 1, 1));
    //     }
    //     if (rectOutline != null) {
    //         Gizmos.DrawLine(rectOutline[0], rectOutline[1]);
    //         Gizmos.DrawLine(rectOutline[0], rectOutline[2]);
    //         Gizmos.DrawLine(rectOutline[2], rectOutline[3]);
    //         Gizmos.DrawLine(rectOutline[1], rectOutline[3]);
    //     }
    //     if (crossHatch != null) {
    //         for (int i=0; i<rectDivisions*rectDivisions*2; i++) {
    //             Gizmos.DrawLine(crossHatch[i,0], crossHatch[i,1]);
    //         }
    //     }

    //     if (diamondNodes != null) {
    //         foreach(Vector3 node in diamondNodes) {
    //             Gizmos.DrawSphere(node, 0.2f);
    //         }
    //     }
    //     if (middleNodes != null) {
    //         float i = 0f;
    //         foreach(Vector3 node in middleNodes) {
    //             Gizmos.color = new Color(1*(i/middleNodes.Length), 0, 0);
    //             Gizmos.DrawCube(node, new Vector3(.2f, .2f, .2f));
    //             i+=1;
    //         }
    //     }

    //     if (diamondsA != null) {
    //         // Debug.Log("diamondsA.Length: "+ diamondsA.Length);
    //         for (int i=0; i<middleNodes.Length; i++){
    //             // left, bottom, right, top
    //             Vector3 left = diamondsA[i,0];
    //             Vector3 bottom = diamondsA[i,1];
    //             Vector3 right = diamondsA[i,2];
    //             Vector3 top = diamondsA[i,3];
    //             Gizmos.DrawLine(left, bottom);
    //             Gizmos.DrawLine(bottom, right);
    //             Gizmos.DrawLine(right, top);
    //             Gizmos.DrawLine(top, left);
    //         }
    //     }

    //     if (diamondsMirror != null) {
    //         Gizmos.color = Color.blue;
    //         for (int i=0; i<middleNodes.Length; i++){
    //             // left, bottom, right, top
    //             Vector3 left = diamondsMirror[i,0];
    //             Vector3 bottom = diamondsMirror[i,1];
    //             Vector3 right = diamondsMirror[i,2];
    //             Vector3 top = diamondsMirror[i,3];
    //             Gizmos.DrawLine(left, bottom);
    //             Gizmos.DrawLine(bottom, right);
    //             Gizmos.DrawLine(right, top);
    //             Gizmos.DrawLine(top, left);
    //         }
    //     }

    //     if (diamondsMiddle != null) {
    //         Gizmos.color = Color.green;
    //         for (int i=0; i<middleNodes.Length; i++){
    //             // left, bottom, right, top
    //             Vector3 left = diamondsMiddle[i,0];
    //             Vector3 bottom = diamondsMiddle[i,1];
    //             Vector3 right = diamondsMiddle[i,2];
    //             Vector3 top = diamondsMiddle[i,3];
    //             Gizmos.DrawLine(left, bottom);
    //             Gizmos.DrawLine(bottom, right);
    //             Gizmos.DrawLine(right, top);
    //             Gizmos.DrawLine(top, left);
    //         }
    //     }

    // }

    Vector3 getViewPoint() {
        // will be replaced by headset function to determine point of focus
        // or at least rotation will be determined by headset and can be fed here
        viewpoint = observer.GetComponent<GazeInteractor>().getGazePoint();
        return viewpoint;
    }

    Vector3[] getPointRange(Vector3 origin, Vector3 viewpoint, float linePercent) {
        // takes origin, viewpoint, and how much of the line we can choose from,
        // then generates range where the field can originate from
        // right now the range is equidistant from the origin to the viewpoint
        Vector3 line = viewpoint - origin;
        Vector3 start = (line * ((1-linePercent)/2)) + origin;
        Vector3 end = (line * linePercent) + start;
        Vector3[] returnArr = new [] {start, end};
        return returnArr;
    }

    Vector3 getPlaneOrigin(Vector3[] pointRange) {
        //right now doesnt have a seed
        Vector3 start = pointRange[0];
        Vector3 end = pointRange[1];
        float x = Random.Range(start.x, end.x);
        float y = Random.Range(start.y, end.y);
        float z = Random.Range(start.z, end.z);

        return new Vector3(x,y,z);
    }

    Vector3[] getPlaneOutline(float rectHeight, float rectWidthMult, Vector3 planeOrigin, Vector3 origin) {
        Vector3 lineVector = planeOrigin - origin;
        Vector3 normal = Vector3.Cross(lineVector, Vector3.up).normalized; //get perpendicular line
        float width = rectWidthMult * rectHeight;
        Vector3 right = (width/2) * normal + planeOrigin;
        Vector3 left = planeOrigin - (width/2) * normal;

        Vector3 normal90 = Vector3.Cross(lineVector, Vector3.right).normalized;
        Vector3 upLeft = (rectHeight/2) * normal90 + left;
        Vector3 upRight = (rectHeight/2) * normal90 + right;
        Vector3 downLeft = left - (rectHeight/2) * normal90;
        Vector3 downRight = right - (rectHeight/2) * normal90;
        return new [] {downLeft, downRight, upLeft, upRight};
    }

    Vector3[,] getCrossHatch(Vector3[] corners, int divs, float rectHeight, float rectWidthMult) {
        float width = rectHeight * rectWidthMult;
        float widthSection = width / divs;
        float section = rectHeight / divs;
        Vector3 upLeft = corners[2];
        Vector3 upRight = corners[3];
        Vector3 downLeft = corners[0];
        Vector3 downRight = corners[1];
        Vector3 widthDir = (upRight - upLeft).normalized;
        Vector3 heightDir = (upLeft-downLeft).normalized;
        Vector3[,] lines = new Vector3[divs*divs*2,2];
        int line = 0;
        for (int j=0; j<divs; j++) {
            for (int i =0; i<divs; i++) {
                lines[line,0] = downLeft +(j*widthSection*widthDir) + (i * section)*heightDir;
                lines[line,1] = downLeft+((j+1)*widthSection*widthDir)+ ((i+1) * section)*heightDir;
                line++;
            }
        }
         for (int j=0; j<divs; j++) {
            for (int i =0; i<divs; i++) {
                lines[line,0] = upLeft + (j*widthSection*widthDir) - (i * section)*heightDir;
                lines[line,1] = upLeft + ((j+1)*widthSection*widthDir) - ((i+1) * section)*heightDir;
                line++;
            }
        }
        return lines;
    }

    Vector3[] getDiamondNodes(Vector3[] corners, int divs) {
        int numNodes = ((divs + 1) * (divs + 1)) +  (divs * divs) - 4;
        Vector3[] nodes = new Vector3[numNodes];

        Vector3 upLeft = corners[2];
        Vector3 upRight = corners[3];
        Vector3 downLeft = corners[0];
        Vector3 downRight = corners[1];
        Vector3 width = upRight - upLeft;
        Vector3 widthDiv = width / (divs*2);
        Vector3 height = upLeft - downLeft;
        Vector3 heightDiv = height / (divs*2);

        int i = 0;
        for (int row=0; row<(divs*2+1); row++){
            for (int col=0; col<(divs*2+1); col++) {
                if (row%2 == 0){
                    // even rows
                    if (col%2==0) {
                        Vector3 node = (col*widthDiv) + (row*heightDiv) +downLeft;
                        int pos = System.Array.IndexOf(corners, node);
                        if (pos == -1) {
                            nodes[i] = node;
                            i++;
                        }
                    }
                } else {
                    //odd rows
                    if (col%2 != 0) {
                        Vector3 node = (col*widthDiv) + (row*heightDiv)+downLeft;
                        int pos = System.Array.IndexOf(corners, node);
                        if (pos == -1) {
                            nodes[i] = node;
                            i++;
                        }
                    }
                }
            }
        }

        return nodes;
    }

    Vector3[] getMiddleNodes(Vector3[] corners, int divs) {
        int numNodes = divs * (2*divs - 2);
        Vector3[] nodes = new Vector3[numNodes];

        Vector3 upLeft = corners[2];
        Vector3 upRight = corners[3];
        Vector3 downLeft = corners[0];
        Vector3 downRight = corners[1];
        Vector3 width = upRight - upLeft;
        Vector3 widthDiv = width / (divs*2);
        Vector3 height = upLeft - downLeft;
        Vector3 heightDiv = height / (divs*2);

        int i = 0;
        for (int row=0; row<(divs*2+1); row++){
            for (int col=0; col<(divs*2+1); col++) {
                if (row%2 == 0){
                    // even rows
                    if (col%2 !=0) {
                        Vector3 node = (col*widthDiv) + (row*heightDiv) +downLeft;
                        if (row != 0 && col !=0 && row != (divs*2) && col != (divs*2)) {
                            nodes[i] = node;
                            i++;
                        }
                    }
                } else {
                    //odd rows
                    if (col%2 == 0) {
                        Vector3 node = (col*widthDiv) + (row*heightDiv)+downLeft;
                        if (row != 0 && col !=0 && row != (divs*2) && col != (divs*2)) {
                            nodes[i] = node;
                            i++;
                        }
                    }
                }
            }
        }
        return nodes;
    }

    Vector3[] sortMiddleNodes(Vector3[] middleNodes, Vector3[] corners) {
        Vector3 upRight = corners[3];
        Vector3 downLeft = corners[0];

        Vector3 middle =(upRight-downLeft)/2 + downLeft;

        System.Array.Sort(middleNodes, (a, b) => compareToMiddle(middle, a).CompareTo(compareToMiddle(middle, b)));

        System.Array.Reverse(middleNodes);
        return middleNodes;
    }

    int compareToMiddle(Vector3 middle, Vector3 a){
        int aDist = (int) Vector3.Distance(middle, a);
        return aDist;
    }

    Vector3[,] createFirstDiamonds(Vector3[] sortedMiddles, Vector3[] startEnd, Vector3 planeOrigin,
                                    int divs, float rectHeight, float rectWidthMult, Vector3[] corners, bool mirror=false) {
        Vector3 wholeRange = startEnd[0] - planeOrigin;
        if (mirror) {
            wholeRange = startEnd[1] - planeOrigin;
        }
        int numDiamonds = sortedMiddles.Length;
        float perc = 1f/(float)numDiamonds;

        Vector3 upLeft = corners[2];
        Vector3 upRight = corners[3];
        Vector3 downLeft = corners[0];
        Vector3 width = (upRight - upLeft).normalized;
        Vector3 height = (upLeft - downLeft).normalized;

        Vector3 diamondHeightHalf = ((rectHeight/divs)/2) * height; //actually these should be scaled
        Vector3 diamondWidthHalf = (((rectHeight*rectWidthMult)/divs)/2) * width; //actually these should be scaled

        Vector3[,] diamondNodes = new Vector3[numDiamonds, 4];

        float lineLength = Vector3.Distance(startEnd[1], startEnd[0]);
        if (randomsA == null || randomsA.Length != numDiamonds || originalRandomA != randomA) {
            randomsA = new float[numDiamonds];
            originalRandomA = randomA;
        }

        for (int i = 0; i<numDiamonds; i++) {
            float perc_i = (perc * i);
            if (randomsA[i] == 0 || randomsA == null) {
                float random = Random.Range(lineLength * -randomA ,lineLength * randomA);
                randomsA[i] = random;
                //right now diamonds can go behind the plane, which will likely cause issues
                //with the other diamonds it's meant to connect to if they swap places
                //I will see if this is infact the case and if so change the random logic
            }
            Vector3 diamondOrigin = (perc_i * wholeRange) + randomsA[i]*wholeRange.normalized +  sortedMiddles[i];
            Vector3 left = diamondOrigin - diamondWidthHalf;
            Vector3 right = diamondOrigin + diamondWidthHalf;
            Vector3 top = diamondOrigin + diamondHeightHalf;
            Vector3 bottom = diamondOrigin -diamondHeightHalf;

            diamondNodes[i,0] = left;
            diamondNodes[i,1] = bottom;
            diamondNodes[i,2] = right;
            diamondNodes[i,3] = top;

        }

        return diamondNodes;
    }

    Vector3[,] createMiddleDiamonds(Vector3[] sortedMiddles, float rectHeight, float rectWidthMult, Vector3[] corners,
                                    int divs, Vector3 planeOrigin) {
        int numDiamonds = sortedMiddles.Length;
        Vector3 upLeft = corners[2];
        Vector3 upRight = corners[3];
        Vector3 downLeft = corners[0];
        Vector3 width = (upRight - upLeft).normalized;
        Vector3 height = (upLeft - downLeft).normalized;

        Vector3[,] diamondNodes = new Vector3[numDiamonds, 4];

        Vector3 diamondHeightHalf = ((rectHeight/divs)/2) * height * middleScale; //actually these should be scaled
        Vector3 diamondWidthHalf = (((rectHeight*rectWidthMult)/divs)/2) * width * middleScale; //actually these should be scaled

        for (int i = 0; i<numDiamonds; i++) {
            // Vector3 diamondOrigin = (perc_i * wholeRange) + randomsA[i]*wholeRange.normalized +  sortedMiddles[i];
            Vector3 diamondOrigin = sortedMiddles[i] * middleScale - planeOrigin; //almost, just dont multiply in the direction of gaze
            Vector3 left = diamondOrigin - diamondWidthHalf;
            Vector3 right = diamondOrigin + diamondWidthHalf;
            Vector3 top = diamondOrigin + diamondHeightHalf;
            Vector3 bottom = diamondOrigin -diamondHeightHalf;

            diamondNodes[i,0] = left;
            diamondNodes[i,1] = bottom;
            diamondNodes[i,2] = right;
            diamondNodes[i,3] = top;
        }
        return diamondNodes;
    }

    // Vector3[,] createSecondDiamonds(Vector3[] sortedMiddles, Vector3[] startEnd, Vector3 planeOrigin,
    //                                 int divs, float rectHeight, float rectWidthMult, Vector3[] corners) {
    // // much of the logic from the first function will be stripped into a helper function
    // // to avoid repeated code
    // }

    // Vector3[,] createThirdDiamonds(Vector3[] sortedMiddles, Vector3[] startEnd, Vector3 planeOrigin,
    //                                 int divs, float rectHeight, float rectWidthMult, Vector3[] corners) {
    // // much of the logic from the first function will be stripped into a helper function
    // // to avoid repeated code, perhaps the index will even be an argument to a more general
    // //createDiamonds function
    // }

    void groupNodesForMesh(Vector3[,] diamondsA, Vector3[,] diamondsB,
                                Vector3[,] middleDiamonds, Vector3[] middleNodes) {
        //gather the 1st, 2nd, 3rd diamonds into their respective groups (shared index)
        // and define a new game object for each with a mesh component.
        //Apply the mesh appropriately (see CreateTerrain script)

        int numDiamonds = middleNodes.Length;

        for (int i = 0; i < numDiamonds; i++) {
            string tubeName = "tube" + i.ToString();

            if(GameObject.Find(tubeName) != null) {
                GameObject oldTube = GameObject.Find(tubeName);
                Destroy(oldTube);
            }
            GameObject tube = new GameObject();
            tube.name = tubeName;

            tube.AddComponent<MeshRenderer>();
            tube.GetComponent<MeshRenderer>().material = tubeMaterial;

            tube.AddComponent<MeshFilter>();
            Mesh tubeMesh = new Mesh();
            tube.GetComponent<MeshFilter>().mesh = tubeMesh;

            Vector3[,] tubeVerts = new Vector3[3, 4];
            for (int j=0; j<4; j++) {
                tubeVerts[0,j] = diamondsA[i, j];
                tubeVerts[1,j] = middleDiamonds[i, j];
                tubeVerts[2,j] = diamondsB[i, j];
            }
            generateTubeMesh(tube, tubeVerts);
            doubleSideCollider(tube);
        }
    }

    void doubleSideCollider(GameObject tube)
    {
        tube.AddComponent<MeshCollider>();
        var mesh1 = tube.GetComponent<MeshCollider>().sharedMesh;
        var mesh2 = Instantiate(mesh1);
        var normals = mesh1.normals;
        
        for (int i = 0; i < normals.Length; ++i)
        {
            normals[i] = -normals[i];
        }

        mesh2.normals = normals;

        for (int i = 0; i < mesh2.subMeshCount; ++i)
        {
            int[] triangles = mesh2.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                int temp = triangles[j];
                triangles[j] = triangles[j + 1];
                triangles[j + 1] = temp;
            }
            mesh2.SetTriangles(triangles, i);
        }
        
        tube.AddComponent<MeshCollider>().sharedMesh = mesh1;
        tube.AddComponent<MeshCollider>().sharedMesh = mesh2;

    }

    void generateTubeMesh(GameObject tube, Vector3[,] tubeVerts) {
        //ok to have redudant vertices? try with first, else construct wall by wall?
        // Vector3[] vertices = new Vector3[4*8]; //optimally, it would just be 12?
        Vector3[] vertices = new Vector3[12];
        int k = 0;
        for (int i = 0; i<3; i++) {
            for (int h =0; h<4; h++) {
                vertices[k] = tubeVerts[i, h];
                k++;
            }
        }

        // int[] triangles = new int[] {4, 0, 1, 4, 5, 1, 4, 8, 9, 4, 5, 9};
        int[] triangles = new int[48] {
                                        4, 5, 9,
                                        8, 4, 9,
                                        10, 5, 9,
                                        10, 6, 5,
                                        11, 6, 10,
                                        11, 7, 6,
                                        11, 7, 4,
                                        11, 4, 8,
                                        0, 1, 5,
                                        4, 0, 5,
                                        6, 1, 5,
                                        6, 2, 1,
                                        7, 2, 6,
                                        7, 3, 2,
                                        7, 3, 0,
                                        7, 0, 4
                                     };
        Mesh tubeMesh= tube.GetComponent<MeshFilter>().mesh;
        tubeMesh.vertices = vertices;
        tubeMesh.triangles = triangles;
        tubeMesh.RecalculateNormals();
    }
}
