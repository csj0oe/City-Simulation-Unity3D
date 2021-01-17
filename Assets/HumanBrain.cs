using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanBrain : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject StartBuilding;
    public GameObject EndBuilding;

    public int AgentState = -1; // -1-disabled / 0-waiting / 1-started / 2-running

    public Material StartDarkMat;
    public Material EndLightMat;

    public float SpawnDelay = 0f;

    private NavMeshAgent agent;
    private Vector3 originalScale;
    private float Timer = -1f;
    public int areaMask = -1;
    private void changeMaterial(GameObject obj, Material mat) {
		for (int i = 0; i < obj.transform.childCount; i++) {
			obj.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = mat;
		}
	}
    private void updateTexture(bool enabled) {
        if (enabled) {
            NavMeshHit navhit;
            NavMesh.SamplePosition(agent.transform.position, out navhit, 1, NavMesh.AllAreas);
            areaMask = IndexFromMask(navhit.mask);
            if (areaMask > 0) {
                agent.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                agent.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
                agent.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;
                agent.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
                agent.transform.GetChild(4).GetComponent<MeshRenderer>().enabled = false;
                agent.transform.GetChild(5).GetComponent<MeshRenderer>().enabled = false;
                agent.transform.GetChild(6).GetComponent<MeshRenderer>().enabled = false;
            } else {
                agent.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                agent.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
                agent.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = true;
                agent.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = true;
                agent.transform.GetChild(4).GetComponent<MeshRenderer>().enabled = true;
                agent.transform.GetChild(5).GetComponent<MeshRenderer>().enabled = true;
                agent.transform.GetChild(6).GetComponent<MeshRenderer>().enabled = true;
            }
        } else {
            agent.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            agent.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = false;
            agent.transform.GetChild(2).GetComponent<MeshRenderer>().enabled = false;
            agent.transform.GetChild(3).GetComponent<MeshRenderer>().enabled = false;
            agent.transform.GetChild(4).GetComponent<MeshRenderer>().enabled = false;
            agent.transform.GetChild(5).GetComponent<MeshRenderer>().enabled = false;
            agent.transform.GetChild(6).GetComponent<MeshRenderer>().enabled = false;
        }
    }
    private int IndexFromMask(int mask) {
        for (int i = 0; i < 32; ++i) {
            if ((1 << i & mask) != 0) {
                return i;
            }
        }
        return -1;
    }
    public void timeOut() {
        agent.Warp(EndBuilding.transform.position);
    }


    // Update is called once per frame
    void Update()
    {
        if (AgentState == 0) {
            if(Timer < 0f) {
                agent = GetComponent<NavMeshAgent>(); 
                //originalScale = agent.transform.localScale;
                //agent.transform.localScale = new Vector3(0f,0f,0f);
                updateTexture(false);
                //agent.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                Timer = Random.Range(0, SpawnDelay);
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
            }
            if (Timer-Time.deltaTime <= 0f) {
                AgentState = 1;
                Timer = -1f;
            }
        } else if (AgentState == 1) {
            //agent.transform.localScale = new Vector3(2f,2f,2f);
            updateTexture(true);
            //agent.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            changeMaterial(StartBuilding, StartDarkMat);
            agent.destination = EndBuilding.transform.position;  
            agent.isStopped = false;
            AgentState = 2;
        } else if (AgentState == 2) {
            if(Timer < 0f) {
                Timer = .5f;
            } else if (Timer > 0f) {
                Timer -= Time.deltaTime;
            }
            if (Timer-Time.deltaTime <= 0f) {
                updateTexture(true);
                Timer = -1f;
            }
            if ((agent.transform.position - agent.destination).magnitude < 1.0f) {
                EndBuilding.GetComponent<Building>().humanHouses.Push(StartBuilding);
                changeMaterial(EndBuilding, EndLightMat);
                AgentState = -1;
                Destroy(agent.gameObject);
            }
        }
    }
}
