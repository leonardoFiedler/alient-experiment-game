﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fase04Controller : BaseFaseController
{
    [SerializeField]
    private GameObject[] bourbons;

	private int[] ordemBourbons = new int[] { 1, 5, 3, 0, 2, 4 };
	private List<int> listaBourbons = new List<int>();
    private int idCollectable; //Id do objeto coletado;
	public Transform enemySpawn;

	private bool canOpen;

    void Start () 
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length > 0) {
			player = GameObject.FindGameObjectsWithTag("Player")[0];
        	player.transform.position = playerSpawn.position;
		} else {
			player = Instantiate(Resources.Load("Player", typeof(GameObject)), new Vector3(playerSpawn.position.x, playerSpawn.position.y, 0), Quaternion.identity) as GameObject;
		}
		
		canOpen = false;
		SetOpenBourbon(0);
    }
        
    public override void Update () 
    {
        base.Update();
    }       

    public override void GetInput()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			int count = 0;
			bool acertou = true;

			Collider2D[] collectObject = Physics2D.OverlapCircleAll(player.transform.position, 0.3f);
            if (collectObject.Length > 0)
            {
                foreach (Collider2D collider2D in collectObject) 
				{
					if (collider2D.tag == "Barril" && canOpen)
					{
						idCollectable = collider2D.GetComponent<CollectableBehavior>().Id;

						if (listaBourbons.Contains(idCollectable)) 
							break;


						foreach (var b in bourbons)
						{
							if (b.GetComponent<CollectableBehavior>().Id == idCollectable) {
								b.GetComponent<Animator>().SetBool("open", true);
							}
						}

						Debug.Log("Ordem: " + ordemBourbons.Length + " - Lista: "+ listaBourbons.Count);

						if (ordemBourbons.Length == (listaBourbons.Count + 1))
						{
							listaBourbons.Add(idCollectable);
							
							count = 0;
							foreach (int bourbon in ordemBourbons)
							{
								if (bourbon != listaBourbons[count])
								{
									acertou = false;
									break;
								}
								count++;
							}
							
							Debug.Log("Acertou: " + acertou);
							if (acertou)
								Instantiate(Resources.Load("Papers"), papersPosition.position, Quaternion.identity);
							else
							{
								listaBourbons = new List<int>();
								foreach (var b in bourbons) 
								{
									b.GetComponent<Animator>().SetBool("open", false);
								}

								GameObject gameObject = Instantiate(Resources.Load("Enemy", typeof(GameObject)), 
								new Vector3(enemySpawn.position.x, enemySpawn.position.y, 0), Quaternion.identity) as GameObject;
								gameObject.transform.Find("Range").GetComponent<CircleCollider2D>().radius = 12.0f;

							}
						}
						else
						{
							listaBourbons.Add(idCollectable);
							Debug.Log("Barril: " + idCollectable);
						}

						break;
					}
                }
            }
		}
        base.GetInput();
	}

	public void SetOpenBourbon(int index)
	{
		int pos = ordemBourbons[index];
		bourbons[pos].GetComponent<Animator>().SetBool("open", true);
		StartCoroutine(CloseBourbon(bourbons[pos], index));
	}

	IEnumerator CloseBourbon(GameObject bourbon, int index)
	{
		yield return new WaitForSeconds(2);
		bourbon.GetComponent<Animator>().SetBool("open", false);
		index++;
		if (index < ordemBourbons.Length)
			SetOpenBourbon(index);
		else
			canOpen = true;
	}
}