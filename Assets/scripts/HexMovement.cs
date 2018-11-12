using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMovement : MonoBehaviour {

	private int nationNum;
	//private bool valid = true;
	private Person person;
    private Ship ship;
    private Castle castle;
    private Harbor harbor;
    private GameObject village;
    bool placedIsValid;
    private Hexagon hexagon;

    void Start(){
        hexagon = this.gameObject.GetComponent<Hexagon>();
    }
		void OnTriggerStay2D(Collider2D col) {
        if (col.gameObject.tag.Equals("mouse") && Input.GetMouseButtonUp(0) && Mouse.hasUnit && !Mouse.pickUpBuffer)
        {
            if (Mouse.person != null){
                person = Mouse.person;
                if (person.hover)
                {
                    placedIsValid = PlacedIsValid("person");
                    if (placedIsValid)
                    {
                        Mouse.hasUnit = false;
                        Mouse.person = null;
                        person.MovePerson(this.gameObject);
                    }
                }  
            }
            else if (Mouse.ship != null){
                ship = Mouse.ship;
                if (ship.hover){
                    placedIsValid = PlacedIsValid("ship");
                    if (placedIsValid){
                        Mouse.hasUnit = false;
                        Mouse.ship = null;
                        ship.hover = false;
                        ship.hasUsedMove = true;
                        hexagon.guardedBy = 1;
                        ship.MoveShip(this.gameObject);
                    }
                }
            }
            else if (Mouse.castle != null){
                castle = Mouse.castle;
                if (castle.hover){
                    placedIsValid = PlacedIsValid("castle");
                    if(placedIsValid){
                        Mouse.hasUnit = false;
                        Mouse.castle = null;
                        castle.hover = false;
                        hexagon.guardedBy = 2;
                        hexagon.hasCastle = true;
                        hexagon.castle = castle.gameObject;
                        castle.Place(this.gameObject);

                    }
                }
            }
            else if (Mouse.harbor != null){
                harbor = Mouse.harbor;
                if (harbor.hover)
                {
                    placedIsValid = PlacedIsValid("harbor");
                    if (placedIsValid)
                    {
                        Mouse.hasUnit = false;
                        Mouse.harbor = null;
                        harbor.hover = false;
                        hexagon.hasHarbor = true;
                        hexagon.harbor = harbor.gameObject;
                        harbor.Place(this.gameObject);

                    }
                }
            }
            else if (Mouse.village != null){
                village = Mouse.village;
                placedIsValid = PlacedIsValid("village");
                if(placedIsValid){
                    Mouse.hasUnit = false;
                    Destroy(Mouse.village);
                    Mouse.village = null;
                    village.GetComponent<Village>().hover = false;
                    Mouse.ResetViableHexagons();
                    this.transform.parent.GetComponent<Country>().SetCapital(hexagon);
                }
            }
		}
    }

    bool PlacedIsValid (string s){ //the criteria for placing the unit down to be something the player can do
        if (s == "person"){ //requirements for normal units
            if (!person.usingFirstTurn){
                if (hexagon.isLand == true)
                {//requirements regardless of country/color of hexagon if not being placed on ship
                    if (this.gameObject.transform.parent != person.transform.parent) //requirements if it is taking a hexagon of another country
                    {
                        if (UIManager.roundNumber != 1)
                        {//prevents capturing hexagons on the first turn
                            bool isAdjacentToParentCountry = false;
                            foreach (GameObject g in hexagon.adj)
                            {
                                Hexagon h = g.GetComponent<Hexagon>();
                                if (g.transform.parent == person.transform.parent)
                                {
                                    isAdjacentToParentCountry = true;
                                }
                                if (h.hasShip)
                                {
                                    if (h.ship.nationNum == person.nationNum && h.ship.moored)
                                    {
                                        isAdjacentToParentCountry = true;
                                    }
                                }
                            }
                            if (isAdjacentToParentCountry)
                            {
                                if (hexagon.guardedBy < person.tier || hexagon.guardedBy == 0)
                                {
                                    if (hexagon.hasGuard)
                                    {
                                        if (hexagon.transform.parent != person.transform.parent)
                                        {
                                            Country country = hexagon.transform.parent.GetComponent<Country>();
                                            UIManager.allPeople.Remove(hexagon.guard);
                                            Destroy(hexagon.guard.gameObject);
                                            hexagon.hasGuard = false;
                                            country.UpdateGuard();
                                            country.UpdateArray();
                                        }
                                    }
                                    return true;
                                }
                                else
                                {
                                    print("the unit you tried to occupy is too heavily guarded for that unit to take!");
                                    return false;
                                }
                            }
                            else
                            {
                                print("You cannot capture hexagons that aren't adjacent to your country!");
                                return false;
                            }
                        }
                        else
                        {
                            print("You cannot capture hexagons on the first turn!");
                            return false;
                        }
                    }
                    if (hexagon.hasCapital)
                    {
                        print("You can't place a unit on the country's capital!");
                        return false;
                    }
                    if (hexagon.hasHarbor)
                    {
                        print("You cant place a unit on a friendly harbor!");
                        return false;
                    }
                    if (hexagon.hasCastle)
                    {
                        print("You can't place a unit on a castle!");
                        return false;
                    }
                    if (hexagon.hasGuard)
                    {
                        if (person.tier + hexagon.guard.tier > 4)
                        {
                            print("You cannot combine those units!");
                            return false;
                        }
                    }
                    return true;
                }

                if (hexagon.hasShip && hexagon.ship.isHarbored && hexagon.ship.harbor.transform.parent == person.transform.parent && hexagon.ship.sailors.Count < 10)
                {
                    Mouse.hasUnit = false;
                    Mouse.person = null;
                    foreach (Hexagon h in Mouse.viableHexagons) { h.ToggleMask(); }
                    Mouse.viableHexagons.Clear();
                    person.BoardShip(this.gameObject);
                    print("Anchors aweigh sailor!");
                    return false;
                }
                print("You can't place units in water, they'd drown!");
                return false;  
            }

            else{
                if (Mouse.viableHexagons.Contains(hexagon))
                {
                    if (hexagon.hasShip){
                        if(hexagon.ship.sailors.Count < 10){
                            Mouse.hasUnit = false;
                            Mouse.person = null;
                            foreach (Hexagon h in Mouse.viableHexagons) { h.ToggleMask(); }
                            Mouse.viableHexagons.Clear();
                            person.BoardShip(this.gameObject);
                            print("Anchors aweigh sailor!");
                            return false;
                        }
                        else{
                            print("Sorry, ship's full mate!");
                            return false;
                        }
                    }
                    return true;
                }
                else { 
                    print("You can only place units adjacent to buildings of your country or on a ship on the first turn!");
                    return false;
                }
            }
        }
        if (s == "ship")
        {
            if (hexagon.isLand == false)
            {
                if (ship.isFirstTurn)
                {
                    foreach (GameObject h in hexagon.adj)
                    {
                        Hexagon hex = h.GetComponent<Hexagon>();
                        if (hex.hasHarbor && hex.transform.parent == ship.transform.parent) {
                            ship.isHarbored = true;
                            ship.harbor = hex.harbor;
                            return true; 
                        } 
                    }
                    print("You must build a ship next to one of the country's harbors!");
                    return false;
                }
                if (Mouse.viableHexagons.Contains(hexagon)){
                    return true;
                }
                else{
                    print("You cannot move to that hexagon right now!");
                    return false;
                }
            }
            else
            {
                print("Boats can't sail on land!");
                return false;
            }
        }
        if (s == "castle"){
            if (hexagon.transform.parent == castle.transform.parent)
            {
                if (!hexagon.hasCapital && !hexagon.hasGrave && !hexagon.hasGuard && !hexagon.hasCastle && !hexagon.hasHarbor)
                {
                    return true;
                }
                else {
                    print("you can't place a castle where there is already something else!");
                    return false;
                }
            }
            else {
                print("you have to place the castle in your own country!");
                return false;
            }

        }
        if (s == "harbor"){
            if (hexagon.transform.parent == harbor.transform.parent)
            {
                if (!hexagon.hasCapital && !hexagon.hasGrave && !hexagon.hasGuard && !hexagon.hasCastle && !hexagon.hasHarbor)
                {
                    foreach(GameObject h in hexagon.adj){
                        Hexagon hex = h.GetComponent<Hexagon>();
                        if (!hex.isLand) return true;
                    }
                    print("A harbor does no good if it isn't next to water!");
                    return false;
                }
                else
                {
                    print("you can't place a harbor where there is already something else!");
                    return false;
                }
            }
            else
            {
                print("you have to place the harbor in your own country!");
                return false;
            }
        }
        if(s == "village"){
            if(hexagon.transform.parent.gameObject == UIManager.selectedCountry && !hexagon.hasCapital && !hexagon.hasGrave && !hexagon.hasGuard && !hexagon.hasCastle && !hexagon.hasHarbor){
                return true;
            }
            else{
                print("you must create the new capital in a free space");
            }
        }
        return false;
    }
}
