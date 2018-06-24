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
		public Gas noblium = new Gas();
        public Gas nitryl = new Gas();
		public Gas nitrousOxide = new Gas();
        public Gas tritium = new Gas();
        public Gas stimulum = new Gas();
        public Gas pluoxium = new Gas();
		public Gas BZ = new Gas();

		public Gases() {
			gaslist = new SS13_Bomb_Calcs.Gas[12] {plasma,oxygen,nitrogen,carbonDioxide,waterVapour,noblium,nitrousOxide,tritium,stimulum,nitryl,pluoxium,BZ };

			plasma.specificHeatCapacity = Constants.PLASMA_SPECIFIC_HEAT_CAPACITY;
			oxygen.specificHeatCapacity = Constants.O2_SPECIFIC_HEAT_CAPACITY;
			nitrogen.specificHeatCapacity = Constants.N2_SPECIFIC_HEAT_CAPACITY;
			carbonDioxide.specificHeatCapacity = Constants.CO2_SPECIFIC_HEAT_CAPACITY;
			waterVapour.specificHeatCapacity = Constants.WATERV_SPECIFIC_HEAT_CAPACITY;
			noblium.specificHeatCapacity = Constants.NOBLIUM_SPECIFIC_HEAT_CAPACITY;
			nitrousOxide.specificHeatCapacity = Constants.N2O_SPECIFIC_HEAT_CAPACITY;
            tritium.specificHeatCapacity = Constants.TRITIUM_SPECIFIC_HEAT_CAPACITY;
            stimulum.specificHeatCapacity = Constants.STIMULUM_SPECIFIC_HEAT_CAPACITY;
            //nitryl.specificHeatCapacity = Constants.NITRYL_SPEC
            pluoxium.specificHeatCapacity = Constants.PLUOXIUM_SPECIFIC_HEAT_CAPACITY;
			BZ.specificHeatCapacity = Constants.BZ_SPECIFIC_HEAT_CAPACITY;

            carbonDioxide.fusionPower = 3;
            waterVapour.fusionPower = 4;
            nitryl.fusionPower = 10;
            tritium.fusionPower = 1;
            BZ.fusionPower = 15;
            stimulum.fusionPower = 7;
            pluoxium.fusionPower = 10;
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
