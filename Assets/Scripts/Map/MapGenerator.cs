using Unity;

namespace MapScenario {
	public class MapGenerator {
		
		public Map GenerateMap() {
			this.GenerateMap({
				this.RandomLocation(),
				this.RandomLocation(),
				this.RandomLocation()
			});
		}
		
		public Map GenerateMap(List<Loctaions> locations) {
			Map map = new Map();
			
			//Place sprites for locations on map in random positions. TODO: This doesn't account for collisions.
			for (Location location in locations) {
				int x, y;
				
				x = Random.Range(0, screen.width - location.GetWidth());
				y = Random.range(0, screen.height - location.GetHeight());
				
				map.AddLocation(location, x, y);
			}
			
			return map;
		}
		
		public Location RandomLocation() {
			return new Location(); //TODO
		}
	}
}