using System;
using UnityEngine;
using System.Collections.Generic;

public class Country : MonoBehaviour
{
    public List<Hexagon> hexagons;
    public int unitCount;
    public int food;
    public int ymax;
    public int ymin = 76;
    public int xmax;
    public int xmin = 50;
    public List<Person> people;
    List<Hexagon> legacyHexes; //the list to store the legacy country's hexagons
    List<Hexagon> newCountryHexes1; //the list to store the 2nd country's hexagons
    List<Hexagon> newCountryHexes2; //the list to store the 3rd country's hexagons if neccesarry
    List<Hexagon> currentRunHexes; //hexagons in the current run
    int runCount; //how many hexagons have been found, if this = hexagons.Count after 1 run there is no split
    List<Hexagon> activeHexes; //the hexagons currently "active" in the run algorithm
    List<Hexagon> visitedHexes; //an array of hexes that have already been touched by the algorithm. This is here so hexes don't get checked twice.
    bool CheckIncomplete = true; //this is the key to stopping the run once there are no longer any hexagons it can move to.
    bool capitalRun; //this is the bool that helps determine which of the new countries formed has the capital of the old one in it, and the one that will keep the scripts/name of the parent.
    public Hexagon capital;
    public bool UpdateArrayAndCountrySplitterConflictFixer = true;
    bool specialTripleSplit = false;
    public bool isSelected;
    float time = .5f;

    void Awake()
    {
        this.name = "Country " + (Instantiator.countryCount); //not sure why I need to do the -1000 thing here, debug it later
        Instantiator.countryCount++;
        this.transform.SetParent(GameObject.Find("Master").transform);
    }
	void Start()
	{
        legacyHexes = new List<Hexagon>();
        newCountryHexes1 = new List<Hexagon>();
        newCountryHexes2 = new List<Hexagon>();
        currentRunHexes = new List<Hexagon>();
        activeHexes = new List<Hexagon>();
        visitedHexes = new List<Hexagon>();


	}
	private void Update()
	{
        if(isSelected){
            time -= Time.deltaTime;
            if (time <= 0){
                if (capital.village.GetComponent<SpriteRenderer>().enabled) capital.village.GetComponent<SpriteRenderer>().enabled = false;
                else capital.village.GetComponent<SpriteRenderer>().enabled = true;
                time = .5f;
            }
        }
	}
	public void UpdateArray()
    {
        people.Clear();
        hexagons.Clear();
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.tag.Equals("hex"))
            {
                hexagons.Add(child.GetComponent<Hexagon>());
            }
            else if (child.tag.Equals("person")) {
                people.Add(child.GetComponent<Person>());
            }
        }

        if (hexagons.Count < 2) {
            UpdateArrayAndCountrySplitterConflictFixer = false; //I want to get rid of this bool deal if possible
            Invoke("RemoveFromCountryLists", .1f);
        }
    }
    public void DeleteIfEmpty() //this is unneccesarry, can just call RemoveCountryLists
    {
        if (hexagons.Count == 0)
        {
            Invoke("RemoveFromCountryLists", .1f);
        }
    }
    void RemoveFromCountryLists(){
        Starve();
        foreach(Hexagon h in hexagons){
            h.transform.SetParent(Instantiator.nonHexFolder.transform);
            h.isCountry = false;
            if (h.hasCapital)
            {
                Destroy(h.village);
                h.guardedBy = 0;
            }
        }
        Instantiator.countryscripts.Remove(this.GetComponent<Country>());
        Instantiator.countries.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
    public void AssignBeginningFood(){
        food = hexagons.Count * 5;
    }
    public void GiveRoundFood(){
        foreach (Hexagon h in hexagons)
        {
            if (h.hasGrave == false) food++;
        }
        try{
            foreach (Person p in people)
            {
                food -= 2 * (int)Math.Pow(3, (p.tier - 1));
                if (food < 0) {
                    Starve();
                    food = hexagons.Count;
                }
            }  

        }
        catch(Exception){}
    }
    public void UpdateGuard(){
        foreach (Hexagon h in hexagons){
            if (h.hasGuard == false) h.guardedBy = 0;
        }
        foreach (Hexagon h in hexagons){
            if (h.hasGuard){
                foreach (GameObject g in h.adj)
                {
                    if (g.transform.parent == h.transform.parent) {
                        Hexagon adjHexScript = g.GetComponent<Hexagon>();
                        if (adjHexScript.guardedBy < h.guardedBy) adjHexScript.guardedBy = h.guardedBy; 
                    }
                }
            }
        }
    }
    public void SetCapital()
    {
        //this could probably be a little bit optimized?
        if (hexagons.Count >= 2)
        {
            GameObject capitalObject;
            int b = 0;
            for (int i = 0; i <= 3; i++)
            {
                b = UnityEngine.Random.Range(0, hexagons.Count);
                if (hexagons[b].hasGuard == false)
                {
                    try
                    {
                        capital = hexagons[b];
                        capitalObject = (GameObject)Instantiate(Resources.Load("Capital"), new Vector3(capital.transform.position.x, capital.transform.position.y, 0f), Quaternion.identity);
                    }
                    catch (Exception) { return; }
                    capitalObject.transform.SetParent(capital.transform);
                    capital.AddCapital();
                    return;
                }
            }
            foreach (Hexagon h in hexagons)
            { //this whole loop is for finding a hexagon to put the capital on if it can't find a random one in 3 tries.
                if (h.hasGuard == false)
                {
                    try
                    {
                        capital = h;
                        capitalObject = (GameObject)Instantiate(Resources.Load("Capital"), new Vector3(capital.transform.position.x, capital.transform.position.y, 0f), Quaternion.identity);
                    }
                    catch (Exception) { return; }
                    capitalObject.transform.SetParent(capital.transform);
                    capital.AddCapital();
                    return;
                }
            }
            Starve();
            b = UnityEngine.Random.Range(0, hexagons.Count);
            capital = hexagons[b]; //maybe I need the try/catch maybe I don't?
            capitalObject = (GameObject)Instantiate(Resources.Load("Capital"), new Vector3(capital.transform.position.x, capital.transform.position.y, 0f), Quaternion.identity);
            capitalObject.transform.SetParent(capital.transform);
            capital.AddCapital();
            Destroy(capital.grave);
        }
     }
    public void CheckCountrySplit()
    {
        print("CheckCountrySplit being ran for " + this.gameObject);
        int runsCount = 0; //how many runs that have found a new country without the old capital.
        bool CountrySplitted = true; //control to make sure if it is split the check to stop it if it isn't split doesn't work.

        foreach (Hexagon h in hexagons) //hexagons is from the country from which CheckCountrySplit is ran

        {
            if (visitedHexes.Contains(h))
            {
                continue;
            }
            activeHexes.Add(h);
            while (CheckIncomplete)
            {
                RunCheck();
            } 
            CheckIncomplete = true;
            if (runCount == hexagons.Count && CountrySplitted){
                return; //have a boolean make this only work on run 1.
            } 
            else
            {
                CountrySplitted = false;
                if (capitalRun)
                {
                    legacyHexes =  new List<Hexagon>(currentRunHexes);
                    capitalRun = false;
                    currentRunHexes.Clear();
                }
                else
                {
                    switch (runsCount)
                    {
                        case 0: newCountryHexes1 = new List<Hexagon>(currentRunHexes); break;
                        case 1: newCountryHexes2 = new List<Hexagon>(currentRunHexes); break;
                        case 2: legacyHexes = new List<Hexagon>(currentRunHexes); specialTripleSplit = true; print("specialTripleSplit happened"); break;
                    }
                    runsCount++;
                }
                currentRunHexes.Clear();
            }
        }
        print(runsCount); 
        CreateCountryOne();
        if (runsCount >= 2){
            CreateCountryTwo(); 
        }
        UpdateLegacyCountry();
    }

    void RunCheck()
    {
        CheckIncomplete = false;
        for (int i = (activeHexes.Count - 1); i >= 0; i--)
        {
            foreach (GameObject g in activeHexes[i].adj)
            {
                Hexagon gScript = g.GetComponent<Hexagon>();
                if (gScript.transform.parent == activeHexes[i].transform.parent){
                    if (activeHexes.Contains(gScript) == false && currentRunHexes.Contains(gScript) == false)
                    {
                        activeHexes.Add(gScript);
                        CheckIncomplete = true;
                    }
                }
            }
            visitedHexes.Add(activeHexes[i]);
            currentRunHexes.Add(activeHexes[i]);
            if (activeHexes[i].hasCapital)
            {
                capitalRun = true;

            }
            activeHexes.Remove(activeHexes[i]);
            runCount++;
        }
    }
    void UpdateLegacyCountry()
    {
        hexagons = new List<Hexagon>(legacyHexes);
        UpdateArray();
        if (specialTripleSplit) {
            print("specialTripleSplit");
            if (hexagons.Count < 2)
            {
                foreach (Hexagon h in hexagons)
                {
                    h.isCountry = false;
                    h.transform.SetParent(Instantiator.nonHexFolder.transform);
                }
                RemoveFromCountryLists();
            }
            food = 0;
        }
    }
    void CreateCountryOne()
    {
        if (newCountryHexes1.Count < 2){
            foreach(Hexagon h in newCountryHexes1){
                h.isCountry = false;
                h.transform.SetParent(Instantiator.nonHexFolder.transform);
            }
            return;
        }
        GameObject country = (GameObject)Instantiate(Resources.Load("Country"), new Vector3(0, 0, 0), Quaternion.identity);
        Country c = country.GetComponent<Country>();
        c.hexagons = new List<Hexagon>(newCountryHexes1);
        Instantiator.countryscripts.Add(c);
        foreach (Hexagon h in c.hexagons)
        {
            h.gameObject.transform.SetParent(country.transform);
        }
        foreach(Person p in people){
            Hexagon h = p.hexagon;
            p.transform.SetParent(h.transform.parent);
        }
        c.UpdateArray();
        c.SetCapital();
    }
    void CreateCountryTwo()
    {
        print("CreateCountryTwo");
        if (newCountryHexes2.Count < 2)
        {
            foreach (Hexagon h in newCountryHexes2)
            {
                h.isCountry = false;
                h.transform.SetParent(Instantiator.nonHexFolder.transform);
            }
            return;
        }
        GameObject country = (GameObject)Instantiate(Resources.Load("Country"), new Vector3(0, 0, 0), Quaternion.identity);
        Country c = country.GetComponent<Country>();
        c.hexagons = new List<Hexagon>(newCountryHexes2);
        Instantiator.countryscripts.Add(c);
        foreach (Hexagon h in newCountryHexes2)
        {
            h.transform.SetParent(country.transform);
        }
        foreach (Person p in people)
        {
            Hexagon h = p.hexagon;
            p.transform.SetParent(h.transform.parent);
        }
        c.UpdateArray();
        c.SetCapital();
    }

    void Starve(){
        foreach(Person p in people){
            if (p != null){
                GameObject grave = (GameObject)Instantiate(Resources.Load("Grave"), new Vector3(p.transform.position.x, p.transform.position.y, 0), Quaternion.identity);
                p.hexagon.hasGrave = true;
                p.hexagon.grave = grave;
                grave.transform.SetParent(p.hexagon.transform);
                p.hexagon.hasGuard = false;
                p.hexagon.guardedBy = 0;
                UIManager.allPeople.Remove(p);
                Destroy(p.gameObject);  
            }
        }
        UIManager.SpawnRedDots();
        people.Clear();
    }
       public int GiveIncome(){
            int i = 0;
            foreach (Hexagon h in hexagons){
                if (!h.hasGrave) i++;
            }
            return i;
        }
}
