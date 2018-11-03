using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controlador : MonoBehaviour {

    public GameObject carPrefab;
    public GameObject wheelPrefab;
    public List<GameObject> individuos;
    public List<GameObject> selecionados;
    public List<GameObject> novosIndividuos;
    public Transform spawnPosition;
    public int qntdIndividuos;

    private bool avancado = false;
    private bool todosParados = false;

	// Inicia os indivíduos
	void Start () {
        GameObject car;
        GameObject rw;
        GameObject fw;
        WheelJoint2D joint;
        JointMotor2D motor;
        float random;
        Vector3 randomDianteira, randomTraseira;

        for (int i = 0; i < qntdIndividuos; i++)
        {
            car = Instantiate(carPrefab);       //spawna o veiculo
            individuos.Add(car);
            car.GetComponent<Rigidbody2D>().mass = Random.Range(0.5f, 2.5f); //randomiza a massa do veiculo
            //randomiza o tamanho do veiculo
            car.transform.localScale = new Vector3(Random.Range(0.5f, 3), Random.Range(0.5f, 1), 1);

            rw = Instantiate(wheelPrefab);      //spawna as rodas
            fw = Instantiate(wheelPrefab);      //---------------
            car.GetComponent<Car>().SetFw(fw);
            car.GetComponent<Car>().SetRw(rw);

            random = Random.Range(0.1f, 0.5f);
            rw.transform.localScale = new Vector3(random, random, 1);     //randomiza o tamanho das rodas
            car.GetComponent<Car>().scaleRw = rw.transform.localScale;
            random = Random.Range(0.1f, 0.5f);
            fw.transform.localScale = new Vector3(random, random, 1);
            car.GetComponent<Car>().scaleFw = fw.transform.localScale;

            rw.transform.parent = car.transform;
            fw.transform.parent = car.transform;
            randomTraseira = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), -2);
            randomDianteira = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), -2);
            car.GetComponent<Car>().posFw = randomDianteira;
            car.GetComponent<Car>().posRw = randomTraseira;
            rw.transform.localPosition = randomTraseira;
            fw.transform.localPosition = randomDianteira;

            car.transform.position = spawnPosition.transform.position;

            joint = car.AddComponent<WheelJoint2D>();   // inicia o joint frontal
            joint.connectedBody = fw.GetComponent<Rigidbody2D>();
            joint.anchor = new Vector2(randomDianteira.x, randomDianteira.y);
            joint.useMotor = true;
            motor = joint.motor;
            motor.motorSpeed = -300;
            joint.motor = motor;

            joint = car.AddComponent<WheelJoint2D>();   // inicia o joint trazeiro
            joint.connectedBody = rw.GetComponent<Rigidbody2D>();
            joint.anchor = new Vector2(randomTraseira.x, randomTraseira.y);
            joint.useMotor = true;
            motor = joint.motor;
            motor.motorSpeed = -300;
            joint.motor = motor;

            rw.transform.parent = null;
            fw.transform.parent = null;
        }
    }
	
    void FixedUpdate()
    {
        todosParados = true;
        foreach (GameObject car in individuos)
        {
            if (car.GetComponent<Car>().avancando)
                todosParados = false;
        }
        if (todosParados)
            Selecao();
    }

	public void Selecao()
    {
        float somaTotal = 0;
        float random;
        GameObject escolhido = null;

        for (int i = 0; i < qntdIndividuos/2; i++)
        {
            foreach (GameObject car in individuos)
            {
                somaTotal += car.GetComponent<Car>().pontuacao;
            }
            random = Random.Range(0, somaTotal);
            while (somaTotal > 0)
            {
                foreach (GameObject car in individuos)
                {
                    somaTotal -= car.GetComponent<Car>().pontuacao;
                    if (somaTotal <= 0)
                    {
                        escolhido = car;
                        car.GetComponent<Car>().GetFw().transform.parent = car.transform;
                        car.GetComponent<Car>().GetRw().transform.parent = car.transform;
                        break;
                    }
                }
            }
            individuos.Remove(escolhido);
            selecionados.Add(escolhido);
            somaTotal = 0;
        }
        Reproduzir();
    }

    public void Reproduzir()
    {
        GameObject novoIndividuo = null;
        GameObject rw;
        GameObject fw;
        Vector3 auxVector;
        Vector3 posTraseira;
        Vector3 posDianteira;
        WheelJoint2D joint;
        JointMotor2D motor;
        float aux = 0;
        float tempoAtual = Time.timeScale;

        Time.timeScale = 0;
        foreach (GameObject car in individuos)
        {
            car.GetComponent<Car>().GetFw().SetActive(false);
            car.GetComponent<Car>().GetRw().SetActive(false);
            car.SetActive(false);
        }

        for (int i = 0; i < qntdIndividuos / 2; i += 2)
        {
            novoIndividuo = Instantiate(carPrefab);
            novosIndividuos.Add(novoIndividuo);
            aux = (selecionados[i].GetComponent<Rigidbody2D>().mass + selecionados[i + 1].GetComponent<Rigidbody2D>().mass) / 2;
            novoIndividuo.GetComponent<Rigidbody2D>().mass = aux; //randomiza a massa do veiculo

            auxVector = (selecionados[i].transform.localScale + selecionados[i + 1].transform.localScale) / 2;
            if(auxVector.x < 1.5f)
            {
                auxVector.x = 1.5f;
            }
            if (auxVector.y < 1)
            {
                auxVector.y = 1;
            }
            novoIndividuo.transform.localScale = auxVector;

            rw = Instantiate(wheelPrefab);      //spawna as rodas
            fw = Instantiate(wheelPrefab);      //---------------
            novoIndividuo.GetComponent<Car>().SetFw(fw);
            novoIndividuo.GetComponent<Car>().SetRw(rw);

            auxVector = new Vector3((selecionados[i].GetComponent<Car>().scaleRw.x + selecionados[i + 1].GetComponent<Car>().scaleRw.x / 2),
                         ((selecionados[i].GetComponent<Car>().scaleRw.y + selecionados[i + 1].GetComponent<Car>().scaleRw.y / 2)), 1);
            if(auxVector.x > 1)
            {
                auxVector.x = 1;
            }
            if (auxVector.y > 1)
            {
                auxVector.y = 1;
            }
            rw.transform.localScale = auxVector;
            novoIndividuo.GetComponent<Car>().scaleRw = auxVector;
            auxVector = new Vector3((selecionados[i].GetComponent<Car>().scaleFw.x + selecionados[i + 1].GetComponent<Car>().scaleFw.x / 2),
                         ((selecionados[i].GetComponent<Car>().scaleFw.y + selecionados[i + 1].GetComponent<Car>().scaleFw.y / 2)), 1);
            if (auxVector.x > 1)
            {
                auxVector.x = 1;
            }
            if (auxVector.y > 1)
            {
                auxVector.y = 1;
            }
            fw.transform.localScale = auxVector;
            novoIndividuo.GetComponent<Car>().scaleFw = auxVector;


            rw.transform.parent = novoIndividuo.transform;
            fw.transform.parent = novoIndividuo.transform;

            posTraseira = new Vector3(((selecionados[i].GetComponent<Car>().GetRw().transform.localPosition.x +
                                         selecionados[i + 1].GetComponent<Car>().GetRw().transform.localPosition.x) / 2),
                                         ((selecionados[i].GetComponent<Car>().GetRw().transform.localPosition.y +
                                          selecionados[i + 1].GetComponent<Car>().GetRw().transform.localPosition.y) / 2), -1);
            posDianteira = new Vector3(((selecionados[i].GetComponent<Car>().GetFw().transform.localPosition.x +
                                         selecionados[i + 1].GetComponent<Car>().GetFw().transform.localPosition.x) / 2),
                                         ((selecionados[i].GetComponent<Car>().GetFw().transform.localPosition.y +
                                          selecionados[i + 1].GetComponent<Car>().GetFw().transform.localPosition.y) / 2), -1);
            rw.transform.localPosition = posTraseira;
            fw.transform.localPosition = posDianteira;
            novoIndividuo.GetComponent<Car>().posFw = posDianteira;
            novoIndividuo.GetComponent<Car>().posRw = posTraseira;

            novoIndividuo.transform.position = spawnPosition.transform.position;

            joint = novoIndividuo.AddComponent<WheelJoint2D>();   // inicia o joint frontal
            joint.connectedBody = fw.GetComponent<Rigidbody2D>();
            joint.anchor = new Vector2(posDianteira.x, posDianteira.y);
            joint.useMotor = true;
            motor = joint.motor;
            motor.motorSpeed = -300;
            joint.motor = motor;

            joint = novoIndividuo.AddComponent<WheelJoint2D>();   // inicia o joint trazeiro
            joint.connectedBody = rw.GetComponent<Rigidbody2D>();
            joint.anchor = new Vector2(posTraseira.x, posTraseira.y);
            joint.useMotor = true;
            motor = joint.motor;
            motor.motorSpeed = -300;
            joint.motor = motor;

            rw.transform.parent = null;
            fw.transform.parent = null;
        }

        for (int i = 0; i < (qntdIndividuos / 2)/2; i ++)
        {
            novoIndividuo = Instantiate(carPrefab);
            novosIndividuos.Add(novoIndividuo);
            aux = (selecionados[i].GetComponent<Rigidbody2D>().mass + selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Rigidbody2D>().mass) / 2;
            novoIndividuo.GetComponent<Rigidbody2D>().mass = aux; //randomiza a massa do veiculo

            auxVector = (selecionados[i].transform.localScale + selecionados[(qntdIndividuos / 2) - i - 1].transform.localScale) / 2;
            if (auxVector.x < 1.5f)
            {
                auxVector.x = 1.5f;
            }
            if (auxVector.y < 1)
            {
                auxVector.y = 1;
            }
            novoIndividuo.transform.localScale = auxVector;

            rw = Instantiate(wheelPrefab);      //spawna as rodas
            fw = Instantiate(wheelPrefab);      //---------------
            novoIndividuo.GetComponent<Car>().SetFw(fw);
            novoIndividuo.GetComponent<Car>().SetRw(rw);

            auxVector = new Vector3((selecionados[i].GetComponent<Car>().scaleRw.x + selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Car>().scaleRw.x / 2),
                         ((selecionados[i].GetComponent<Car>().scaleRw.y + selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Car>().scaleRw.y / 2)), 1);
            if (auxVector.x > 1.5f)
            {
                auxVector.x = 1.5f;
            }
            if (auxVector.y > 1.5f)
            {
                auxVector.y = 1.5f;
            }
            rw.transform.localScale = auxVector;
            novoIndividuo.GetComponent<Car>().scaleRw = auxVector;
            auxVector = new Vector3((selecionados[i].GetComponent<Car>().scaleFw.x + selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Car>().scaleFw.x / 2),
                         ((selecionados[i].GetComponent<Car>().scaleFw.y + selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Car>().scaleFw.y / 2)), 1);
            if (auxVector.x > 1)
            {
                auxVector.x = 1;
            }
            if (auxVector.y > 1)
            {
                auxVector.y = 1;
            }
            fw.transform.localScale = auxVector;
            novoIndividuo.GetComponent<Car>().scaleFw = auxVector;


            rw.transform.parent = novoIndividuo.transform;
            fw.transform.parent = novoIndividuo.transform;

            posTraseira = new Vector3(((selecionados[i].GetComponent<Car>().GetRw().transform.localPosition.x +
                                         selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Car>().GetRw().transform.localPosition.x) / 2),
                                         ((selecionados[i].GetComponent<Car>().GetRw().transform.localPosition.y +
                                          selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Car>().GetRw().transform.localPosition.y) / 2), -1);
            posDianteira = new Vector3(((selecionados[i].GetComponent<Car>().GetFw().transform.localPosition.x +
                                         selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Car>().GetFw().transform.localPosition.x) / 2),
                                         ((selecionados[i].GetComponent<Car>().GetFw().transform.localPosition.y +
                                          selecionados[(qntdIndividuos / 2) - i - 1].GetComponent<Car>().GetFw().transform.localPosition.y) / 2), -1);
            rw.transform.localPosition = posTraseira;
            fw.transform.localPosition = posDianteira;
            novoIndividuo.GetComponent<Car>().posFw = posDianteira;
            novoIndividuo.GetComponent<Car>().posRw = posTraseira;

            novoIndividuo.transform.position = spawnPosition.transform.position;

            joint = novoIndividuo.AddComponent<WheelJoint2D>();   // inicia o joint frontal
            joint.connectedBody = fw.GetComponent<Rigidbody2D>();
            joint.anchor = new Vector2(posDianteira.x, posDianteira.y);
            joint.useMotor = true;
            motor = joint.motor;
            motor.motorSpeed = -300;
            joint.motor = motor;

            joint = novoIndividuo.AddComponent<WheelJoint2D>();   // inicia o joint trazeiro
            joint.connectedBody = rw.GetComponent<Rigidbody2D>();
            joint.anchor = new Vector2(posTraseira.x, posTraseira.y);
            joint.useMotor = true;
            motor = joint.motor;
            motor.motorSpeed = -300;
            joint.motor = motor;

            rw.transform.parent = null;
            fw.transform.parent = null;
        }

        for (int i = 0; i < (qntdIndividuos / 2); i++)
        {
            novoIndividuo = Instantiate(carPrefab);
            novosIndividuos.Add(novoIndividuo);
            aux = selecionados[i].GetComponent<Rigidbody2D>().mass;
            novoIndividuo.GetComponent<Rigidbody2D>().mass = aux; //randomiza a massa do veiculo

            auxVector = selecionados[i].transform.localScale;
            novoIndividuo.transform.localScale = auxVector;

            rw = Instantiate(wheelPrefab);      //spawna as rodas
            fw = Instantiate(wheelPrefab);      //---------------
            novoIndividuo.GetComponent<Car>().SetFw(fw);
            novoIndividuo.GetComponent<Car>().SetRw(rw);

            auxVector = new Vector3(selecionados[i].GetComponent<Car>().scaleRw.x, selecionados[i].GetComponent<Car>().scaleRw.y,1);
            rw.transform.localScale = auxVector;
            novoIndividuo.GetComponent<Car>().scaleRw = auxVector;
            auxVector = new Vector3(selecionados[i].GetComponent<Car>().scaleFw.x, selecionados[i].GetComponent<Car>().scaleFw.y, 1);
            fw.transform.localScale = auxVector;
            novoIndividuo.GetComponent<Car>().scaleFw = auxVector;
        

            rw.transform.parent = novoIndividuo.transform;
            fw.transform.parent = novoIndividuo.transform;

            posTraseira = new Vector3(selecionados[i].GetComponent<Car>().GetRw().transform.localPosition.x,
                                         selecionados[i].GetComponent<Car>().GetRw().transform.localPosition.y, -1);
            posDianteira = new Vector3(selecionados[i].GetComponent<Car>().GetFw().transform.localPosition.x,
                                         selecionados[i].GetComponent<Car>().GetFw().transform.localPosition.y, -1);
            rw.transform.localPosition = posTraseira;
            fw.transform.localPosition = posDianteira;
            novoIndividuo.GetComponent<Car>().posFw = posDianteira;
            novoIndividuo.GetComponent<Car>().posRw = posTraseira;

            novoIndividuo.transform.position = spawnPosition.transform.position;

            joint = novoIndividuo.AddComponent<WheelJoint2D>();   // inicia o joint frontal
            joint.connectedBody = fw.GetComponent<Rigidbody2D>();
            joint.anchor = new Vector2(posDianteira.x, posDianteira.y);
            joint.useMotor = true;
            motor = joint.motor;
            motor.motorSpeed = -300;
            joint.motor = motor;

            joint = novoIndividuo.AddComponent<WheelJoint2D>();   // inicia o joint trazeiro
            joint.connectedBody = rw.GetComponent<Rigidbody2D>();
            joint.anchor = new Vector2(posTraseira.x, posTraseira.y);
            joint.useMotor = true;
            motor = joint.motor;
            motor.motorSpeed = -300;
            joint.motor = motor;

            rw.transform.parent = null;
            fw.transform.parent = null;
        }

        foreach (GameObject car in individuos)
        {
            Destroy(car.GetComponent<Car>().GetFw());
            Destroy(car.GetComponent<Car>().GetRw());
            Destroy(car);
        }
        individuos.Clear();
        foreach (GameObject car in selecionados)
        {
            Destroy(car.GetComponent<Car>().GetFw());
            Destroy(car.GetComponent<Car>().GetRw());
            Destroy(car);
        }
        selecionados.Clear();
        foreach (GameObject car in novosIndividuos)
        {
            individuos.Add(car);
        }
        novosIndividuos.Clear();
        Time.timeScale = tempoAtual;
    }

    public void AvancarTempo()
    {
        if(avancado)
        {
            Time.timeScale = 1;
            avancado = false;
        }
        else
        {
            Time.timeScale = 3;

            avancado = true;
        }
    }
}
