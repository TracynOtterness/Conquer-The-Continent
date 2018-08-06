using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMovement : MonoBehaviour {

	private int nationNum;
	public GameObject personObject;
	//private bool valid = true;
	private Person person;
    bool placedIsValid;
    private Hexagon hexagon;

    void Start(){
        hexagon = this.gameObject.GetComponent<Hexagon>();
    }
		void OnTriggerStay2D(Collider2D col) {
        if (col.gameObject.tag.Equals("mouse") && Input.GetMouseButtonUp(0) == true && Mouse.hasPerson == true)
        {
            personObject = Mouse.personObject;
            person = personObject.GetComponent<Person>();
            if (person.hover)
            {
                placedIsValid = PlacedIsValid();
                if (placedIsValid)
                {
                    Mouse.hasPerson = false;
                    hexagon.guardedBy = person.tier;
                    person.Place(this.gameObject);
                }
            }
		}
    }
    bool PlacedIsValid (){ //the criteria for placing the unit down to be something the player can do
        Hexagon thisHex = this.gameObject.GetComponent<Hexagon>();
        // criteria for it to be a valid place to move the unit goes here
        if (thisHex.isLand == true ) {//requirements regardless of country/color of hexagon
            if (this.gameObject.transform.parent != person.transform.parent) //requirements if it is taking a hexagon of another country
            {
                if (UIManager.roundNumber != 1){
                    if (thisHex.guardedBy < person.tier || thisHex.guardedBy == 0)
                    {
                        if (thisHex.hasGuard)
                        {
                            if (thisHex.transform.parent != person.transform.parent)
                            {
                                Country country = thisHex.transform.parent.GetComponent<Country>();
                                UIManager.allPeople.Remove(thisHex.guard);
                                country.people.Remove(thisHex.guard);
                                Destroy(thisHex.guard.gameObject);
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
                else {
                    print("You cannot capture hexagons on the first turn!");
                    return false;
                }
            }
            if (thisHex.hasCapital) {
                print("You can't place a unit on the country's capital!");
                return false;
            }
            if(thisHex.hasGuard){
                if (person.tier + thisHex.guard.tier > 4){
                    print("You cannot combine those units!");
                    return false;
                }
            }
            return true;
        }
        print("You can't place units in water, they'd drown!");
        return false;
    }
}

