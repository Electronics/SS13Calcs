using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS13_Bomb_Calcs {
	class Gas {
		public float moles;
		public float specificHeatCapacity;

		public override string ToString() {
			return $"{base.ToString()}: moles: {moles}, specific: {specificHeatCapacity}";
		}
	}
}
