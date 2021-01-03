using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumansAtHouses : MonoBehaviour
{

    public Camera cam;
    public GameObject road;
	public GameObject roadsParent;
	public GameObject house;
	public GameObject housesParent;
	public GameObject WorkplacesParent;
	public NavMeshAgent agent;
	public GameObject HumansParent;

    public Material HouseLight;
    public Material HouseDark;
    public Material WorkLight;
    private float Timer = -1f;
    public int DelayToStartDay = 3;
    public int DelayForMovement = 30;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	private void createHuman(GameObject hs) {
			Vector3 pos = hs.transform.position + ( hs.transform.forward * ( road.transform.GetChild(0).localScale.z/3 ) );
			NavMeshAgent new_human = Instantiate(agent, pos, Quaternion.identity, HumansParent.transform);
			new_human.GetComponent<Click>().cam = cam;
            new_human.GetComponent<HumanBrain>().StartDarkMat = HouseDark;
            new_human.GetComponent<HumanBrain>().EndLightMat = WorkLight;
			new_human.GetComponent<HumanBrain>().StartBuilding = hs;
            int randomWorkplace = Random.Range(0, WorkplacesParent.transform.childCount);
            new_human.GetComponent<HumanBrain>().EndBuilding = WorkplacesParent.transform.GetChild(randomWorkplace).gameObject;
            new_human.GetComponent<HumanBrain>().AgentState = 1;
	}

    private void changeMaterial(GameObject obj, Material mat)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = mat;
		}
	}

    // Update is called once per frame
    void Update()
    {
        if (VoronoiDemo.gameState == 0) { // init world
            //print(housesParent.transform.childCount);
            for (int i = 0; i < housesParent.transform.childCount; i++) {
                //print("change");
                Transform hs = housesParent.transform.GetChild(i);
                changeMaterial(hs.gameObject, HouseLight);
            }
            VoronoiDemo.gameState = 1;
            print("Game Stat = " + VoronoiDemo.gameState);
        } else if (VoronoiDemo.gameState == 1) { // spawn humans
            if(Timer < 0f) {
                Timer = DelayToStartDay;
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
                print("Game Stat = " + VoronoiDemo.gameState + " :: Time Left = " + Timer);
            }
            if (Timer-Time.deltaTime <= 0f) {
                for (int i = 0; i < housesParent.transform.childCount; i++) {
                    Transform hs = housesParent.transform.GetChild(i);
                    //changeMaterial(hs.gameObject, HouseDark);
                    createHuman(hs.gameObject);
                }
                VoronoiDemo.gameState = 2;
                Timer = -1f;
                print("Game Stat = " + VoronoiDemo.gameState);
            }
        } else if (VoronoiDemo.gameState == 2) { // humans moving
            if(Timer < 0f) {
                Timer = DelayForMovement;
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
                print("Game Stat = " + VoronoiDemo.gameState + " :: Time Left = " + Timer);
            }
            if (Timer-Time.deltaTime <= 0f) {
                for (int i = 0; i < HumansParent.transform.childCount; i++) {
                    NavMeshAgent hm = HumansParent.transform.GetChild(i).GetComponent<NavMeshAgent>();
                    //changeMaterial(hs.gameObject, HouseDark);
                    hm.GetComponent<HumanBrain>().timeOut();
                }
                VoronoiDemo.gameState = 3;
                Timer = -1f;
                print("Game Stat = " + VoronoiDemo.gameState);
            }
        } 
    }
}
