using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Person : MonoBehaviour
{

    public int tier = 0;
    public int nationNum = 5;
    private Vector3 pos;
    public bool hover;
    public GameObject hex;
    public Hexagon hexagon;
    private Hexagon[] hexagons;
    public GameObject unusedHexMaster;
    public GameObject country;
    private Hexagon oldHexagon;
    private Sprite[] sprites;
    public bool hasUsedMove = false;
    public bool usingFirstTurn = true;
    public GameObject redDot = null;

    void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("People");
        this.nationNum = UIManager.turnNumber + 2;
        this.name = "Unit " + UIManager.selectedCountry + " , " + UIManager.selectedCountry.GetComponent<Country>().unitCount;
        this.transform.SetParent(UIManager.selectedCountry.transform);
        UIManager.selectedCountry.GetComponent<Country>().unitCount++;
    }
    void Update()
    {
        if (hover)
            this.transform.position = Mouse.mousePos;
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (hover)
        {
            if (col.gameObject.tag.Equals("hex"))
            {
                hex = col.gameObject;
                hexagon = hex.GetComponent<Hexagon>();
            }
        }
    }
    public void Place(GameObject gameobject)
    {
        hex = gameobject;
        hexagon = gameobject.GetComponent<Hexagon>();
        this.GetComponent<CircleCollider2D>().radius = .175f;
        Country hexCountry = this.transform.parent.gameObject.GetComponent<Country>();
        pos = hex.transform.position;
        GameObject originalHex = hex;
        this.transform.position = pos;
        pos.z = -1;
        redDot.transform.position = pos;
        if (UIManager.toggled == true) redDot.layer = 0;
        hover = false;
        Mouse.hasPerson = false;
        if (hex.transform.parent != this.gameObject.transform.parent) //hexagon being placed on is not in the same country as the unit
        {
            hexagon.isCountry = true;
            foreach (GameObject g in hexagon.adj) //this loop is for joining countries together
            {
                Hexagon hex = g.GetComponent<Hexagon>();
                if (hex.nationNum == this.nationNum && g.transform.parent != this.gameObject.transform.parent)
                {
                    if (hex.isCountry)
                    {
                        Country originalCountry = g.transform.parent.GetComponent<Country>();
                        Country newCountry = UIManager.selectedCountryScript;
                        if (originalCountry.food > newCountry.food){
                            newCountry.food = originalCountry.food;
                            newCountry.capital.RemoveCapital();
                            newCountry.capital = originalCountry.capital;
                            }
                        else{
                            originalCountry.capital.RemoveCapital();
                        }
                        List<Hexagon> hexesToSwitchParent = g.gameObject.transform.parent.gameObject.GetComponent<Country>().hexagons;
                        List<Person> unitsToSwitchParent = g.gameObject.transform.parent.GetComponent<Country>().people;
                        foreach (Hexagon h in hexesToSwitchParent)
                        {
                            h.gameObject.transform.SetParent(UIManager.selectedCountry.transform);
                        }
                        foreach (Person p in unitsToSwitchParent)
                        {
                            p.country = UIManager.selectedCountry;
                            p.gameObject.transform.SetParent(country.transform);
                        }
                        if (originalCountry.isSelected) originalCountry.isSelected = false;
                    }
                    else { 
                        g.transform.SetParent(UIManager.selectedCountry.transform);
                        g.GetComponent<Hexagon>().isCountry = true;
                    }
                }
            }

            //resets the "hex" variable to the currently clicked hexagon
            hex = originalHex;
            //establishes old country and new country
            GameObject OldHexCountry = hex.transform.parent.gameObject;
            Country OldHexCountryScript = OldHexCountry.GetComponent<Country>();
            hex.transform.SetParent(UIManager.selectedCountry.transform);
            GameObject NewHexCountry = hex.transform.parent.gameObject;
            Country NewHexCountryScript = NewHexCountry.GetComponent<Country>();

            //updates hexagons array of countries
            NewHexCountryScript.UpdateArray();
            if (OldHexCountryScript != null){
                OldHexCountryScript.UpdateArray();
                if (OldHexCountryScript.UpdateArrayAndCountrySplitterConflictFixer == true){
                    OldHexCountryScript.CheckCountrySplit();
                }
            }


            //situation for when you take a capital building
            if (hexagon.hasCapital)
            {
                hexagon.RemoveCapital();
                if (OldHexCountryScript != null)
                {
                    OldHexCountryScript.food = 0;
                    OldHexCountryScript.Invoke("SetCapital", .1f);
                }
            }

            if (hexagon.nationNum != this.nationNum)
            {
                hasUsedMove = true;
            }

            hexagon.nationNum = this.nationNum;
            hexagon.ChangeSprite(this.nationNum);
            UIManager.UpdateStats();
        }

        if (hexagon.hasGuard){
            if (this != hexagon.guard){
                if (hexagon.guard.hasUsedMove) hasUsedMove = true;
                int newTier = hexagon.guard.tier + tier;
                UIManager.allPeople.Remove(hexagon.guard);
                this.transform.parent.GetComponent<Country>().people.Remove(hexagon.guard);
                Destroy(hexagon.guard.gameObject);
                UpdateTier(newTier);
            }
        }
        //update grave status
        if (hexagon.hasGrave)
        {
            Destroy(hexagon.grave);
            hexagon.hasGrave = false;
            UIManager.UpdateStats();
        }

        //update guardedBy feature
        hexagon.hasGuard = true;
        hexagon.guard = this;
        //update the capital's flag
        hexCountry.capital.EditFlag();
        try
        {
            oldHexagon.hasGuard = false;
            oldHexagon.guard = null;
        }
        catch (Exception) {}
        hexCountry.UpdateGuard();
        oldHexagon = hexagon;
    }
    public void StartingTier(){
        tier = UIManager.tier;
        this.GetComponent<SpriteRenderer>().sprite = sprites[tier];
        tier += 1;
    }
    void UpdateTier(int a){
        tier = a;
        GetComponent<SpriteRenderer>().sprite = sprites[tier - 1];
        }
}
