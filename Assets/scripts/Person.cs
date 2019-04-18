using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Person : MonoBehaviour
{

    public int tier = 0;
    public int nationNum = 5;
    float timer = 0;
    int currentStep = 0;
    private Vector3 pos;
    public bool hover;
    public GameObject hex;
    public Hexagon hexagon;
    public Ship ship;
    private Hexagon[] hexagons;
    public GameObject unusedHexMaster;
    private Hexagon oldHexagon;
    public Country hexCountry;
    public Sprite[] sprites;
    public bool hasUsedMove = false;
    public bool isSailor = false;
    public bool usingFirstTurn = true;
    bool doMarch = false;
    public GameObject redDot = null;
    List<Hexagon> pathHexes;
    Vector2 hexDistance;
    static float time = .5f;
    public SpriteRenderer sprite;

    void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("People");
        pathHexes = new List<Hexagon>();
        this.nationNum = UIManager.turnNumber;
        this.name = "Unit " + UIManager.selectedCountry + " , " + UIManager.selectedCountry.GetComponent<Country>().unitCount;
        this.transform.SetParent(UIManager.selectedCountry.transform);
        sprite = this.GetComponent<SpriteRenderer>();
        UIManager.selectedCountryScript.unitCount++;
    }
    void Update()
    {
        
        if (hover && UIManager.hoverMode) {
            transform.position = FindObjectOfType<Mouse>().transform.position;
        }

        if (doMarch){
            timer += Time.deltaTime;
            if(timer >= .5f){
                this.transform.position = pathHexes[currentStep + 1].transform.position;
                timer = 0f;
                currentStep++;
                if (currentStep + 1 > pathHexes.Count - 1 ){
                    doMarch = false;
                    Place();
                }
                else{
                    hexDistance = new Vector2((pathHexes[currentStep + 1].transform.position.x - pathHexes[currentStep].transform.position.x), (pathHexes[currentStep + 1].transform.position.y - pathHexes[currentStep].transform.position.y));
                }
            }
            else{
                this.transform.position = new Vector3((pathHexes[currentStep].transform.position.x + 2 * timer * hexDistance.x), (pathHexes[currentStep].transform.position.y + 2 * timer * hexDistance.y));//transform = hex that person is coming from + percentage of time to .5 seconds * hexdistance; 
            }
        }

    }

    public void MovePerson(GameObject gameobject){
        hex = gameobject;
        hexagon = gameobject.GetComponent<Hexagon>();
        this.GetComponent<CircleCollider2D>().radius = .175f;
        hexCountry = this.transform.parent.gameObject.GetComponent<Country>();
        if (UIManager.hoverMode)
        {
            pos = hex.transform.position;
            this.transform.position = pos;
            pos.z = -1;
            Place();
        }
        else
        {
            GetPath();
            if (pathHexes.Count == 1){//no marching is needed if you place the unit on the hexagon it started on
                Place();
            }
            else{
                pathHexes.Reverse();
                foreach (Hexagon h in pathHexes)
                {
                    print(h);
                }
                hexDistance = new Vector2((pathHexes[1].transform.position.x - pathHexes[0].transform.position.x), (pathHexes[1].transform.position.y - pathHexes[0].transform.position.y));
                doMarch = true; 
            }
        }
        ResetViableHexagons();//no matter what, you still need to reset the viable hexagons
    }



    public void Place()
    {
        print("Person.place()");
        if(usingFirstTurn) usingFirstTurn = false;
        if (redDot != null){
            redDot.transform.position = pos;
            if (UIManager.toggled == true) redDot.layer = 0;
        }
        hover = false;
        if (hex.transform.parent != this.transform.parent) //hexagon being placed on is not in the same country as the unit
        {
            hexagon.isCountry = true;
            foreach (GameObject g in hexagon.adj) //this loop checks to see if countries are being combined
            {
                Hexagon hexa = g.GetComponent<Hexagon>();
                if (hexa.nationNum == this.nationNum && g.transform.parent != this.gameObject.transform.parent) //this handles what happens when it finds a hexagon of the same color
                {
                    if (hexa.isCountry) //if found hexagon of same color is part of a country
                    {
                        Country originalCountry = g.transform.parent.GetComponent<Country>();
                        Country newCountry = hexCountry;
                        if (originalCountry.isInvasionCountry && !newCountry.isInvasionCountry){
                            originalCountry.capital.RemoveCapital();
                            originalCountry.capital.shipCapital.GetComponent<Ship>().moored = false;
                            foreach (Person p in originalCountry.capital.shipCapital.GetComponent<Ship>().sailors)
                            {
                                p.transform.SetParent(originalCountry.capital.shipCapital.transform);
                                p.hexCountry = null;
                            }
                            originalCountry.UpdateArray();
                            if (originalCountry.capital.shipCapital.GetComponent<Ship>().sailors.Count != 0) { print("Your ship is no longer the capital of the country, you need to build a harbor to unload any extra troops from the ship."); }
                            originalCountry.capital.shipCapital = null;
                        }
                        else if (newCountry.isInvasionCountry && !originalCountry.isInvasionCountry){
                            newCountry.food = originalCountry.food;
                            newCountry.capital.RemoveCapital();
                            newCountry.capital.shipCapital.GetComponent<SpriteRenderer>().enabled = true;
                            newCountry.capital.shipCapital.GetComponent<Ship>().moored = false;
                            foreach (Person p in newCountry.capital.shipCapital.GetComponent<Ship>().sailors)
                            {
                                p.transform.SetParent(newCountry.capital.shipCapital.transform);
                                p.hexCountry = null;
                            }
                            newCountry.UpdateArray();
                            if (newCountry.capital.shipCapital.GetComponent<Ship>().sailors.Count != 0) { print("Your ship is no longer the capital of the country, you need to build a harbor to unload any extra troops from the ship."); }
                            newCountry.capital.shipCapital = null;
                            newCountry.capital = originalCountry.capital;
                            newCountry.isInvasionCountry = false;
                        }
                        else if (originalCountry.food > newCountry.food){
                            newCountry.food = originalCountry.food;
                            newCountry.capital.RemoveCapital();
                            if (newCountry.isInvasionCountry){
                                newCountry.capital.shipCapital.GetComponent<SpriteRenderer>().enabled = true;
                                newCountry.capital.shipCapital.GetComponent<Ship>().moored = false;
                                foreach (Person p in newCountry.capital.shipCapital.GetComponent<Ship>().sailors)
                                {
                                    p.transform.SetParent(newCountry.capital.shipCapital.transform);
                                    p.hexCountry = null;
                                }
                                newCountry.UpdateArray();
                                if (newCountry.capital.shipCapital.GetComponent<Ship>().sailors.Count != 0) { print("Your ship is no longer the capital of the country, you need to build a harbor to unload any extra troops from the ship."); }
                                newCountry.capital.shipCapital = null;
                                newCountry.isInvasionCountry = false;
                            }
                            newCountry.capital = originalCountry.capital;
                            }
                        else
                        {
                            originalCountry.capital.RemoveCapital();
                            if (originalCountry.isInvasionCountry){
                                originalCountry.capital.shipCapital.GetComponent<Ship>().moored = false;
                                foreach (Person p in originalCountry.capital.shipCapital.GetComponent<Ship>().sailors)
                                {
                                    p.transform.SetParent(originalCountry.capital.shipCapital.transform);
                                    p.hexCountry = null;
                                }
                                originalCountry.UpdateArray();
                                if (originalCountry.capital.shipCapital.GetComponent<Ship>().sailors.Count != 0) { print("Your ship is no longer the capital of the country, you need to build a harbor to unload any extra troops from the ship."); }
                                originalCountry.capital.shipCapital = null;
                            }
                        }
                        if (originalCountry.hasHarbor) { newCountry.hasHarbor = true; }

                        Country switchCountry = g.gameObject.transform.parent.GetComponent<Country>();
                        List<Hexagon> hexesToSwitchParent = switchCountry.hexagons;
                        List<Person> unitsToSwitchParent = switchCountry.people;
                        List<Ship> shipsToSwitchParent = switchCountry.ships;

                        foreach (Hexagon h in hexesToSwitchParent)
                        {
                            h.gameObject.transform.SetParent(UIManager.selectedCountry.transform);
                            if (h.hasCastle) h.castle.transform.SetParent(UIManager.selectedCountry.transform);
                            if (h.hasHarbor) h.harbor.transform.SetParent(UIManager.selectedCountry.transform);
                        }
                        foreach (Person p in unitsToSwitchParent)
                        {
                            //print(p);
                            p.gameObject.transform.SetParent(transform.parent.transform);
                        }
                        foreach (Ship s in shipsToSwitchParent){
                            s.gameObject.transform.SetParent(transform.parent.transform);
                        }
                        switchCountry.UpdateArray();
                        if (originalCountry.isSelected) originalCountry.isSelected = false;
                    }
                    else { //adding single hexagon to the country
                        g.transform.SetParent(hexCountry.transform);
                        g.GetComponent<Hexagon>().isCountry = true;
                    }
                }
            }

            //establishes old country and new country
            GameObject OldHexCountry = hex.transform.parent.gameObject;
            Country OldHexCountryScript = OldHexCountry.GetComponent<Country>();
            hex.transform.SetParent(hexCountry.transform);

            //updates arrays of countries
            hexCountry.UpdateArray();
            if (OldHexCountryScript != null){
                OldHexCountryScript.UpdateArray();
                if (OldHexCountryScript.UpdateArrayAndCountrySplitterConflictFixer == true){
                    //print("check country split on " + OldHexCountryScript);
                    OldHexCountryScript.CheckCountrySplit();
                }
            }


            //situation for when you take a capital building
            if (hexagon.hasCapital)
            {
                print("you took a hexagon with a capital on it");
                hexagon.RemoveCapital();
                if (OldHexCountryScript != null)
                {
                    OldHexCountryScript.food = 0;
                    StartCoroutine(SetCapitalWithDelay(OldHexCountryScript));
                }
            }
            //if you take a castle
            if (hexagon.hasCastle)
            {
                hexagon.hasCastle = false;
                Destroy(hexagon.castle);
            }
            //if you take a harbor
            if (hexagon.hasHarbor){
                hexagon.hasHarbor = false;
                Destroy(hexagon.harbor);
            }
            hexagon.nationNum = this.nationNum;
            hexagon.ChangeSprite(this.nationNum);
            UIManager.UpdateStats();


        } //end of if it is taking another country's hexagon

        //update grave status
        if (hexagon.hasGrave)
        {
            Destroy(hexagon.grave);
            hexagon.hasGrave = false;
            UIManager.UpdateStats();
        }
        //end the turn
        hasUsedMove = true;
        sprite.sprite = sprites[tier - 1];
        if (hexagon.hasGuard)
        {
            if (this != hexagon.guard)
            {
                if (hexagon.guard.hasUsedMove == false) hasUsedMove = false;
                int newTier = hexagon.guard.tier + tier;
                UIManager.allPeople.Remove(hexagon.guard);
                UIManager.redDots.Remove(hexagon.guard.redDot);
                this.transform.parent.GetComponent<Country>().people.Remove(hexagon.guard);
                Destroy(hexagon.guard.gameObject);
                UpdateTier(newTier);
                UIManager.UpdateStats();
            }
        }
        //remove guard from old hexagon
        try
        {
            oldHexagon.hasGuard = false;
            oldHexagon.guard = null;
        }
        catch (Exception) { }

        //add guard to new hexagon
        hexagon.hasGuard = true;
        hexagon.guard = this;

        //update the capital's flag
        if (hexCountry.capital.village != null){
            hexCountry.capital.EditFlag();
        }
        //update the rest of the country and set this hexagon to oldhexagon for future use
        hexCountry.UpdateGuard();
        oldHexagon = hexagon;
    }
    public void StartingTier(){
        tier = UIManager.tier;
        sprite.sprite = sprites[tier - 1];
        //tier += 1;
    }
    void UpdateTier(int a){
        tier = a;
        GetComponent<SpriteRenderer>().sprite = sprites[tier - 1];
        }
    public void BoardShip(GameObject g){
        hexagon = g.GetComponent<Hexagon>();
        hexCountry = this.transform.parent.gameObject.GetComponent<Country>();
        hexagon.ship.sailors.Add(this);
        if (usingFirstTurn) { usingFirstTurn = false; }
        isSailor = true;
        hover = false;
        transform.position = new Vector3(0,0,0);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        if (oldHexagon != null){
            oldHexagon.hasGuard = false;
            oldHexagon.guard = null;
        }
        transform.SetParent(hexagon.ship.transform);
        hexCountry.people.Remove(this);
        hexCountry.UpdateGuard();
        UIManager.UpdateStats();
        if (UIManager.showingShipInfo){
            UIManager.ShowBoatInfo(hexagon.ship);
        }
        oldHexagon = null;
        if (redDot != null) redDot.GetComponent<SpriteRenderer>().enabled = false;
    }
    void GetPath(){
        currentStep = 0;
        pathHexes.Clear();
        pathHexes.Add(hexagon);
        Hexagon lastHex = hexagon; 
        for (int i = hexagon.distanceFromStartingPoint; i > 0; i--){
            foreach(GameObject g in lastHex.adj){
                Hexagon h = g.GetComponent<Hexagon>();
                if (h.distanceFromStartingPoint + 1 == i && h.transform.parent == this.transform.parent){
                    print(lastHex + " is adjacent to " + h + ", adding " + h + " to pathhexes");
                    pathHexes.Add(h);
                    lastHex = h;
                    break;
                }
            }
        }
    }
    void ResetViableHexagons(){
        foreach(Hexagon h in Mouse.viableHexagons){
            h.ToggleMask();
            h.distanceFromStartingPoint = -1;
        }
        Mouse.viableHexagons.Clear();
        if(usingFirstTurn){
            foreach(Hexagon h in hexCountry.hexagons){
                if(h.hasCapital || h.hasCastle || h.hasHarbor){
                    h.distanceFromStartingPoint = -1;
                }
            }
        }
        if(hexCountry.isInvasionCountry){
            hexCountry.capital.distanceFromStartingPoint = -1;
        }
    }
    public bool DoAnimation()//returns true if on highlighted sprites
    {
        if (sprite.sprite == sprites[tier - 1])
        {
            sprite.sprite = sprites[tier + 3];
            return true;
        }
        else
        {
            sprite.sprite = sprites[tier - 1];
            return false;
        }
    }
    IEnumerator SetCapitalWithDelay(Country country)
    {
        print("SetCapitalWithDelay");
        yield return new WaitForSeconds(.1f);
        print(country + "SetCapital");
        country.SetCapital();
    }
}
