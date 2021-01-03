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
    public Material WorkDark;

    private int Timer = -1;
    public int DelayToStartDay = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	private void createHuman(GameObject hs) {
			Vector3 pos = hs.transform.position + ( hs.transform.forward * ( road.transform.GetChild(0).localScale.z/3 ) );
			NavMeshAgent new_human = Instantiate(agent, pos, Quaternion.identity, HumansParent.transform);
			new_human.GetComponent<Click>().cam = cam;
            new_human.GetComponent<HumanBrain>().HouseLight = HouseLight;
            new_human.GetComponent<HumanBrain>().HouseDark = HouseDark;
            new_human.GetComponent<HumanBrain>().WorkLight = WorkLight;
            new_human.GetComponent<HumanBrain>().WorkDark = WorkDark;
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
        if (VoronoiDemo.gameState == 0) {
            //print(housesParent.transform.childCount);
            for (int i = 0; i < housesParent.transform.childCount; i++) {
                //print("change");
                Transform hs = housesParent.transform.GetChild(i);
                changeMaterial(hs.gameObject, HouseLight);
            }
            VoronoiDemo.gameState = 1;
            print("Game Stat = " + VoronoiDemo.gameState);
        } else if (VoronoiDemo.gameState == 1) {
            if(Timer == -1) {
                Timer = DelayToStartDay;
            } else if (Timer > 0) {
                Timer--;
            } else if (Timer == 0) {
                for (int i = 0; i < housesParent.transform.childCount; i++) {
                    Transform hs = housesParent.transform.GetChild(i);
                    //changeMaterial(hs.gameObject, HouseDark);
                    createHuman(hs.gameObject);
                }
                VoronoiDemo.gameState = 2;
                print("Game Stat = " + VoronoiDemo.gameState);
            }
        }
    }
}
