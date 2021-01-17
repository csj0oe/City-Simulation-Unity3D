using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumansManager : MonoBehaviour
{
    public GameObject road;
	public GameObject housesParent;
	public GameObject WorkplacesParent;
	public NavMeshAgent agent;
	public GameObject HumansParent;

    public Material HouseLight;
    public Material HouseDark;
    public Material WorkLight;
    public Material WorkDark;
    private float Timer = -1f;
    public int DelayToStartDay = 10;
    public int DelayToEndDay = 10;
    public int DelayForSpawn = 5;
    public int DelayForMovement = 3*60;
	public static int gameState = -1;  // -1-notReady / 0-init / 1-AtHome / 2-goingToWork / 3-atWork / 4-goingToHome / 5-Finished
    private void createHuman(GameObject startB, GameObject endB, Material startM, Material endM) {
			Vector3 pos = startB.transform.position + ( startB.transform.forward * ( road.transform.GetChild(0).localScale.z/3 ) );
			NavMeshAgent new_human = Instantiate(agent, pos, Quaternion.identity, HumansParent.transform);
			//new_human.GetComponent<Click>().cam = cam;
            new_human.GetComponent<HumanBrain>().StartDarkMat = startM;
            new_human.GetComponent<HumanBrain>().EndLightMat = endM;
			new_human.GetComponent<HumanBrain>().StartBuilding = startB;
            new_human.GetComponent<HumanBrain>().EndBuilding = endB;
            new_human.GetComponent<HumanBrain>().SpawnDelay = DelayForSpawn;
            new_human.GetComponent<HumanBrain>().AgentState = 0;
	}

    private void changeMaterial(GameObject obj, Material mat) {
		for (int i = 0; i < obj.transform.childCount; i++) {
			obj.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = mat;
		}
	}

    // Update is called once per frame
    void Update() {
        if (gameState == 0) { // init world
            int randomWorkplace = 0;
            for (int i = 0; i < housesParent.transform.childCount; i++) {
                GameObject hs = housesParent.transform.GetChild(i).gameObject;
                randomWorkplace = (randomWorkplace+1)%WorkplacesParent.transform.childCount;
                //changeMaterial(hs, HouseLight);
                GameObject eb = WorkplacesParent.transform.GetChild(randomWorkplace).gameObject;
                hs.GetComponent<Building>().humanHouses.Push(eb);
            }
            gameState = 1;
            print("Game Stat = " + gameState);
        } else if (gameState == 1) { // spawn humans
            if(Timer < 0f) {
                Timer = DelayToStartDay;
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
                print("Game Stat = " + gameState + " :: Time Left = " + Timer);
            }
            if (Timer-Time.deltaTime <= 0f) {
                for (int i = 0; i < housesParent.transform.childCount; i++) {
                    GameObject hs = housesParent.transform.GetChild(i).gameObject;
                    while(hs.GetComponent<Building>().humanHouses.Count > 0) {
                        GameObject ds = hs.GetComponent<Building>().humanHouses.Pop();
                        createHuman(hs, ds, HouseDark, WorkLight);
                    }
                }
                gameState = 2;
                Timer = -1f;
                print("Game Stat = " + gameState);
            }
        } else if (gameState == 2) { // humans moving
            if(Timer < 0f) {
                Timer = DelayForMovement;
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
                print("Game Stat = " + gameState + " :: Time Left = " + Timer);
            }
            if (Timer-Time.deltaTime <= 0f || HumansParent.transform.childCount == 0 ) {
                for (int i = 0; i < HumansParent.transform.childCount; i++) {
                    NavMeshAgent hm = HumansParent.transform.GetChild(i).GetComponent<NavMeshAgent>();
                    //changeMaterial(hs.gameObject, HouseDark);
                    hm.GetComponent<HumanBrain>().timeOut();
                }
                gameState = 3;
                Timer = -1f;
                print("Game Stat = " + gameState);
            }
        } else if (gameState == 3) { // spawn humans
            if(Timer < 0f) {
                Timer = DelayToEndDay;
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
                print("Game Stat = " + gameState + " :: Time Left = " + Timer);
            }
            if (Timer-Time.deltaTime <= 0f) {
                for (int i = 0; i < WorkplacesParent.transform.childCount; i++) {
                    GameObject hs = WorkplacesParent.transform.GetChild(i).gameObject;
                    while(hs.GetComponent<Building>().humanHouses.Count > 0) {
                        GameObject ds = hs.GetComponent<Building>().humanHouses.Pop();
                        createHuman(hs, ds, WorkDark, HouseLight);
                    }
                }
                gameState = 4;
                Timer = -1f;
                print("Game Stat = " + gameState);
            }
        } else if (gameState == 4) { // humans moving
            if(Timer < 0f) {
                Timer = DelayForMovement;
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
                print("Game Stat = " + gameState + " :: Time Left = " + Timer);
            }
            if (Timer-Time.deltaTime <= 0f || HumansParent.transform.childCount == 0 ) {
                for (int i = 0; i < HumansParent.transform.childCount; i++) {
                    NavMeshAgent hm = HumansParent.transform.GetChild(i).GetComponent<NavMeshAgent>();
                    //changeMaterial(hs.gameObject, HouseDark);
                    hm.GetComponent<HumanBrain>().timeOut();
                }
                gameState = 1;
                Timer = -1f;
                print("Game Stat = " + gameState);
            }
        } 
    }
}
