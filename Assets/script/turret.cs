using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret : MonoBehaviour {

    [SerializeField] float lookSensibility, firerate = 1; float _xMouse,_yMouse,mousecanons, mousecam, firecountdown, mapcountdown , distanceRéel,RotationX;

    private Ray ray; private RaycastHit hit;

    public GameObject canons, cam, target, bullet, eject, carte;

    GameObject[] ejects;

    Vector3 mouseVisée;

    int incrémentation;

    bool isMap;

	void Start ()
    {
        //assignation des variable
        lookSensibility = 1;
        canons = GameObject.Find("canons");
        cam = GameObject.Find("vue tir");
        eject = GameObject.Find("eject r");
        carte = GameObject.Find("vue carte");
        carte.SetActive(false);

        ejects = GameObject.FindGameObjectsWithTag("eject");

        //cacher le curseur
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

	void Update ()
    {
        Rotation();
        Shoot();
        visee();
        viseeCarte();
        distance();
        rotationCanons();
    }

    void Rotation()
    {
        if (!isMap)
        {
            //replaçage du viseur
            GameObject viseur = GameObject.Find("viseur");
            viseur.transform.position = new Vector2(Screen.width / 2, Screen.height / 2);

            //assignation des valeurs
            _yMouse -= Input.GetAxis("Mouse Y") * Time.deltaTime * 100;
            _xMouse += Input.GetAxis("Mouse X") * Time.deltaTime * 100;

            //bloquage de la variable de la cam
            if (_yMouse > 70) { _yMouse = 70; }
            if (_yMouse < -90) { _yMouse = -90; }

            //rotation de la cam
            Vector3 rotationY = new Vector3(_yMouse, _xMouse, 0) * lookSensibility;
            cam.transform.rotation = Quaternion.Euler(rotationY);

            //rotation des graphiques
            Vector3 rotationX = new Vector3(0, _xMouse, 0) * lookSensibility;
            gameObject.transform.rotation = Quaternion.Euler(rotationX);
            RotationX = _xMouse;
        }

        if (isMap)
        {
            //recupération de la position de la camera
            Vector2 mypos = carte.GetComponent<Camera>().WorldToScreenPoint(transform.position);

            //blocage des variable
            float posX = mypos.x; //Mathf.Clamp(mypos.x, 0f, 1920f);
            float posY = mypos.y; //Mathf.Clamp(mypos.y, 0f, 1080f);

            //calcul des distances
            float distX = Input.mousePosition.x - posX;
            float distY = Input.mousePosition.y - posY;

            //calcul de l'angle
            float angle = Mathf.Atan(distX / distY) * Mathf.Rad2Deg;

            //rotation de l'objet
            transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));

        }
    }

    void Shoot()
    {

        if (Input.GetButton("Fire1") && firecountdown < Time.time && !isMap)
        {
            firecountdown = Time.time + firerate;


            Camera camera = cam.GetComponent<Camera>();
            RaycastHit hit;
            Vector2 screenPoint = new Vector2(Screen.width / 2, Screen.height / 2);
            ray = camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out hit, camera.farClipPlane))
            {

                GameObject mytarget = Instantiate(target, hit.point, Quaternion.identity);
                GameObject mybullet = Instantiate(bullet,eject.transform.position, Quaternion.identity);
                mybullet.GetComponent<bulletTrajectoire>().envoieDonnes(eject.transform, mytarget.transform);
            }

        }

        if (Input.GetButton("Fire1") && firecountdown < Time.time && isMap)
        {
            firecountdown = Time.time + firerate;


            Camera camera = carte.GetComponent<Camera>();
            RaycastHit hit;
            Vector2 screenPoint = Input.mousePosition;
            ray = camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out hit, camera.farClipPlane))
            {
                incrémentation++;
                if(incrémentation> 1) { incrémentation = 0; }

                GameObject mytarget = Instantiate(target, hit.point, Quaternion.identity);
                GameObject mybullet = Instantiate(bullet, ejects[incrémentation].transform.position, Quaternion.identity);
                mybullet.GetComponent<bulletTrajectoire>().envoieDonnes(eject.transform, mytarget.transform);
            }

        }

    }

    void visee()
    {

        Camera camera = cam.GetComponent<Camera>();

        if (Input.GetButton("Fire2"))
        {

            camera.fieldOfView = 30;

        }
        else
        {
            camera.fieldOfView = 60;
        }
    }

    void viseeCarte ()
    {

        isMap = carte.activeSelf;

        if (Input.GetButton("Fire3") && isMap == false && mapcountdown < Time.time)
        {
            mapcountdown = Time.time + 0.5f;
            carte.SetActive(true);
            isMap = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetButton("Fire3") && isMap && mapcountdown < Time.time)
        {
            mapcountdown = Time.time + 0.5f;
            carte.SetActive(false);
            isMap = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void distance()
    {
        Camera camera = cam.GetComponent<Camera>();
        RaycastHit hit;
        Vector2 screenPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        ray = camera.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out hit, camera.farClipPlane))
        {
            mouseVisée = hit.point;
            
            distanceRéel = Vector3.Distance(transform.position, mouseVisée);
        }
    }

    void rotationCanons()
    {
        if(distanceRéel != 0)
        {
        //définition de l'inclinaison a un max
        float inclinaison = 2;

        //réduction de l'inclinaison pour ne pas dépasser une hauteur max
        while ((inclinaison * (distanceRéel / 2) * (distanceRéel / 2) + inclinaison * distanceRéel * distanceRéel / 2) > 25)
        {

            if (inclinaison > 0.02f)
            {
                inclinaison -= 0.01f;
            }

            if (inclinaison < 0.02f)
            {
                inclinaison -= 0.001f;
            }

        }

        //calcul de la hauteur selon une courbe en temps réel
        float hauteur = (inclinaison * (distanceRéel / 2) * (distanceRéel / 2) + inclinaison * distanceRéel * distanceRéel / 2);

        float angle = Mathf.Atan(hauteur / distanceRéel) * Mathf.Rad2Deg;

            canons.transform.rotation = Quaternion.Euler(new Vector3(-angle, RotationX * lookSensibility, 0f));

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 100);
    }
}
