using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControlleur : MonoBehaviour
{

    public float _Xmov, _Ymov, _Zmov, speed, speedScroll, size;

    void Update()
    {
        //déplacement vertical
        _Zmov = Input.GetAxis("Vertical") * Time.deltaTime * speed * 10;
        transform.position += Vector3.forward * _Zmov;

        //déplacement horizontal
        _Xmov = Input.GetAxis("Horizontal") * Time.deltaTime * speed * 10;
        transform.position += Vector3.right * _Xmov;

        //dépalcement du viseur
        GameObject viseur = GameObject.Find("viseur");
        viseur.transform.position = Input.mousePosition;


        //assignation de la valeur de scrool
        _Ymov = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * speedScroll * 100;

        //sauvergarde du zoom
        size -= _Ymov;

        //vérification de la taille de zoom
        if (size < 3) { size = 3; }
        if (size > 20) { size = 20; }

        //zoom
        gameObject.GetComponent<Camera>().orthographicSize = size;

    }
}
