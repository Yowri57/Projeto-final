using UnityEngine;
using System.Collections;
using System.IO;

namespace com.javierquevedo.events{
	
public class GameEvents : MonoBehaviour {
	
		public delegate void BubblesRemovedHandler (int bubbleCount, bool exploded);
		public static event BubblesRemovedHandler OnBubblesRemoved;
		
		public static void BubblesRemoved(int bubbleCount, bool exploded){
			if (OnBubblesRemoved != null){
				OnBubblesRemoved(bubbleCount, exploded);
			}
		}
		
		public delegate void GameFinishedHandler (GameState state);
		public static event GameFinishedHandler OnGameFinished;
		
		public static void GameFinished(GameState state){
			if (OnGameFinished != null){
				OnGameFinished(state);
							//try
			//{
				//Pass the filepath and filename to the StreamWriter Constructor
				StreamWriter sw = new StreamWriter("Test.txt", true);
				//Write a line of text
				sw.WriteLine(state.ToString());
				//Write a second line of text
				sw.Close();
				HUD.gravartempo();
			//}
			//catch (Exception e)
			//{
			//	Console.WriteLine("Exception: " + e.Message);
			//}
			}
		}
		
	}
	
}
