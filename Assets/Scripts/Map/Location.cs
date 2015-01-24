using UnityEngine;

namespace MapScenario {
	
	[System.Serializable]
	public class Location {
		public string name;
		public SpriteRenderer sprite;
		//TODO: Maybe add enemies specific to each location?
		
		public Location() {
			this.name = "NAMELESS";
			
		}
		
		public Location(string name, SpriteRenderer sprite) {
			this.name = name;
			this.sprite = sprite;
		}
		
		public int GetHeight() { return 0; }// (int) this.sprite.rect.height; }
		public int GetWidth() { return 0; }// (int) this.sprite.rect.width; }
	}
}