using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletTrajectoire : MonoBehaviour {

    float distance, distanceRestante, hauteur; public float inclinaison = -0.5f, speed = 70;

    Transform turret, target;public GameObject particule;

	
    public void envoieDonnes(Transform _turret, Transform _target)
    {
        //récupération des variables
        turret = _turret;
        target = _target;

        //calcul des potitions
        Vector3 TargetSol = new Vector3(target.position.x, 0, target.position.z);
        Vector3 turretSol = new Vector3(turret.position.x, 0, turret.position.z);

        //calcul des distance
        distance = Vector3.Distance(TargetSol, turretSol);

        //vérification de la distance max
        if(distance > 100)
        {
            Destroy(gameObject);
            Destroy(target.gameObject);
            Debug.Log("trop loin");
        }

        if (distance < 5)
        {
            Destroy(gameObject);
            Destroy(target.gameObject);
            Debug.Log("trop près");
        }
    }


    void Update ()
    {
        Distance();
        Move();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //vérification de l'arrivé 
        if(other.name == target.name)
        {
            Destroy(gameObject);
            Destroy(target.gameObject);
            GameObject go =Instantiate(particule, transform.position, Quaternion.identity);
            Destroy(go, 5);
        }
    }

    void Move()
    {
        //définition de l'inclinaison a un max
        inclinaison = 2;

        //réduction de l'inclinaison pour ne pas dépasser une hauteur max
        while ((inclinaison * (distance/2) * (distance / 2) + inclinaison * distance * distance/2) > 25 )
        {

            if (inclinaison > 0.02f)
            {
                inclinaison -= 0.01f;
            }
            
            if(inclinaison < 0.02f)
            {
                inclinaison -= 0.001f;
            }

        }

        //calcul de la hauteur selon une courbe en temps réel
        hauteur = inclinaison * distanceRestante * distanceRestante + inclinaison * distance * distanceRestante;

        //calcul de la direction
        Vector3 dir = target.position - transform.position;
        //calcul de la distance à parcourir
        float distanceThisFrame = speed * Time.deltaTime;

        //ajout de la hauteur
        dir += Vector3.up * hauteur;

        //déplacement
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void Distance()
    {
        //calcul des positions
        Vector3 TargetSol = new Vector3(target.position.x, 0, target.position.z);
        Vector3 turretSol = new Vector3(transform.position.x, 0, transform.position.z);

        //calcul de la distance restante
        distanceRestante = Vector3.Distance(TargetSol, turretSol);
    }
}
