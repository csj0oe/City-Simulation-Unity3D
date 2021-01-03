using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using UnityEngine.AI;

public class Voronoi : MonoBehaviour
{    public Material land;
    public int NPOINTS = 2000, WIDTH = 1000, HEIGHT = 1000;
	public GameObject road;
	public GameObject roadsParent;
	public float freqx = 0.002f, freqy = 0.002f, offsetx = 0f, offsety = 0.6f;
	public GameObject house;
	public GameObject housesParent;
	public GameObject WorkplacesParent;
	public Material workMaterial;
	public float spaceBetweenHouses = 0.3f;
	public NavMeshSurface surface;
	private float [,] map;
    private List<Vector2> m_points;
	private List<LineSegment> m_edges = null;
	private List<LineSegment> m_spanningTree;
	private List<LineSegment> m_delaunayTriangulation;
	private Texture2D tx;


	private void generateHouses() {
		for (int i = 0; i < roadsParent.transform.childCount; i++) {
			Transform rd = roadsParent.transform.GetChild(i);
			
			float shiftBy1, shiftBy2 = 0;
			int j = 1;
			while (shiftBy2 < rd.transform.localScale.x - (house.transform.GetChild(0).localScale.z * 2) ) {
				shiftBy1 = (rd.transform.GetChild(0).localScale.z/2);
				shiftBy2 = (house.transform.GetChild(0).localScale.z + spaceBetweenHouses) * j;
				createHouse(rd, shiftBy1, shiftBy2, false);
				createHouse(rd, shiftBy1, shiftBy2, true);
				j++;
			}
		}
	}

	private void createHouse(Transform rd, float shiftBy1, float shiftBy2, bool otherSide) {
		GameObject new_house_precalc = Instantiate(house);
		if (otherSide) new_house_precalc.transform.rotation = rd.transform.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
		else           new_house_precalc.transform.rotation = rd.transform.rotation;
		new_house_precalc.transform.position = rd.transform.position;
		// move it next to the road (not on top of it)
		new_house_precalc.transform.position = new_house_precalc.transform.position + ( new_house_precalc.transform.forward * shiftBy1 );
		// move it to the correct position on the road
		if (otherSide) new_house_precalc.transform.position = new_house_precalc.transform.position + (  new_house_precalc.transform.right * shiftBy2 );
		else           new_house_precalc.transform.position = new_house_precalc.transform.position + ( -new_house_precalc.transform.right * shiftBy2 );
		Vector3 poss = new_house_precalc.transform.position;
		// TODO better calculation
		Vector3    pos = new_house_precalc.transform.GetChild(0).position;
		Vector3    hSz = new_house_precalc.transform.GetChild(0).localScale/2;
		Quaternion rot = new_house_precalc.transform.rotation;
		Destroy(new_house_precalc);
		if (!Physics.CheckBox(pos, hSz, rot)) {
			Road data = rd.GetComponent("Road") as Road;
			int zoneType = data.zoneType;
			if (zoneType == 2) return; // empty zone
			int houseSize = ( zoneType > 0 ) ? 10 : 1;
			GameObject new_house_final = Instantiate(house);
			new_house_final.transform.parent = housesParent.transform;
			new_house_final.transform.rotation = rot;
			new_house_final.transform.position = poss;
			if (houseSize > 1) {
				new_house_final.transform.localScale = new_house_final.transform.localScale + new Vector3(0, houseSize, 0);
				changeMaterial(new_house_final, workMaterial);
				new_house_final.transform.parent = WorkplacesParent.transform;
			} else {
				//createHuman(new_house_final);
			}
		} else {
			//Collider[] x = Physics.OverlapBox(pos, hSz, rot);
			//print(x[0].name);
		}
	}

	private void changeMaterial(GameObject house, Material mat)
	{
		for (int i = 0; i < house.transform.childCount; i++)
		{
			house.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = mat;
		}
	}

	void Start ()
	{
        map=createMap();
        Color[] pixels = createPixelMap(map);

        /* Create random points points */
		m_points = new List<Vector2> ();
		List<uint> colors = new List<uint> ();
		for (int i = 0; i < NPOINTS; i++) {
			int x = Random.Range(0, WIDTH-1);
			int y = Random.Range(0, HEIGHT-1);
			int iter = 0;
			while (iter < 3 && map[x,y] < 0.7) {
				x = Random.Range(0, WIDTH-1);
				y = Random.Range(0, HEIGHT-1);
				iter++;
			}
			colors.Add ((uint)0);
			Vector2 vec = new Vector2(x, y); 
			m_points.Add (vec);
		}

		/* Generate Graphs */
		Delaunay.Voronoi v = new Delaunay.Voronoi (m_points, colors, new Rect (0, 0, WIDTH, HEIGHT));
		m_edges = v.VoronoiDiagram ();
		m_spanningTree = v.SpanningTree (KruskalType.MINIMUM);
		m_delaunayTriangulation = v.DelaunayTriangulation ();

		/* Shows Voronoi diagram */
		Color color = Color.blue;
		for (int i = 0; i < m_edges.Count; i++) {
			LineSegment seg = m_edges[i];				
			Vector2 left = (Vector2)seg.p0;
			Vector2 right = (Vector2)seg.p1;
			//DrawLine (pixels,left, right,color);
			Vector2 dir = (right - left)/WIDTH*100; 
			float a = Vector2.SignedAngle(Vector2.right, right - left);
			GameObject go = Instantiate(road, new Vector3(left.y/WIDTH*100-100/2, 0, left.x/HEIGHT*100-100/2), Quaternion.Euler(0,a+90,0));
			go.transform.localScale = new Vector3(dir.magnitude, 1, 1);
			go.transform.parent = roadsParent.transform;
			// set the zone type : 1 - work / 0 - home / 2 - nothing
			if ( Mathf.RoundToInt(seg.p0.Value.x) >= 0 && Mathf.RoundToInt(seg.p0.Value.x) < WIDTH &&
				 Mathf.RoundToInt(seg.p0.Value.y) >= 0 && Mathf.RoundToInt(seg.p0.Value.y) < HEIGHT &&
				 Mathf.RoundToInt(seg.p1.Value.x) >= 0 && Mathf.RoundToInt(seg.p1.Value.x) < WIDTH &&
				 Mathf.RoundToInt(seg.p1.Value.y) >= 0 && Mathf.RoundToInt(seg.p1.Value.y) < HEIGHT &&
				 map[Mathf.RoundToInt(seg.p0.Value.x), Mathf.RoundToInt(seg.p0.Value.y)] > 0.7 &&
				 map[Mathf.RoundToInt(seg.p1.Value.x), Mathf.RoundToInt(seg.p1.Value.y)] > 0.7 ) {
					Road go_data = go.GetComponent("Road") as Road;
					go_data.zoneType = 1;
				 }
			else if ( Mathf.RoundToInt(seg.p0.Value.x) >= 0 && Mathf.RoundToInt(seg.p0.Value.x) < WIDTH &&
				 Mathf.RoundToInt(seg.p0.Value.y) >= 0 && Mathf.RoundToInt(seg.p0.Value.y) < HEIGHT &&
				 Mathf.RoundToInt(seg.p1.Value.x) >= 0 && Mathf.RoundToInt(seg.p1.Value.x) < WIDTH &&
				 Mathf.RoundToInt(seg.p1.Value.y) >= 0 && Mathf.RoundToInt(seg.p1.Value.y) < HEIGHT &&
				 map[Mathf.RoundToInt(seg.p0.Value.x), Mathf.RoundToInt(seg.p0.Value.y)] > 0.5 &&
				 map[Mathf.RoundToInt(seg.p1.Value.x), Mathf.RoundToInt(seg.p1.Value.y)] > 0.5 ) {
					Road go_data = go.GetComponent("Road") as Road;
					go_data.zoneType = 2;
				 }
				 
		}

		/* Shows Delaunay triangulation */
		/*
 		color = Color.red;
		if (m_delaunayTriangulation != null) {
			for (int i = 0; i < m_delaunayTriangulation.Count; i++) {
					LineSegment seg = m_delaunayTriangulation [i];				
					Vector2 left = (Vector2)seg.p0;
					Vector2 right = (Vector2)seg.p1;
					DrawLine (pixels,left, right,color);
			}
		}*/

		/* Shows spanning tree */
		/*
		color = Color.black;
		if (m_spanningTree != null) {
			for (int i = 0; i< m_spanningTree.Count; i++) {
				LineSegment seg = m_spanningTree [i];				
				Vector2 left = (Vector2)seg.p0;
				Vector2 right = (Vector2)seg.p1;
				DrawLine (pixels,left, right,color);
			}
		}*/

		/* Apply pixels to texture */
		tx = new Texture2D(WIDTH, HEIGHT);
        land.SetTexture ("_MainTex", tx);
		tx.SetPixels (pixels);
		tx.Apply ();

		/* Generate Buildings */
		generateHouses();
		/* build the NavMesh */
		surface.BuildNavMesh();
		/* Start Game Logic */
		HumansManager.gameState = 0; // Ready to start game logic
	}

	private float [,] createMap() 
    {
        float [,] map = new float[WIDTH, HEIGHT];
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
                map[i, j] = Mathf.PerlinNoise(freqx * i + offsetx, freqy * j + offsety);
        return map;
    }

    /* Functions to create and draw on a pixel array */
    private Color[] createPixelMap(float[,] map)
    {
        Color[] pixels = new Color[WIDTH * HEIGHT];
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
            {
                pixels[i * HEIGHT + j] = Color.Lerp(Color.white, Color.black, map[i, j]);
            }
        return pixels;
    }
    private void DrawPoint (Color [] pixels, Vector2 p, Color c) {
		if (p.x<WIDTH&&p.x>=0&&p.y<HEIGHT&&p.y>=0) 
		    pixels[(int)p.x*HEIGHT+(int)p.y]=c;
	}
	// Bresenham line algorithm
	private void DrawLine(Color [] pixels, Vector2 p0, Vector2 p1, Color c) {
		int x0 = (int)p0.x;
		int y0 = (int)p0.y;
		int x1 = (int)p1.x;
		int y1 = (int)p1.y;

		int dx = Mathf.Abs(x1-x0);
		int dy = Mathf.Abs(y1-y0);
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx-dy;
		while (true) {
            if (x0>=0&&x0<WIDTH&&y0>=0&&y0<HEIGHT)
    			pixels[x0*HEIGHT+y0]=c;

			if (x0 == x1 && y0 == y1) break;
			int e2 = 2*err;
			if (e2 > -dy) {
				err -= dy;
				x0 += sx;
			}
			if (e2 < dx) {
				err += dx;
				y0 += sy;
			}
		}
	}
}