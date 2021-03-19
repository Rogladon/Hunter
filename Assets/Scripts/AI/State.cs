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
		//Инициализация состояния
		abstract public void Init();
		//Условие перехода на это состояние
		abstract public bool Condition();
		//Основное действие (выполняется как Update()
		abstract public void Behaviour();
		//Выполняется при переходе на это состояние один раз
		virtual public void StartState() {
		}
		//При окончании состония (когда новое начинается)
		virtual public void EndState() {
		}
		//А это перед инициализацией стартовой, тут пишется собственно в какие состояния можно перейти из этого и приоритет состояния
		protected void PrevInit() {
			nextStates = new List<State>();
			states = new List<State>();
			currentState = this;
			priority = 0;
		}

	}
}
