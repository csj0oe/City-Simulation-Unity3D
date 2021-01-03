using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumansAtWork : MonoBehaviour
{

    public Camera cam;
    public GameObject road;
	public GameObject roadsParent;
	public GameObject house;
	public GameObject housesParent;
	public GameObject WorkplacesParent;
	public NavMeshAgent agent;
	public GameObject HumansParent;
    public Material WorkDark;
    public Material HouseLight;
    private float Timer = -1f;
    public int DelayToEndDay = 3;
    public int DelayForMovement = 30;

	private void createHuman(GameObject startB, GameObject endB) {
			Vector3 pos = startB.transform.position + ( startB.transform.forward * ( road.transform.GetChild(0).localScale.z/3 ) );
			NavMeshAgent new_human = Instantiate(agent, pos, Quaternion.identity, HumansParent.transform);
			new_human.GetComponent<Click>().cam = cam;
            new_human.GetComponent<HumanBrain>().StartDarkMat = WorkDark;
            new_human.GetComponent<HumanBrain>().EndLightMat = HouseLight;
			new_human.GetComponent<HumanBrain>().StartBuilding = startB;
            new_human.GetComponent<HumanBrain>().EndBuilding = endB;
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
        if (VoronoiDemo.gameState == 3) { // spawn humans
            if(Timer < 0f) {
                Timer = DelayToEndDay;
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
                print("Game Stat = " + VoronoiDemo.gameState + " :: Time Left = " + Timer);
            }
            if (Timer-Time.deltaTime <= 0f) {
                for (int i = 0; i < WorkplacesParent.transform.childCount; i++) {
                    GameObject hs = WorkplacesParent.transform.GetChild(i).gameObject;
                    while(hs.GetComponent<Building>().humanHouses.Count > 0) {
                        GameObject ds = hs.GetComponent<Building>().humanHouses.Pop();
                        createHuman(hs, ds);
                    }
                }
                VoronoiDemo.gameState = 4;
                Timer = -1f;
                print("Game Stat = " + VoronoiDemo.gameState);
            }
        } else if (VoronoiDemo.gameState == 4) { // humans moving
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
                VoronoiDemo.gameState = 5;
                Timer = -1f;
                print("Game Stat = " + VoronoiDemo.gameState);
            }
        } 
    }
}