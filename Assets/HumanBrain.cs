﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanBrain : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject StartBuilding;
    public GameObject EndBuilding;

    public int AgentState = 0; // 0-disabled / 1-started / 2-running

    public Material StartDarkMat;
    public Material EndLightMat;

    private NavMeshAgent agent;

    private void changeMaterial(GameObject obj, Material mat)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().material = mat;
		}
	}

    public void timeOut() {
        agent.Warp(EndBuilding.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (AgentState == 1) {
            agent = GetComponent<NavMeshAgent>();   
            changeMaterial(StartBuilding, StartDarkMat);
            agent.destination = EndBuilding.transform.position;
            agent.isStopped = false;
            AgentState = 2;
        } else if (AgentState == 2) {
            if ((agent.transform.position - agent.destination).magnitude < 1.0f) {
                EndBuilding.GetComponent<Building>().humanHouses.Push(StartBuilding);
                changeMaterial(EndBuilding, EndLightMat);
                AgentState = 0;
                Destroy(agent.gameObject);
            }
        }
    }
}