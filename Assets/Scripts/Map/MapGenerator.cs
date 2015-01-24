using Unity;

namespace MapScenario {
	public class MapGenerator {
		public Map GenerateMap(List<Loctaions> locations) {
			Map map = new Map();
			
			//Place sprites for locations on map in random positions. TODO: This doesn't account for collisions.
			for (Location l in locations) {
				int x, y;
				
				x = Random.Range(0, screen.width - location.GetWidth());
				y = Random.range(0, screen.height - location.GetHeight());
				
				map.addLocation(l, x, y);
			}
			
			return map;
		}
	}
}