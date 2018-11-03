using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorCamera : MonoBehaviour {

    public GameObject carroSelecionado;
    public Controlador controle;

    private float maiorPos = -999;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SelecionarCarro();
        if(carroSelecionado != null)
            this.transform.position = new Vector3(carroSelecionado.transform.position.x, this.transform.position.y, this.transform.position.z);
	}

    void SelecionarCarro()
    {
        maiorPos = -999;
        foreach (GameObject car in controle.individuos)
        {
            if(maiorPos < car.GetComponent<Car>().pontuacao)
            {
                maiorPos = car.GetComponent<Car>().pontuacao;
                carroSelecionado = car;
            }
        }
    }
}
