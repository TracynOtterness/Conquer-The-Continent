using System;
using UnityEngine;
using System.Collections.Generic;

public class Country : MonoBehaviour
{

    public int unitCount;
    public int food;
    public int ymax;
    public int ymin = 76;
    public int xmax;
    public int xmin = 50;
    public List<Hexagon> hexagons;
    public List<Person> people;
    public List<Ship> ships;
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
    public bool hasHarbor;
    public bool isInvasionCountry;
    float time = .5f;
    public int invasionFishTurns = 5;

    void Awake()
    {
        this.name = "Country " + (Instantiator.countryCount); 
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
                if (!isInvasionCountry){
                    if (capital.village.GetComponent<SpriteRenderer>().enabled) capital.village.GetComponent<SpriteRenderer>().enabled = false;
                    else capital.village.GetComponent<SpriteRenderer>().enabled = true;
                    time = .5f; 
                }
                else{
                    if (capital.shipCapital.GetComponent<SpriteRenderer>().enabled) capital.shipCapital.GetComponent<SpriteRenderer>().enabled = false;
                    else capital.shipCapital.GetComponent<SpriteRenderer>().enabled = true;
                    time = .5f; 
                }
            }
        }
	}
	public void UpdateArray()
    {
        people.Clear();
        ships.Clear();
        hexagons.Clear();
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.tag.Equals("hex"))
            {
                hexagons.Add(child.GetComponent<Hexagon>());
            }
            else if (child.tag.Equals("person"))
            {
                //print(child);
                people.Add(child.GetComponent<Person>());
            }
            else if (child.tag.Equals("ship")){
                ships.Add(child.GetComponent<Ship>());
            }
        }
        if (!isInvasionCountry){
            if (hexagons.Count < 2)
            {
                UpdateArrayAndCountrySplitterConflictFixer = false; //I want to get rid of this bool deal if possible
                Invoke("RemoveFromCountryLists", .1f);
            }   
        }
        //might need an alternative route to destruction for invasion countries here
    }
    public void DeleteIfEmpty() //this is unneccesarry, can just call RemoveCountryLists
    {
        if (hexagons.Count == 0)
        {
            Invoke("RemoveFromCountryLists", .1f);
        }
    }
    void RemoveFromCountryLists(){
        //print(this);
        if (!Instantiator.spawningCountries){ Starve();}
        foreach(Hexagon h in hexagons){
            h.transform.SetParent(Instantiator.nonHexFolder.transform);
            h.isCountry = false;
            if (h.hasCapital)
            {
                h.hasCapital = false;
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
       // if (isInvasionCountry) { print("Hello"); }
        foreach (Hexagon h in hexagons)
        {
            if (h.hasGrave == false) food++;
        }
        try
        {
            //print(this + " has " + people.Count + " people");
            foreach (Person p in people)
            {
                //print(p);
                if (!p.isSailor)
                {
                    food -= 2 * (int)Math.Pow(3, (p.tier - 1));
                }
                else
                {
                    if (invasionFishTurns > 0 && p.tier == 1)
                    {
                        food += Ship.fishValue;
                    }
                    food -= Ship.sailorFoodConsumption;
                }
            }
            invasionFishTurns--;
            if (food < 0)
            {
                Starve();
                food = hexagons.Count;
            }
        }
        catch (Exception) { print("Caught Exception"); }
    }
    public void UpdateGuard(){
        print("UpdateGuard for " + this);
        foreach(Hexagon h in hexagons)
        {
            h.guardedBy = 0;
        }
        foreach (Hexagon h in hexagons){
            if (h.hasCapital || h.hasHarbor && h.guardedBy < 1)
            {
                h.guardedBy = 1;
            }
            if(h.hasCastle && h.guardedBy < 2){
                print("castle located");
                h.guardedBy = 2;  
                foreach (GameObject g in h.adj)
                {
                    if (g.transform.parent == h.transform.parent)
                    {
                        print("1");
                        Hexagon adjHexScript = g.GetComponent<Hexagon>();
                        print(adjHexScript);
                        if (adjHexScript.guardedBy < 2) {
                            print("2");
                            adjHexScript.guardedBy = 2; 
                        }
                    }
                }
            }
            if(h.hasGuard){
                if(h.guardedBy < h.guard.tier){
                    h.guardedBy = h.guard.tier;  
                }
                foreach (GameObject g in h.adj)
                {
                    if (g.transform.parent == h.transform.parent)
                    {
                        Hexagon adjHexScript = g.GetComponent<Hexagon>();
                        if (adjHexScript.guardedBy < h.guard.tier) adjHexScript.guardedBy = h.guard.tier;
                    }
                }
            }
        }
    }
    public void SetCapital(Hexagon chosenHexagon = null)
    {
        print("SetCapital");
        GameObject capitalObject;
        if(chosenHexagon == null){
            //this could probably be a little bit optimized?
            if (hexagons.Count >= 2)
            {
                int b = 0;
                for (int i = 0; i <= 3; i++)
                {
                    b = UnityEngine.Random.Range(0, hexagons.Count);
                    if (hexagons[b].hasGuard == false && hexagons[b].hasCastle == false && hexagons[b].hasHarbor == false && hexagons[b].hasGrave == false)
                    {
                        print("found a spot by chance");
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
                    if (h.hasGuard == false && h.hasCastle == false && h.hasHarbor == false && h.hasGrave == false)
                    {
                        print("found a spot via elimination");
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
                //this happens if every single hexagon has something on it
                Starve();
                b = UnityEngine.Random.Range(0, hexagons.Count);
                capital = hexagons[b];
                print("forcing a spot to me made on " + capital);
                capitalObject = (GameObject)Instantiate(Resources.Load("Capital"), new Vector3(capital.transform.position.x, capital.transform.position.y, 0f), Quaternion.identity);
                capitalObject.transform.SetParent(capital.transform);
                capital.AddCapital();
                Destroy(capital.grave);
                Destroy(capital.castle);
                Destroy(capital.harbor);
            }
        }
        else{
            if (isInvasionCountry)
            {
                this.capital.shipCapital.GetComponent<SpriteRenderer>().enabled = true;
                this.capital.shipCapital.GetComponent<Ship>().moored = false;
                foreach (Person p in this.capital.shipCapital.GetComponent<Ship>().sailors)
                {
                    p.transform.SetParent(this.capital.shipCapital.transform);
                    p.hexCountry = null;
                }
                this.capital.shipCapital = null;
                this.isInvasionCountry = false;
            }
            capital.RemoveCapital();
            capital = chosenHexagon;
            capitalObject = (GameObject)Instantiate(Resources.Load("Capital"), new Vector3(capital.transform.position.x, capital.transform.position.y, 0f), Quaternion.identity);
            capitalObject.transform.SetParent(capital.transform);
            capital.AddCapital();
            capital.EditFlag();

        }
     }
    public void CheckCountrySplit()
    {
        print("CheckCountrySplit being ran for " + this.gameObject);
        int runsCount = 0; //how many runs that have found a new country without the old capital.
        currentRunHexes.Clear();

        foreach (Hexagon h in hexagons) //hexagons is from the country from which CheckCountrySplit is ran
        {
            if (visitedHexes.Contains(h))
            {
                print("visitedHexagons contains " + h);
                continue;
            }
            print(h + "is the starting point for this run");
            activeHexes.Add(h);
            while (CheckIncomplete)
            {
                RunCheck();
            } 
            CheckIncomplete = true;
            print("The run beginning with "+h+" contained " + runCount + " hexagons out of " + hexagons.Count + " in the parent country");
            if (runCount == hexagons.Count){
                activeHexes.Clear();
                visitedHexes.Clear();
                runCount = 0;
                return; 
            } 
            else
            {
                if (capitalRun)
                {
                    legacyHexes =  new List<Hexagon>(currentRunHexes);
                    capitalRun = false;
                    currentRunHexes.Clear();
                }
                else
                {
                    print("Non capital run");
                    switch (runsCount)
                    {
                        case 0: newCountryHexes1 = new List<Hexagon>(currentRunHexes); break;
                        case 1: newCountryHexes2 = new List<Hexagon>(currentRunHexes); break;
                        case 2: legacyHexes = new List<Hexagon>(currentRunHexes); specialTripleSplit = true; print("specialTripleSplit happened"); break;
                    }
                    runsCount++;
                }
                currentRunHexes.Clear();
                runCount = 0;
            }
        }
        print(runsCount); 
        CreateCountryOne();
        if (runsCount >= 2){
            CreateCountryTwo(); 
        }
        UpdateLegacyCountry();
        visitedHexes.Clear();
        legacyHexes.Clear();
        newCountryHexes1.Clear();
        newCountryHexes2.Clear();
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
                        print("added " + gScript + "to activeHexes");
                        activeHexes.Add(gScript);
                        CheckIncomplete = true;
                    }
                }
                if (gScript.hasShip && gScript.ship.transform.parent == activeHexes[i].transform.parent)
                {
                    capitalRun = true;
                }
            }
            visitedHexes.Add(activeHexes[i]);
            currentRunHexes.Add(activeHexes[i]);
            if (activeHexes[i].hasCapital)
            {
                capitalRun = true;
                print("This run is a capital run");
            }
            activeHexes.Remove(activeHexes[i]);
            runCount++;
        }
    }
    void UpdateLegacyCountry()
    {
        hexagons = new List<Hexagon>(legacyHexes);
        print("LegacyHexes:");
        foreach(Hexagon h in legacyHexes)
        {
            print(h);
        }
        if (specialTripleSplit) {
            print("specialTripleSplit");
            if (hexagons.Count < 2)
            {
                foreach (Hexagon h in hexagons)
                {
                    print(h + "is getting sent to NonHexFolder");
                    h.isCountry = false;
                    h.transform.SetParent(Instantiator.nonHexFolder.transform);
                    if (h.hasGuard) //starve the single guy off
                    {
                        Person singleHexGuy = h.guard;
                        GameObject grave = (GameObject)Instantiate(Resources.Load("Grave"), new Vector3(singleHexGuy.transform.position.x, singleHexGuy.transform.position.y, 0), Quaternion.identity);
                        h.hasGrave = true;
                        h.grave = grave;
                        grave.transform.SetParent(h.transform);
                        grave.name = "grave " + singleHexGuy;
                        h.hasGuard = false;
                        UIManager.allPeople.Remove(singleHexGuy);
                        Destroy(singleHexGuy.gameObject);
                        UIManager.SpawnRedDots();
                    }
                    if (h.hasCapital)
                    {
                        h.RemoveCapital();
                    }
                    h.guardedBy = 0;
                }
                print("removeFromCountryLists");
                Invoke("RemoveFromCountryLists", .1f);
            }
            food = 0;
        }
        else if (hexagons.Count < 2)
        {
            foreach (Hexagon h in hexagons)
            {
                print(h + "is getting sent to NonHexFolder");
                h.isCountry = false;
                h.transform.SetParent(Instantiator.nonHexFolder.transform);
                if (h.hasGuard) //starve the single guy off
                {
                    Person singleHexGuy = h.guard;
                    GameObject grave = (GameObject)Instantiate(Resources.Load("Grave"), new Vector3(singleHexGuy.transform.position.x, singleHexGuy.transform.position.y, 0), Quaternion.identity);
                    h.hasGrave = true;
                    h.grave = grave;
                    grave.transform.SetParent(h.transform);
                    grave.name = "grave " + singleHexGuy;
                    h.hasGuard = false;
                    UIManager.allPeople.Remove(singleHexGuy);
                    Destroy(singleHexGuy.gameObject);
                    UIManager.SpawnRedDots();
                }
                if (h.hasCapital)
                {
                    h.RemoveCapital();
                }
                h.guardedBy = 0;
            }
        }
        UpdateArray();
        UpdateGuard();
    }
    void CreateCountryOne()
    {
        print("createCountryOne");
        if (newCountryHexes1.Count < 2){ //single hexagon cut off
            foreach(Hexagon h in newCountryHexes1){
                print(h + "is getting sent to NonHexFolder");
                h.isCountry = false;
                h.transform.SetParent(Instantiator.nonHexFolder.transform);
                if (h.hasGuard) //starve the single guy off
                {
                    Person singleHexGuy = h.guard;
                    GameObject grave = (GameObject)Instantiate(Resources.Load("Grave"), new Vector3(singleHexGuy.transform.position.x, singleHexGuy.transform.position.y, 0), Quaternion.identity);
                    h.hasGrave = true;
                    h.grave = grave;
                    grave.transform.SetParent(h.transform);
                    grave.name = "grave " + singleHexGuy;
                    h.hasGuard = false;
                    UIManager.allPeople.Remove(singleHexGuy);
                    Destroy(singleHexGuy.gameObject);
                    UIManager.SpawnRedDots();
                }
                if (h.hasCapital)
                {
                    h.RemoveCapital();
                }
                h.guardedBy = 0;
            }
            return;
        }
        GameObject country = (GameObject)Instantiate(Resources.Load("Country"), new Vector3(0, 0, 0), Quaternion.identity);
        Country c = country.GetComponent<Country>();
        print(country);
        print("NewHexes1:");
        foreach(Hexagon h in newCountryHexes1)
        {
            print(h);
        }
        c.hexagons = new List<Hexagon>(newCountryHexes1);
        Instantiator.countryscripts.Add(c);
        foreach (Hexagon h in c.hexagons)
        {
            h.gameObject.transform.SetParent(country.transform);
            if(h.hasCastle){
                h.castle.transform.SetParent(country.transform);
            }
            if (h.hasHarbor)
            {
                h.harbor.transform.SetParent(country.transform);
            }
        }
        foreach(Person p in people){
            Hexagon h = p.hexagon;
            p.transform.SetParent(h.transform.parent);
        }
        c.UpdateArray();
        c.UpdateGuard();
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
                if (h.hasGuard) //starve the single guy off
                {
                    Person singleHexGuy = h.guard;
                    GameObject grave = (GameObject)Instantiate(Resources.Load("Grave"), new Vector3(singleHexGuy.transform.position.x, singleHexGuy.transform.position.y, 0), Quaternion.identity);
                    h.hasGrave = true;
                    h.grave = grave;
                    grave.transform.SetParent(h.transform);
                    grave.name = "grave " + singleHexGuy;
                    h.hasGuard = false;
                    UIManager.allPeople.Remove(singleHexGuy);
                    Destroy(singleHexGuy.gameObject);
                    UIManager.SpawnRedDots();
                }
                if (h.hasCapital)
                {
                    h.RemoveCapital();
                }
                h.guardedBy = 0;
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
            if (h.hasCastle)
            {
                h.castle.transform.SetParent(country.transform);
            }
            if (h.hasHarbor)
            {
                h.harbor.transform.SetParent(country.transform);
            }
        }
        foreach (Person p in people)
        {
            Hexagon h = p.hexagon;
            p.transform.SetParent(h.transform.parent);
        }
        c.UpdateArray();
        c.UpdateGuard();//just put this here
        c.SetCapital();
    }

    void Starve(){
        print("starving the units in " + this);
        foreach(Person p in people){
            print(p + " from nationNumber " + p.nationNum);
            if (p != null){
                GameObject grave = (GameObject)Instantiate(Resources.Load("Grave"), new Vector3(p.transform.position.x, p.transform.position.y, 0), Quaternion.identity);
                p.hexagon.hasGrave = true;
                p.hexagon.grave = grave;
                grave.transform.SetParent(p.hexagon.transform);
                grave.name = "grave " + p;
                p.hexagon.hasGuard = false;
                p.hexagon.guardedBy = 0;
                UIManager.allPeople.Remove(p);
                Destroy(p.gameObject);  
            }
        }
        UIManager.SpawnRedDots();
        people.Clear();
        Invoke("UpdateArray", .1f);
        Invoke("UpdateGuard", .2f);
    }
       public int GiveIncome(){
        int i = 0;
        foreach (Hexagon h in hexagons){
            if (!h.hasGrave) {
                i++;
            }
        }
        if (isInvasionCountry && invasionFishTurns > 0)
        {
            foreach (Person p in people)
            {
                if (p.isSailor && p.tier == 1)
                {
                    i += Ship.fishValue;
                } 
            }
        }
            return i;
        }
}
