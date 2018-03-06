using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class Instantiator : MonoBehaviour {
	public Hexagon hex;
	public int HexPosX = 0;
	public int HexPosY = 0;
	private int HexColumn = 24;
	private int HexRow =30;
	GameObject hexagon;
	public float x;
	public float y;
	public int totalHexes = 0;
	void Start () {
			x = this.transform.position.x;
			y = this.transform.position.y;
		for (int i = 0; i<=HexColumn; i++) {
			x = 0;
			HexPosX = 0;
			for (int j = 0; j <= HexRow; j++) {
				x += .32f;
				transform.position = new Vector3 (x, y, 0f);
				hexagon = (GameObject) Instantiate (Resources.Load ("Hexagon"), new Vector3 (x,y,0f), Quaternion.identity);
				HexPosX++;
			}
			HexPosY++;
			y += .38f;
		}
		Landmaster ();
	}
	public void Landmaster(){
		int b = 0;
		int step = 200;
		string currenthex;
		print (HexPosX + " , " + HexPosY);
		while (totalHexes < 350) {
			HexPosX = 14;
			HexPosY = 14;
			for (int steps = 0; steps < step; steps++) {
				if (HexPosX == 0) {
					if (HexPosY == 0) {
						b = 0; //leftmost column bottom
					} else if (HexPosY == 24) {
						b = 1; //leftmost column top
					} else {
						b = 2; //leftmost column, not top or bottom
					}
				} else if (HexPosX == 30) {
					if (HexPosY == 24) {
						b = 3; //rightmost column top
					} else if (HexPosY == 0) {
						b = 4; //rightmost column bottom
					} else {
						b = 5; //rightmost column, not top or bottom
					}
				} else if (HexPosY == 0) {
					if (HexPosX % 2 == 0) {
						b = 6; //bottom row, even column
					} else if (HexPosX % 2 == 1) {
						b = 7; //bottom row, odd column
					}
				} else if (HexPosY == 24) {
					if (HexPosX % 2 == 0) {
						b = 8; //top row, even column
					} else if (HexPosX % 2 == 1) {
						b = 9; //top row, odd column
					}
				} else {
					b = 10; //not a border hexagon
				}
				switch (b) {
				case 0:
					b = Random.Range (0, 2);
					break;
				case 1:
					b = Random.Range (1, 4);
					break;
				case 2:
					b = Random.Range (0, 4);
					break;
				case 3:
					b = Random.Range (3, 6);
					break;
				case 4:
					b = Random.Range (0, 2);
					if (b == 1) {
						b = 5;
					}
					break;
				case 5:
					b = Random.Range (3, 7);
					if (b == 6) {
						b = 0;
					}
					break;
				case 6:
					b = Random.Range (0, 3);
					if (b == 2) {
						b = 5;
					}
					break;
				case 7:
					b = Random.Range (0, 5);
					if (b == 3) {
						b = 4;
					}
					if (b == 4) {
						b = 5;
					}
					break;
				case 8:
					b = Random.Range (1, 6);
					break;
				case 9:
					b = Random.Range (2, 5);
					break;
				case 10:
					b = Random.Range (0, 6);
					break;
				}
				print (HexPosX + " , " + HexPosY);
				switch (b) {
				case 0:
					HexPosY++;
					break;
				case 1:
					if (HexPosX % 2 == 1) {
						HexPosY++;
					}
					HexPosX++;
					break;
				case 2:
					if (HexPosX % 2 == 0) {
						HexPosY--;
					}
					HexPosX++;
					break;
				case 3:
					HexPosY--;
					break;
				case 4:
					if (HexPosX % 2 == 0) {
						HexPosY--;
					}
					HexPosX--;
					break;
				case 5:
					if (HexPosX % 2 == 1) {
						HexPosY++;
					}
					HexPosX--;
					break;
				}
				print (b);
				currenthex = "Hexagon " + HexPosX + " , " + HexPosY;
				hexagon = GameObject.Find (currenthex);
				hex = hexagon.GetComponent<Hexagon> ();
				print (hexagon);
				if (hex.isLand == true) {
					continue;
				} else {
					hex.CreateLand ();
				}
			}
		}
	}
} 
