using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIManager : MonoBehaviour
{
    private Person person;
    public static float viewportToCanvasAdjustment = 40f;
    public static int turnNumber;
    public static int roundNumber;
    public static GameObject selectedCountry;
    public TurnIndicator turnIndicator;
    public static Country selectedCountryScript;
    public static Ship ship;
    public GameObject unusedHexMaster;
    public static GameObject shipInfo;
    public static GameObject invasionButton;
    public static GameObject canvas;
    public static RectTransform canvasRect;
    public static int tier;
    public static List<Person> allPeople;
    public static List<Ship> allShips;
    private List<Country> countries;
    public static List<GameObject> redDots;
    public static List<GameObject> shipSlots;
    private static GameObject redDot;
    public static Text foodText;
    public static Text incomeText;
    public static Text expensesText;
    public static Text profitText;
    public static ButtonTypeSwitcher bts;
    public static bool toggled = false;
    public static bool standardUnits = true;
    public static bool showingShipInfo = false;
    public static bool hoverMode = true;
    public static int playerCount = 6;
    public static int shipMovementSpeed = 10;
    public static int personMovementSpeed = 3;
	void Start()
    {
        foodText = GameObject.Find("food").GetComponent<Text>();
        incomeText = GameObject.Find("income").GetComponent<Text>();
        expensesText = GameObject.Find("expenses").GetComponent<Text>();
        profitText = GameObject.Find("profit").GetComponent<Text>();
        bts = GameObject.Find("Unit type switch button").GetComponent<ButtonTypeSwitcher>();
        shipInfo = GameObject.Find("Ship Info");
        invasionButton = GameObject.Find("Invasion Button");
        canvas = GameObject.Find("Canvas");
        canvasRect = canvas.GetComponent<RectTransform>();
        shipSlots = new List<GameObject>(10);

        for (int i = 0; i < shipInfo.transform.childCount; i++){
            shipSlots.Add(shipInfo.transform.GetChild(i).gameObject);
        }

        redDots = new List<GameObject>();
        allPeople = new List<Person>();
        allShips = new List<Ship>();
        countries = Instantiator.countryscripts;

        turnNumber = 1;
        roundNumber = 1;

        GameObject.FindWithTag("Audio").GetComponent<AudioSource>().volume = Slider.volume;
        GameObject.FindWithTag("Audio").GetComponent<AudioSource>().Play();
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

    public void SpawnUnit()
    {
        if(!Mouse.hasUnit){
            if (selectedCountry != null && selectedCountry != unusedHexMaster)
            {
                if (showingShipInfo && !(showingShipInfo && ship.isHarbored && ship.harbor.transform.parent == selectedCountryScript.transform))
                {
                    HideShipInfo();
                    UpdateStats();
                }
                if (standardUnits)
                { //if this is spawning one of the human units
                    string button = EventSystem.current.currentSelectedGameObject.name;
                    char[] tierList = button.ToCharArray();
                    tier = (int)char.GetNumericValue(tierList[18]);
                    if (selectedCountryScript.food >= 10 * tier)
                    {
                        GameObject personObject;//needs to be defined here for scope reasons
                        if (showingShipInfo && ship.isHarbored && ship.harbor.transform.parent == selectedCountryScript.transform){//special ship boarding protocol
                            personObject = (GameObject)Instantiate(Resources.Load("Person"), new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f), Quaternion.identity);
                            selectedCountryScript.food -= 10 * tier;
                            tier -= 1;
                            person = personObject.GetComponent<Person>();
                            allPeople.Add(person);
                            person.StartingTier();
                            person.BoardShip(ship.hexagon.gameObject);
                        }
                        else{//typical unit spawn
                            personObject = (GameObject)Instantiate(Resources.Load("Person"), new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f), Quaternion.identity);
                            selectedCountryScript.food -= 10 * tier;
                            tier -= 1;
                            person = personObject.GetComponent<Person>();
                            allPeople.Add(person);
                            selectedCountryScript.UpdateArray();
                            person.transform.SetParent(selectedCountry.transform);
                            person.StartingTier();
                            Mouse.person = person;
                            FindObjectOfType<Mouse>().PickUpUnit("person");
                        }
                        UpdateStats();
                    }
                    else
                    {
                        print("you do not have enough food to create that unit!");
                    }
                }
                else
                {//if this is spawning a castle, harbor, or ship
                    string button = EventSystem.current.currentSelectedGameObject.name;
                    switch (button)
                    {
                        case "castle button": SpawnCastle(); break;
                        case "harbor button": SpawnHarbor(); break;
                        case "ship button": SpawnShip(); break;
                        case "village button": SpawnNewCapital(); break;
                    }
                }
            }
            else print("you must select a country before you can create a unit!");
        }
        else{
            print("You must place the unit you have purchased before purchasing another one!");
        }
    }

    public void NextTurn()
    {
        if (Mouse.hasUnit == false)
        {
            turnNumber++;
            if (turnNumber == playerCount + 1)
            {
                turnNumber = 1;
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
                foreach (Ship s in allShips){
                    s.hasUsedMove = false;
                    s.NewRound();
                }
            } 
            turnIndicator.ChangeSprite(turnNumber);
            SpawnRedDots();
            if (selectedCountryScript != null){
                selectedCountry = null;
                selectedCountryScript.isSelected = false;
                if (selectedCountryScript.isInvasionCountry){
                    selectedCountryScript.capital.shipCapital.GetComponent<SpriteRenderer>().enabled = true;
                }
                else selectedCountryScript.capital.village.GetComponent<SpriteRenderer>().enabled = true;
                selectedCountryScript = null;
                UpdateStats();
            }
            if (showingShipInfo) { HideShipInfo(); }
            if (!standardUnits) { bts.ChangeButtons(); }
        }
        else print("You must place the currently selected unit before you can end the turn!");
    }

    public static void SetCurrentCountry(GameObject g)
    {
        if (!Mouse.hasUnit)
        {
            if (selectedCountryScript != null) {
                selectedCountryScript.isSelected = false;
                if (selectedCountryScript.isInvasionCountry){
                    selectedCountryScript.capital.shipCapital.GetComponent<SpriteRenderer>().enabled = true;  
                }
                else{
                    selectedCountryScript.capital.village.GetComponent<SpriteRenderer>().enabled = true;   
                }
            }
            Mouse.ship = null;
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
                if (p.nationNum - 2 == turnNumber && !p.isSailor)
                {
                    try { redDot = (GameObject)Instantiate(Resources.Load("RedDott"), new Vector3(p.transform.position.x, p.transform.position.y, -1f), Quaternion.identity); }
                    catch (Exception) { continue; } //this try/catch is a temporary solution for missing spots in allPeople caused by starvation
                    if (toggled) redDot.layer = 0;
                    redDots.Add(redDot);
                    p.redDot = redDot;
                    redDot.GetComponent<RedDot>().person = p;
                }
            }
            foreach (Ship s in allShips)
            {
                if (s.nationNum - 2 == turnNumber)
                {
                try { redDot = (GameObject)Instantiate(Resources.Load("RedDott"), new Vector3(s.transform.position.x, s.transform.position.y, -1f), Quaternion.identity); }
                catch (Exception) { continue; }
                if (toggled) redDot.layer = 0;
                    redDots.Add(redDot);
                    s.redDot = redDot;
                    redDot.GetComponent<RedDot>().ship = s;
                redDot.GetComponent<RedDot>().notShip = false;
                }
            }
    }

    public static void UpdateStats()
    {
        if (selectedCountryScript != null)
        {
            //update all of the text

            int expenses = 0;
            foodText.text = "food: " + selectedCountryScript.food;
            int income = selectedCountryScript.GiveIncome();
            incomeText.text = "income: " + income;
            foreach (Person p in selectedCountryScript.people)
            {
                if (p.isSailor){
                    expenses += Ship.sailorFoodConsumption;
                }
                else{
                    expenses += 2 * (int)Math.Pow(3, (p.tier - 1));
                }
            }
            expensesText.text = "expenses: " + expenses;
            profitText.text = "profit per turn: " + (income - expenses);

            //update button shades
            DarkenButtons();

        }
        else
        {
            foodText.text = null;
            incomeText.text = null;
            expensesText.text = null;
            profitText.text = null;
        }
    }

    void SpawnShip()
    {
        if(!Mouse.hasUnit){
            if (selectedCountryScript.food >= 20)
            {
                if (selectedCountryScript.hasHarbor)
                {
                    selectedCountryScript.food -= 20;
                    GameObject shipObject = (GameObject)Instantiate(Resources.Load("Ship"), new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f), Quaternion.identity);
                    ship = shipObject.GetComponent<Ship>();
                    ship.transform.SetParent(selectedCountry.transform);
                    selectedCountryScript.UpdateArray();
                    allShips.Add(ship);
                    Mouse.ship = ship;
                    FindObjectOfType<Mouse>().PickUpUnit("ship");
                    UpdateStats();
                }
                else print("What are you going to do with a ship when you have no harbor?");
            }
            else
            {
                print("you do not have enough food to create that unit!");
            }
        }
        else{
            print("You must place the unit you have purchased before purchasing another one!");
        }
    }

    void SpawnCastle(){
        if (!Mouse.hasUnit)
        {
            if (selectedCountryScript.food >= 20)
            {
                selectedCountryScript.food -= 20;
                GameObject castleObject = (GameObject)Instantiate(Resources.Load("Castle"), new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f), Quaternion.identity);
                Castle castle = castleObject.GetComponent<Castle>();
                castle.transform.SetParent(selectedCountry.transform);
                Mouse.castle = castle;
                FindObjectOfType<Mouse>().PickUpUnit("castle");
                UpdateStats();
            }
            else
            {
                print("you do not have enough food to create that unit!");
            }
        }
        else
        {
            print("You must place the unit you have purchased before purchasing another one!");
        }
    }

    void SpawnHarbor(){
        if (!Mouse.hasUnit)
        {
            if (selectedCountryScript.food >= 20)
            {
                selectedCountryScript.food -= 20;
                selectedCountryScript.hasHarbor = true;
                GameObject harborObject = (GameObject)Instantiate(Resources.Load("Harbor"), new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f), Quaternion.identity);
                Harbor harbor = harborObject.GetComponent<Harbor>();
                harbor.transform.SetParent(selectedCountry.transform);
                Mouse.harbor = harbor;
                FindObjectOfType<Mouse>().PickUpUnit("harbor");
                UpdateStats();
            }
            else
            {
                print("you do not have enough food to create that unit!");
            }
        }
        else
        {
            print("You must place the unit you have purchased before purchasing another one!");
        }
    }

    void SpawnNewCapital()
    {
        if (!Mouse.hasUnit)
        {
            if (selectedCountryScript.food >= 20)
            {
                selectedCountryScript.food -= 20;
                GameObject villageObject = (GameObject)Instantiate(Resources.Load("Capital"), new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f), Quaternion.identity);
                villageObject.GetComponent<Village>().hover = true;
                Mouse.village = villageObject;
                FindObjectOfType<Mouse>().PickUpUnit("village");
                UpdateStats();
            }
            else
            {
                print("you do not have enough food to create that unit!");
            }
        }
        else
        {
            print("You must place the unit you have purchased before purchasing another one!");
        }
    }

    public static void ShowBoatInfo(Ship s){
        ship = s;
        shipInfo.GetComponent<Image>().enabled = true;
        showingShipInfo = true;
        for (int i = 0; i < 10; i++){
            shipSlots[i].GetComponent<Image>().enabled = true;
            try { shipSlots[i].GetComponent<Image>().sprite = s.sailors[i].GetComponent<SpriteRenderer>().sprite; }
            catch (Exception) { shipSlots[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprites/Mystery Man"); }
        }
        if (!ship.moored){
            int shipIncome = 0;
            int shipExpenses = s.sailors.Count;
            foreach (Person p in s.sailors)
            {
                if (p.tier == 1)
                {
                    shipIncome += 4;
                }
            }
            foodText.text = "ship food: " + s.food;
            incomeText.text = "ship income: " + shipIncome;
            expensesText.text = "ship expenses: " + shipExpenses;
            profitText.text = "ship profit: " + (shipIncome - shipExpenses);
        }
        else{
            if (ship.transform.parent.GetComponent<Country>().invasionFishTurns > 0){
                int shipIncome = 0;
                int fishingTurns = ship.transform.parent.GetComponent<Country>().invasionFishTurns;
                foreach (Person p in s.sailors)
                {
                    if (p.tier == 1)
                    {
                        shipIncome += 4;
                    }
                }
                foodText.text = "fishing income: " + shipIncome;
                incomeText.text = null;
                expensesText.text = "Turns of \nfishing remaining: " + fishingTurns;
                profitText.text = null;
            }
            else{
                foodText.text = "This hexagon has been overfished!";
                expensesText.text = "You can no longer gain revenue from the ship here.";
                incomeText.text = null;
                profitText.text = null;
            }
        }

        if (s.invasionPossibility && !s.moored && s.sailors.Count != 0 && !s.isHarbored){
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(s.transform.position);
            Vector2 screenPosition = new Vector2(((viewportPosition.x * canvasRect.sizeDelta.x) - (.5f * canvasRect.sizeDelta.x)), ((viewportPosition.y * canvasRect.sizeDelta.y) - (.5f * canvasRect.sizeDelta.y) + viewportToCanvasAdjustment));
            invasionButton.GetComponent<RectTransform>().anchoredPosition = screenPosition;
            invasionButton.GetComponent<Image>().enabled = true;
            invasionButton.GetComponent<Button>().enabled = true;
            invasionButton.GetComponentInChildren<Text>().enabled = true;
        }
        }

    public static void HideShipInfo(){
        showingShipInfo = false;
        shipInfo.GetComponent<Image>().enabled = false;
        for (int i = 0; i < 10; i++)
        {
            shipSlots[i].GetComponent<Image>().enabled = false;
        }
        foodText.text = null;
        incomeText.text = null;
        expensesText.text = null;
        profitText.text = null;
        invasionButton.GetComponent<Image>().enabled = false;
        invasionButton.GetComponent<Button>().enabled = false;
        invasionButton.GetComponentInChildren<Text>().enabled = false;
    }
    public void Disembark(){
        if (ship.isHarbored && showingShipInfo)
        {
            string button = EventSystem.current.currentSelectedGameObject.name;
            char[] sailorList = button.ToCharArray();
            int sailorNumber = (int)char.GetNumericValue(sailorList[7]);
            Person disembarkingSailor = ship.sailors[sailorNumber - 1];
            Country disembarkingCountry = ship.harbor.transform.parent.GetComponent<Country>();
            disembarkingSailor.GetComponent<SpriteRenderer>().enabled = true;
            disembarkingSailor.GetComponent<CircleCollider2D>().enabled = true;
            disembarkingSailor.isSailor = false;
            disembarkingSailor.transform.SetParent(disembarkingCountry.transform);
            disembarkingCountry.UpdateArray();

            Mouse.person = disembarkingSailor;
            FindObjectOfType<Mouse>().PickUpUnit("person");
            disembarkingSailor.hasUsedMove = true;

            ship.sailors.Remove(disembarkingSailor);
            ShowBoatInfo(ship);
        }
        else if (ship.moored){
            string button = EventSystem.current.currentSelectedGameObject.name;
            char[] sailorList = button.ToCharArray();
            int sailorNumber = (int)char.GetNumericValue(sailorList[7]);
            Person disembarkingSailor = ship.sailors[sailorNumber - 1];
            Country disembarkingCountry = ship.invasionCountry.GetComponent<Country>();
            disembarkingSailor.GetComponent<SpriteRenderer>().enabled = true;
            disembarkingSailor.GetComponent<CircleCollider2D>().enabled = true;
            disembarkingSailor.isSailor = false;
            disembarkingCountry.UpdateArray();

            Mouse.person = disembarkingSailor;
            FindObjectOfType<Mouse>().PickUpUnit("person");
            disembarkingSailor.hasUsedMove = true;

            ship.sailors.Remove(disembarkingSailor);
            ShowBoatInfo(ship);
        }
        else print("You can't disembark in the middle of the ocean!");
    }
    public static void HideInvasionButton(){
        invasionButton.GetComponent<Image>().enabled = false;
        invasionButton.GetComponent<Button>().enabled = false;
        invasionButton.GetComponentInChildren<Text>().enabled = false;
    }
    public static void AdjustInvasionButton(int i){
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(ship.transform.position);
            invasionButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(((viewportPosition.x * canvasRect.sizeDelta.x) - (.5f * canvasRect.sizeDelta.x)), ((viewportPosition.y * canvasRect.sizeDelta.y) - (.5f * canvasRect.sizeDelta.y) + (viewportToCanvasAdjustment - i)));   
    }
    public void CreateInvasionCountry(){
        print("InvasionCountryMade");
        HideInvasionButton();
        GameObject invasionCountry = (GameObject)Instantiate(Resources.Load("Country"), new Vector3(0, 0, 0), Quaternion.identity);
        Country invasionCountryScript = invasionCountry.GetComponent<Country>();
        invasionCountryScript.isInvasionCountry = true;
        invasionCountryScript.food = ship.food;
        SetCurrentCountry(invasionCountry);
        Instantiator.countryscripts.Add(invasionCountryScript);
        invasionCountryScript.capital = ship.hexagon;
        invasionCountryScript.capital.shipCapital = ship.gameObject;
        ship.food = 0;
        ship.invasionCountry = invasionCountry;
        ship.moored = true;
        ship.transform.SetParent(invasionCountry.transform);
        foreach(Person p in ship.sailors){
            p.transform.SetParent(invasionCountry.transform);
            p.hexCountry = invasionCountryScript;
            p.hexagon = ship.hexagon;
        }
        invasionCountryScript.UpdateArray();
        UpdateStats();
    }
    public static void DarkenButtons(){
        if (selectedCountryScript != null){
            for (int i = 0; i < 4; i++)
            {
                if (standardUnits)
                {
                    if (selectedCountryScript.food < (i + 1) * 10)
                    {
                        bts.buttonArray[i].GetComponent<Button>().interactable = false;
                        bts.buttonArray[i].transform.GetChild(0).GetComponent<Image>().color = new Color(0.5176471f, 0.4980392f, 0.4980392f, 0.5019608f);
                    }
                    else
                    {
                        bts.buttonArray[i].GetComponent<Button>().interactable = true;
                        bts.buttonArray[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                }
                else
                {
                    if (selectedCountryScript.food < 20)
                    {
                        bts.buttonArray[i].GetComponent<Button>().interactable = false;
                        bts.buttonArray[i].transform.GetChild(0).GetComponent<Image>().color = new Color(0.5176471f, 0.4980392f, 0.4980392f, 0.5019608f);
                    }
                    else
                    {
                        bts.buttonArray[i].GetComponent<Button>().interactable = true;
                        bts.buttonArray[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    }
                }
            } 
        }
    }
}
