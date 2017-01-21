using System.Collections;
using System.Collections.Generic;

public class GameVariable {
	public enum controlerType {Joy1, Joy2, KeyLeft, KeyRight};
	public int numberOfPlayers; //should be set before Start() is run
	public controlerType[] playerControlType;
	public int localColliderRadius = 1;

	public GameVariable(){

	}

}
