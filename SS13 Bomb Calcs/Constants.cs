using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS13_Bomb_Calcs {
	public class Constants {
		public static float MINIMUM_HEAT_CAPACITY = 0.0003F;
		public static float ATMOSPHERE = 101.325F;
		public static float TANK_FRAGMENT_PRESSURE = 40*ATMOSPHERE;
		public static float TANK_FRAGMENT_SCALE = 6*ATMOSPHERE;
		public static float T0C = 273.15F;
		public static float PLASMA_MINIMUM_BURN_TEMPERATURE = 100+T0C;
        public static float FIRE_MINIMUM_TEMPERATURE_TO_EXIST = 100 + T0C;
		public static float PLASMA_OXYGEN_FULLBURN = 10;
        public static float PLASMA_UPPER_TEMPERATURE = 1370 + T0C;
		public static float OXYGEN_BURN_BASE_RATE = 1.4F;
		public static float FIRE_PLASMA_ENERGY_RELEASED = 3000000;
		public static float PLASMA_BURN_RATE_DELTA = 9;
		public static float MAX_TANK_PRESSURE = 10*ATMOSPHERE;
		public static float TANK_VOLUME = 70;
		public static float MINIMUM_TEMPERATURE_DELTA_TO_CONSIDER = 0.5F;

		public static float R_IDEAL_GAS_EQUATION = 8.31F;
	
		public static float O2_SPECIFIC_HEAT_CAPACITY = 20;
		public static float PLASMA_SPECIFIC_HEAT_CAPACITY = 200;
		public static float N2_SPECIFIC_HEAT_CAPACITY = 20;
		public static float CO2_SPECIFIC_HEAT_CAPACITY = 30;
		public static float WATERV_SPECIFIC_HEAT_CAPACITY = 40;
		public static float FREON_SPECIFIC_HEAT_CAPACITY = 2000;
		public static float N2O_SPECIFIC_HEAT_CAPACITY = 40;
		public static float BZ_SPECIFIC_HEAT_CAPACITY = 20;

	}
}
