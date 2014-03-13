using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using micfort.GHL.Math2;

namespace CG_2IV05.Common
{
	[Serializable]
	public class HyperPointSerializable<T> : ISerializable
		where T : IComparable<T>
	{
		public HyperPoint<T> Value { get; set; } 

		public HyperPointSerializable()
		{
			
		}

		public HyperPointSerializable(HyperPoint<T> value)
		{
			this.Value = value;
		}

		protected HyperPointSerializable(SerializationInfo info, StreamingContext context)
		{
			T[] values = (T[])info.GetValue("values", typeof (T[]));
			Value = new HyperPoint<T>(values);
		}

		public static implicit operator HyperPointSerializable<T>(HyperPoint<T> p)
		{
			return new HyperPointSerializable<T>(p);
		}

		public static implicit operator HyperPoint<T>(HyperPointSerializable<T> p)
		{
			return p.Value;
		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]

		#region Implementation of ISerializable

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("values", Value.p);
		}

		#endregion
	}
}
