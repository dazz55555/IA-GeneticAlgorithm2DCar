using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    public float pontuacao = 0;
    public Controlador control;
    public Vector3 posFw;
    public Vector3 posRw;
    public Vector3 scaleFw;
    public Vector3 scaleRw;
    public bool avancando = true;


    private GameObject fw;
    private GameObject rw;
    private float ultimaPontuacao;



    // Use this for initialization
    void Start () {
		pontuacao = this.transform.position.x - control.spawnPosition.transform.position.x;
        ultimaPontuacao = pontuacao;
    }
	
	// Update is called once per frame
	void Update () {
        pontuacao = this.transform.position.x - control.spawnPosition.transform.position.x;
        if(pontuacao >= (ultimaPontuacao + 0.8f) || pontuacao <= (ultimaPontuacao - 0.8f))
        {
            ultimaPontuacao = pontuacao;
            CancelInvoke("NaoAvancando");
        }
        else
        {
            Invoke("NaoAvancando", 3);
        }
	}

    void NaoAvancando()
    {
        avancando = false;
    }

    public void SetFw(GameObject fw)
    {
        this.fw = fw;
    }
    public GameObject GetFw()
    {
        return this.fw;
    }
    public void SetRw(GameObject rw)
    {
        this.rw = rw;
    }
    public GameObject GetRw()
    {
        return this.rw;
    }
}
