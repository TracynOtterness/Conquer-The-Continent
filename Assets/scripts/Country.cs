using System.Collections;
using System.Collections.Generic;
using UnityEngine;

private Instantiator instantiator;

public class Country : MonoBehaviour {

    void Awake(){
        this.name = "Country " + instantiator.countryCount;
    }
}
