using System;
using System.Collections.Generic;

namespace DocumentManager.Models {
	public class IdValuePair {
		public int Id, Value;

		public IdValuePair() {
		}

		public IdValuePair(int id, int value) {
			Id = id;
			Value = value;
		}

		public override string ToString() {
			return $"[{Id}] {Value}";
		}
	}
}
