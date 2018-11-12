using System.Collections.Generic;
using System;
using UnityEngine;
public class Instantiator : MonoBehaviour
{
    private Hexagon hex; //our link to the hexagon script
    private Hexagon chex;
    public static int HexPosX;//the inital x position of the instantiator on the hexagon grid
    public static int HexPosY;//the inital y position of the instantiator on the hexagon grid
    public static int size = 3;
    private static int HexColumn = 36; // how many columns there are (basically the x height of the hexagon grid)
    private static int HexRow = 60; // how many rows there are (basically the length of the hexagon grid)
    int step = 200; //how many steps it takes in each walk
    int maxHexes = (int)(HexColumn * HexRow * .2632f); //the minimum number of land hexagons that will be on a map
    private int countryNumber = 1;
    public static int countryCount = 1;
    private GameObject hexagon; // a gameobject we use for instantiation as well as Landmaster()
    private float x; //the coordinates in the gameworld that each hexagon will be instantiated at on runtime
    private float y;
    public static int totalHexes; // the total hexagons that have been turned into land. Needs to be here because it is changed by a method in hexagon
    private bool countryExists;
    private int maxCountries = 500;
    public static List<GameObject> countries;
    public static List<Country> countryscripts;
    private GameObject[] countries1;
    public static GameObject nonHexFolder;
    public static bool spawningCountries = true;

    void Start()
    { //everything between here and when it calls Landmaster() is creating the grid itself
        nonHexFolder = GameObject.Find("Non-Country hexagons");
        countries = new List<GameObject>();
        countryscripts = new List<Country>();
        x = 0;
        y = 0;
        for (int i = 0; i <= HexColumn; i++)
        {
            x = 0;
            HexPosX = 0;
            for (int j = 0; j <= HexRow; j++)
            {
                hexagon = (GameObject)Instantiate(Resources.Load("Hexagon"), new Vector3(x, y, 0f), Quaternion.identity);
                transform.position = new Vector3(x, y, 0f);
                HexPosX++;
                x += .459f;
            }
            HexPosY++;
            y += .56f;
        }
        for (int l = 0; l <= maxCountries; l++)
        {
            GameObject country = (GameObject)Instantiate(Resources.Load("Country"), new Vector3(0, 0, 0), Quaternion.identity);
            countries.Add(country);
            countryscripts.Add(country.GetComponent<Country>());
        }
        LandMaster(); // changes some hexagoncountryscripts to land, generates the random map each time.
        Invoke("CountryMaster", .2f);
    }
    public void LandMaster()
    {
        int b = 0; // used in switch statements later on
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
                        b = UnityEngine.Random.Range(2, 4);
                        break;
                    case 2:
                        b = UnityEngine.Random.Range(0, 4);
                        break;
                    case 3:
                        b = UnityEngine.Random.Range(3, 5);
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
        countryNumber = 1;
        HexPosY = 0;
        for (int i = 0; i <= HexColumn; i++)
        {
            HexPosX = 0;

            for (int j = 0; j <= HexRow; j++)
            {
                GameObject country = countries[countryNumber];
                countryExists = false;
                hexagon = GameObject.Find("Hexagon " + HexPosX + " , " + HexPosY);
                hex = hexagon.GetComponent<Hexagon>();
                foreach (GameObject c in hex.adj)
                {
                    try
                    {
                        chex = c.GetComponent<Hexagon>();
                    }
                    catch (Exception) { continue; }

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
                    catch (Exception) { continue; }
                    if (hex.isCountry == false && !countryExists && hex.nationNum != 0 && chex.nationNum == hex.nationNum)
                    {
                        hex.isCountry = true;
                        hexagon.transform.SetParent(country.transform);
                        countryNumber++;
                    }
                }
    HexPosX++;
    }
    HexPosY++;
  }
        HexPosY = 0;
  for (int i = 0; i<=HexColumn; i++) {
    HexPosX = 0;
    for(int j = 0; j<=HexRow; j++) {
        hexagon = GameObject.Find("Hexagon " + HexPosX + " , " + HexPosY);
        hex = hexagon.GetComponent<Hexagon>();
        foreach(GameObject c in hex.adj){
                    try {
                        chex = c.GetComponent<Hexagon>();
                    }
                    catch (Exception){
                        continue;
                    }
          if (chex.nationNum == hex.nationNum && c.transform.parent != hexagon.transform.parent){
                        Hexagon[] countryFixer = c.transform.parent.GetComponentsInChildren<Hexagon>();
                    foreach (Hexagon p in countryFixer){
                            p.transform.parent = hexagon.transform.parent;
            }
        }
      }
                HexPosX++;
    }
            HexPosY++;
           
  }

        foreach (Country c in countryscripts){
            c.UpdateArray();
            c.DeleteIfEmpty();
            c.SetCapital();
            c.AssignBeginningFood();
        }
        GameObject[] allHexes = GameObject.FindGameObjectsWithTag("hex");
        foreach (GameObject g in allHexes) {
            if (g.GetComponent<Hexagon>().hasCapital) g.GetComponent<Hexagon>().EditFlag();
            if (g.GetComponent<Hexagon>().isCountry == false){
                g.transform.SetParent(nonHexFolder.transform);
            }
        }
        spawningCountries = false;
    }
}
