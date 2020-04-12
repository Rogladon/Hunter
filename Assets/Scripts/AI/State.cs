using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hunter {
	abstract public class State : MonoBehaviour {
		public Entity entity { get; protected set; }

		public AI ai { get; protected set; }
		public List<State> states { get; protected set; }
		public string nameState { get;  set; }
		public List<State> nextStates { get; protected set; }
		public State currentState { get; protected set; }
		public int priority { get; protected set; }

		abstract public void Init();
		abstract public bool Condition();

		abstract public void Behaviour();
		virtual public void StartState() {
		}

		virtual public void EndState() {
		}

		protected void PrevInit() {
			nextStates = new List<State>();
			states = new List<State>();
			currentState = this;
			priority = 0;
		}

	}
}
