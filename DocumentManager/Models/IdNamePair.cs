using System;
using System.Collections.Generic;

namespace DocumentManager.Models {
	public class IdNamePair {
		public int Id;
		public string Name;

		public IdNamePair() {
		}

		public IdNamePair(int id, string name) {
			Id = id;
			Name = name;
		}

		public override string ToString() {
			return $"[{Id}] {Name}";
		}
	}
}
