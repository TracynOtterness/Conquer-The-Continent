using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIManager : MonoBehaviour
{
    private Person person;
    public static int turnNumber;
    public static int roundNumber;
    public static GameObject selectedCountry;
    public TurnIndicator turnIndicator;
    public static Country selectedCountryScript;
    public GameObject unusedHexMaster;
    public static int tier;
    public static List<Person> allPeople;
    private List<Country> countries;
    public static List<GameObject> redDots;
    private static GameObject redDot;
    public static bool toggled = false;
    public static Text foodText;
    public static Text incomeText;
    public static Text expensesText;
    public static Text profitText;


    void Start()
    {
        foodText = GameObject.Find("food").GetComponent<Text>();
        incomeText = GameObject.Find("income").GetComponent<Text>();
        expensesText = GameObject.Find("expenses").GetComponent<Text>();
        profitText = GameObject.Find("profit").GetComponent<Text>();
        redDots = new List<GameObject>();
        allPeople = new List<Person>();
        countries = Instantiator.countryscripts;
        turnNumber = 1;
        roundNumber = 1;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (GameObject r in redDots)
            {
                if (r.layer == 9) {
                    r.layer = 0;
                    toggled = true;
                }
                else {
                    r.layer = 9;
                    toggled = false;
                }
            }
        }
    }


    public void SpawnPerson()
    {
        if (selectedCountry != null && selectedCountry != unusedHexMaster)
        {
            GameObject personObject = (GameObject)Instantiate(Resources.Load("Person"), new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f), Quaternion.identity);
            string button = EventSystem.current.currentSelectedGameObject.name;
            char[] tierList = button.ToCharArray();
            tier = (int)char.GetNumericValue(tierList[18]);
            if (selectedCountryScript.food >= 10 * tier)
            {
                selectedCountryScript.food -= 10 * tier;
                tier -= 1;
                person = personObject.GetComponent<Person>();
                allPeople.Add(person);
                person.country = selectedCountry;
                person.country.GetComponent<Country>().UpdateArray();
                personObject.transform.SetParent(selectedCountry.transform);
                person.StartingTier();
                Mouse.personObject = personObject;
                Mouse.person = personObject.GetComponent<Person>();
                FindObjectOfType<Mouse>().PickUpUnit();
                UpdateStats();
            }
            else {
                Destroy(personObject);
                print("you do not have enough food to create a new unit!"); 
            }
        }
        else print("you must select a country before you can create a unit!");
    }
    public void NextTurn()
    {
        if (Mouse.hasPerson == false)
        {
            turnIndicator.ChangeSprite(turnNumber + 3);
            turnNumber++;
            if (turnNumber == 6)
            {
                turnNumber = 1;
                turnIndicator.ChangeSprite(turnNumber + 2);
                roundNumber++;
                foreach (Country c in Instantiator.countryscripts)
                {
                    c.GiveRoundFood();
                    c.capital.EditFlag();
                }
                foreach (Person p in allPeople)
                {
                    p.hasUsedMove = false;
                }
            }
            SpawnRedDots();
            if (selectedCountryScript != null){
                selectedCountry = null;
                selectedCountryScript.isSelected = false;
                selectedCountryScript.capital.village.GetComponent<SpriteRenderer>().enabled = true;
                selectedCountryScript = null;
                UpdateStats();
            }
        }
        else print("You must place the currently selected unit before you can end the turn!");
    }
    public static void SetCurrentCountry(GameObject g)
    {
        if (!Mouse.hasPerson)
        {
            if (selectedCountryScript != null) {
                selectedCountryScript.isSelected = false;
                selectedCountryScript.capital.village.GetComponent<SpriteRenderer>().enabled = true;
            }
            print("SetCurrentCounty");
            selectedCountry = g;
            selectedCountryScript = g.GetComponent<Country>();
            selectedCountryScript.isSelected = true;
            UpdateStats();
        }
        else print("You can't switch countries while you have a unit equipped!");
    }
    public static void SpawnRedDots()
    {
        foreach (GameObject r in redDots)
        {
            Destroy(r);
        }
        redDots.Clear();
        foreach (Person p in allPeople)
        {
            if (p.nationNum - 2 == turnNumber)
            {
                try { redDot = (GameObject)Instantiate(Resources.Load("RedDott"), new Vector3(p.transform.position.x, p.transform.position.y, -1f), Quaternion.identity); }
                catch (Exception) { continue; } //this try/catch is a temporary solution for missing spots in allPeople caused by starvation
                if (toggled) redDot.layer = 0;
                redDots.Add(redDot);
                p.redDot = redDot;
                redDot.GetComponent<RedDot>().person = p;
            }
        }
    }
    public static void UpdateStats()
    {
        if (selectedCountryScript != null)
        {
            int expenses = 0;
            foodText.text = "food: " + selectedCountryScript.food;
            int income = selectedCountryScript.GiveIncome();
            incomeText.text = "income: " + income;
            foreach (Person p in selectedCountryScript.people)
            {
                expenses += 2 * (int)Math.Pow(3, (p.tier - 1));
            }
            expensesText.text = "expenses: " + expenses;
            profitText.text = "profit per turn: " + (income - expenses);
        }
        else
        {
            foodText.text = null;
            incomeText.text = null;
            expensesText.text = null;
            profitText.text = null;
        }
    }
}