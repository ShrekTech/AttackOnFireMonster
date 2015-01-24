using Unity;

namespace MapScenario {
	class Location {
		private string name;
		private Sprite sprite;
		//TODO: Maybe add enemies specific to each location?
		
		public Location(string name, Sprite sprite) {
			this.name = name;
			this.sprite = sprite;
		}
		
		public int GetHeight() { return this.sprtie.rect.height; }
		public int GetWidth() { return this.sprite.rect.width; }
	}
}