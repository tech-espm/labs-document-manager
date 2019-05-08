using DocumentManager.Models;
using System;
using System.Collections.Generic;

namespace DocumentManager.Attributes {
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public sealed class AccessControlAttribute : Attribute {
		public AccessControlAttribute(bool anonymous) {
			Anonymous = anonymous;
		}

		public AccessControlAttribute(Feature requestedFeature, bool apiCall = false) {
			RequestedFeature = requestedFeature;
			ApiCall = apiCall;
		}

		public bool Anonymous { get; }

		public Feature RequestedFeature { get; }

		public bool ApiCall { get; }
	}
}
