using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harbor : MonoBehaviour {


    public bool hover;
    public int nationNum;
    public GameObject hex;
    private Hexagon hexagon;

    // Use this for initialization
    void Start()
    {
        nationNum = UIManager.turnNumber;
        name = "Harbor " + UIManager.selectedCountry;
        transform.SetParent(UIManager.selectedCountry.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (hover && UIManager.hoverMode)
        {
            this.transform.position = Mouse.mousePos;
        }
    }
    public void Place(GameObject g)
    {
        hex = g;
        hexagon = g.GetComponent<Hexagon>();
        this.GetComponent<SpriteRenderer>().sortingOrder = 2;
        Destroy(this.GetComponent<CircleCollider2D>());
        Vector3 pos = hex.transform.position;
        this.transform.position = pos;
        foreach (GameObject go in hexagon.adj){
            Hexagon h = go.GetComponent<Hexagon>();
            if (h.hasShip && !h.ship.isHarbored){
                h.ship.isHarbored = true;
            }
        }
        Mouse.ResetViableHexagons();
    }
}
