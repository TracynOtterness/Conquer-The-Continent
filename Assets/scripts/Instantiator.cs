using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class Instantiator : MonoBehaviour
{
    public Hexagon hex; //our link to the hexagon script
    public Hexagon chex;
    public int HexPosX = 0;//the inital x position of the instantiator on the hexagon grid
    public int HexPosY = 0;//the inital y position of the instantiator on the hexagon grid
    private int HexColumn = 50; // how many columns there are (basically the max x value)
    private int HexRow = 76; // how many rows there are (basically the max y value)
    private int countryNumber = 1;
    public int countryCount = 1;
    GameObject hexagon; // a gameobject we use for instantiation as well as Landmaster()
    public float x; //the coordinates in the gameworld that each hexagon will be instantiated at on runtime
    public float y;
    public int totalHexes = 0; // the total hexagons that have been turned into land. Needs to be here because it is changed by a method in hexagon
    public bool countryExists;
    private int maxCountries = 250;
    public GameObject[] countries;

    void Start()
    { //everything between here and when it calls Landmaster() is creating the grid itself.
        x = this.transform.position.x;
        y = this.transform.position.y;
        for (int i = 0; i <= HexColumn; i++)
        {
            x = 0;
            HexPosX = 0;
            for (int j = 0; j <= HexRow; j++)
            {
                x += .32f;
                transform.position = new Vector3(x, y, 0f);
                hexagon = (GameObject)Instantiate(Resources.Load("Hexagon"), new Vector3(x, y, 0f), Quaternion.identity);
                HexPosX++;
            }
            HexPosY++;
            y += .38f;
        }
        for (int l; l <= maxCountries; l++){
            GameObject country = (GameObject)Instantiate(Resources.Load("Country"), new Vector3(0 , 0 , 0), Quaternion.identity);
            countryCount++;
        }
        countries = GameObject.FindObjectsWithTag("country");
        LandMaster(); // changes some hexagons to land, generates the random map each time.
        Invoke("CountryMaster", 1);
    }
    public void LandMaster()
    {
        int b = 0; // used in switch statements later on
        int step = 200; //how many steps it takes in each walk
        int maxHexes = 1000; //the minimum number of hexagons that will be on a map
        string currenthex; // a string we use to set hexagon to the correct instance later
        while (totalHexes < maxHexes)
        { //from here to line 167 is the code to randomly change the hexagons to land in such a way that they are all going to be connected
            //HexPosX = HexRow / 2;
            //HexPosY = HexColumn / 2;
            HexPosX = UnityEngine.Random.Range(5, HexRow - 6);
            HexPosY = UnityEngine.Random.Range(5, HexColumn - 6);
            for (int steps = 0; steps < step; steps++)
            { //here we check if the hexagon is on the border, which limits the directions the instantiator can step next
                if (HexPosX == 5)
                {
                    if (HexPosY == 5)
                    {
                        b = 0; //leftmost column bottom
                    }
                    else if (HexPosY == HexColumn - 5)
                    {
                        b = 1; //leftmost column top
                    }
                    else
                    {
                        b = 2; //leftmost column, not top or bottom
                    }
                }
                else if (HexPosX == HexRow - 5)
                {
                    if (HexPosY == HexColumn - 5)
                    {
                        b = 3; //rightmost column top
                    }
                    else if (HexPosY == 5)
                    {
                        b = 4; //rightmost column bottom
                    }
                    else
                    {
                        b = 5; //rightmost column, not top or bottom
                    }
                }
                else if (HexPosY == 5)
                {
                    if (HexPosX % 2 == 0)
                    {
                        b = 6; //bottom row, even column
                    }
                    else if (HexPosX % 2 == 1)
                    {
                        b = 7; //bottom row, odd column
                    }
                }
                else if (HexPosY == HexColumn - 5)
                {
                    if (HexPosX % 2 == 0)
                    {
                        b = 8; //top row, even column
                    }
                    else if (HexPosX % 2 == 1)
                    {
                        b = 9; //top row, odd column
                    }
                }
                else
                {
                    b = 10; //not a border hexagon
                }
                switch (b)
                { //picks a random direction to go based on the last step. starting at 0 above the hexagon, the values represent directions going clockwise around the hexagon
                    case 0:
                        b = UnityEngine.Random.Range(0, 2);
                        break;
                    case 1:
                        b = UnityEngine.Random.Range(1, 4);
                        break;
                    case 2:
                        b = UnityEngine.Random.Range(0, 4);
                        break;
                    case 3:
                        b = UnityEngine.Random.Range(3, 6);
                        break;
                    case 4:
                        b = UnityEngine.Random.Range(0, 2);
                        if (b == 1)
                        {
                            b = 5;
                        }
                        break;
                    case 5:
                        b = UnityEngine.Random.Range(3, 7);
                        if (b == 6)
                        {
                            b = 0;
                        }
                        break;
                    case 6:
                        b = UnityEngine.Random.Range(0, 3);
                        if (b == 2)
                        {
                            b = 5;
                        }
                        break;
                    case 7:
                        b = UnityEngine.Random.Range(0, 5);
                        if (b == 3)
                        {
                            b = 4;
                        }
                        if (b == 4)
                        {
                            b = 5;
                        }
                        break;
                    case 8:
                        b = UnityEngine.Random.Range(1, 6);
                        break;
                    case 9:
                        b = UnityEngine.Random.Range(2, 5);
                        break;
                    case 10:
                        b = UnityEngine.Random.Range(0, 6);
                        break;
                }
                switch (b)
                { //based on which direction it went, change the values on the hexagonal grid appropriately
                    case 0:
                        HexPosY++;
                        break;
                    case 1:
                        if (HexPosX % 2 == 1)
                        {
                            HexPosY++;
                        }
                        HexPosX++;
                        break;
                    case 2:
                        if (HexPosX % 2 == 0)
                        {
                            HexPosY--;
                        }
                        HexPosX++;
                        break;
                    case 3:
                        HexPosY--;
                        break;
                    case 4:
                        if (HexPosX % 2 == 0)
                        {
                            HexPosY--;
                        }
                        HexPosX--;
                        break;
                    case 5:
                        if (HexPosX % 2 == 1)
                        {
                            HexPosY++;
                        }
                        HexPosX--;
                        break;
                }
                currenthex = "Hexagon " + HexPosX + " , " + HexPosY;
                hexagon = GameObject.Find(currenthex);//define the instance of the hexagon object we need to change
                hex = hexagon.GetComponent<Hexagon>();//get the instance of the Hexagon script attached to hexagon
                if (hex.isLand == true)
                {//doesn't increment totalHexes
                    continue;

                }
                else
                {//does increment totalhexes
                    hex.CreateLand();
                }
            }
        }
    }

    public void CountryMaster()
    {
        HexPosY = 0;
        for (int i = 0; i <= HexColumn; i++)
        {
            HexPosX = 0;

            for (int j = 0; j <= HexRow; j++)
            {
                countryExists = false;
                hexagon = GameObject.Find("Hexagon " + HexPosX + " , " + HexPosY);
                hex = hexagon.GetComponent<Hexagon>();
                foreach (GameObject c in hex.adj)
                {
                    try
                    {
                        chex = c.GetComponent<Hexagon>();
                    }
                    catch (Exception e) { continue; }

                    if (hex.nationNum != 0 && chex.nationNum == hex.nationNum && chex.isCountry == true)
                    {
                        hex.isCountry = true;
                        hexagon.transform.SetParent(c.transform.parent);
                        countryExists = true;
                    }
      }
                foreach (GameObject c in hex.adj) {
                    try
                    {
                        chex = c.GetComponent<Hexagon>();
                    }
                    catch (Exception e) { continue; }
                    if (hex.isCountry == false && !countryExists && hex.nationNum != 0 && chex.nationNum == hex.nationNum)
                    {
                        hex.isCountry = true;
                        hexagon.transform.SetParent("Country" + countryNumber);
                        countryNumber++;
                        print("New country at " + HexPosX + " , " + HexPosY);
                    }
                }
    HexPosX++;
    }
    HexPosY++;
  }
  
  for (int i = 0; i>=HexColumn; i++) {
    HexPosX = 0;
    for(int j = 0; j>=HexRow; j++) {
        hexagon = GameObject.Find("Hexagon " + HexPosX + " , " + HexPosY);
        hex = hexagon.GetComponent<Hexagon>();
        foreach(GameObject c in hex.adj){
                    try {
                        chex = c.GetComponent<Hexagon>();
                    }
                    catch (Exception e){
                        continue;
                    }
          if (chex.nationNum == hex.nationNum && c.transform.parent != hexagon.t){
                        GameObject[] countryFixer = c.GetComponentsInChildren<Hexagon>();
                    foreach (GameObject p in countryFixer){
                            p.transform.parent = hexagon.transform.parent;
                            print("fixed!");
            }
        }
      }
    }
            print("second sweep complete");
  }
        print(countryNumber);
}
}
