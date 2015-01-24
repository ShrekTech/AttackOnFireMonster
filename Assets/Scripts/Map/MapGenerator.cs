using UnityEngine;
using System.Collections.Generic;
using MapScenario;

public class MapGenerator : MonoBehaviour {
	
	public List<Location> locationPool = new List<Location>();
	
	public Map GenerateMap() {
		return this.GenerateMap( new List<Location>() {
			this.RandomLocation(),
			this.RandomLocation(),
			this.RandomLocation()
		});
	}
	
	public Map GenerateMap(List<Location> locations) {
		Map map = new Map();
		
		//Place sprites for locations on map in random positions. TODO: This doesn't account for collisions.
		// for (Location location in locations) {
		// 	int x, y;
		//
		// 	x = Random.Range(0, screen.width - location.GetWidth());
		// 	y = Random.range(0, screen.height - location.GetHeight());
		//
		// 	map.AddLocation(location, x, y);
		// }
		
		return map;
	}
	
	public Location RandomLocation() {		
		return locationPool[Random.Range(0, locationPool.Count)];
	}
}
