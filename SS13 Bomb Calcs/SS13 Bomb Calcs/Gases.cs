using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Bomb_Calcs {
	class Gases {
		public Gas[] gaslist;

		public Gas plasma = new Gas();
		public Gas oxygen = new Gas();
		public Gas nitrogen = new Gas();
		public Gas carbonDioxide = new Gas();
		public Gas waterVapour = new Gas();
		public Gas freon = new Gas();
		public Gas nitrousOxide = new Gas();
		public Gas BZ = new Gas();

		public Gases() {
			gaslist = new SS13_Bomb_Calcs.Gas[8] {plasma,oxygen,nitrogen,carbonDioxide,waterVapour,freon,nitrousOxide,BZ };

			plasma.specificHeatCapacity = Constants.PLASMA_SPECIFIC_HEAT_CAPACITY;
			oxygen.specificHeatCapacity = Constants.O2_SPECIFIC_HEAT_CAPACITY;
			nitrogen.specificHeatCapacity = Constants.N2_SPECIFIC_HEAT_CAPACITY;
			carbonDioxide.specificHeatCapacity = Constants.CO2_SPECIFIC_HEAT_CAPACITY;
			waterVapour.specificHeatCapacity = Constants.WATERV_SPECIFIC_HEAT_CAPACITY;
			freon.specificHeatCapacity = Constants.FREON_SPECIFIC_HEAT_CAPACITY;
			nitrousOxide.specificHeatCapacity = Constants.N2O_SPECIFIC_HEAT_CAPACITY;
			BZ.specificHeatCapacity = Constants.BZ_SPECIFIC_HEAT_CAPACITY;
		}

		public IEnumerator GetEnumerator()
        {
            return new AEnumerator(this);
        }

        private class AEnumerator : IEnumerator
        {
            public AEnumerator(Gases inst)
            {
                this.instance = inst;
            }

            private Gases instance;
            private int position = -1;

            public object Current
            {
                get
                {
                    return instance.gaslist[position];
                }
            }

            public bool MoveNext()
            {
                position++;
                return (position < instance.gaslist.Length);
            }

            public void Reset()
            {
                position = -1;
            }

        }
	}
}
