using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Bomb_Calcs {
	class Tank {
		public Gases gases = new Gases();

		public float pressure;
		public float volume;
		public float temperature;

		public Tank(float volume) {
			this.volume = volume;
		}

		public Tank(float volume , float pressure , float temperature , float plasmaPercentage , float oxygenPercentage , float nitrogenPercentage , float carbonDioxidePercentage , float waterPercentage , float freonPercentage , float nitrousPercentage , float BZPercentage) {
			this.volume = volume;
			fill(pressure,temperature, plasmaPercentage,oxygenPercentage,nitrogenPercentage,carbonDioxidePercentage,waterPercentage,freonPercentage,nitrousPercentage,BZPercentage);
		}

		public void fill(float pressure, float temperature, float plasmaPercentage, float oxygenPercentage, float nitrogenPercentage, float carbonDioxidePercentage, float waterPercentage, float freonPercentage, float nitrousPercentage, float BZPercentage) {
			float totalPercentage = plasmaPercentage + oxygenPercentage + nitrogenPercentage + carbonDioxidePercentage + waterPercentage + freonPercentage + nitrousPercentage + BZPercentage;
			if (totalPercentage > 1.0001 || totalPercentage < 0.9999) throw new ArithmeticException($"Total percentage added up to {totalPercentage}");

			this.pressure = pressure;
			this.temperature = temperature;

			float totalNewMoles = pressure * volume / (Constants.R_IDEAL_GAS_EQUATION * temperature);
			//Debug.WriteLine($"Total new moles: {totalNewMoles}, p {plasmaPercentage}, o {oxygenPercentage}, n {nitrogenPercentage}, co2 {carbonDioxidePercentage}");

			gases.plasma.moles = totalNewMoles * plasmaPercentage;
			gases.oxygen.moles = totalNewMoles * oxygenPercentage;
			gases.nitrogen.moles = totalNewMoles * nitrogenPercentage;
			gases.carbonDioxide.moles = totalNewMoles * carbonDioxidePercentage;
			gases.waterVapour.moles = totalNewMoles * waterPercentage;
			gases.freon.moles = totalNewMoles * freonPercentage;
			gases.nitrousOxide.moles = totalNewMoles * nitrousPercentage;
			gases.BZ.moles = totalNewMoles * BZPercentage;
		}

		public float getTotalMoles() {
			float totalMoles = 0;
			foreach(Gas gas in gases) {
				//Debug.WriteLine($"Adding: {gas.moles} (shc: {gas.specificHeatCapacity}) => {totalMoles}");
				totalMoles += gas.moles;
			}
			return totalMoles;
		}

        public float getHeatCapacity() {
            float hc = 0;
			foreach(Gas gas in gases) {
				hc += gas.moles * gas.specificHeatCapacity;
			}
            return hc;
        }

		public float calculatePressure() {
			pressure = getTotalMoles() * Constants.R_IDEAL_GAS_EQUATION * temperature / volume;
			return pressure;
		}

		public String contentsAsPercentage() {
			float totalMoles = getTotalMoles();
			return $"Total Moles: {totalMoles}, Plasma: {gases.plasma.moles}({gases.plasma.moles/totalMoles*100}%), O2: {gases.oxygen.moles}({gases.oxygen.moles/totalMoles*100}%), N2: {gases.nitrogen.moles}({gases.nitrogen.moles/totalMoles*100}%), CO2: {gases.carbonDioxide.moles}({gases.carbonDioxide.moles/totalMoles*100}%), N20: {gases.nitrousOxide.moles}({gases.nitrousOxide.moles/totalMoles*100}%), H20: {gases.waterVapour.moles}({gases.waterVapour.moles/totalMoles*100}%), Freon: {gases.freon.moles}({gases.freon.moles/totalMoles*100}%), BZ: {gases.BZ.moles}({gases.BZ.moles/totalMoles*100}%)";
		}

		public override string ToString() {
			return $"{base.ToString()} Pressure: {pressure}kPa, Temperature: {temperature}K({temperature-Constants.T0C}C), Volume: {volume}, Contents: {contentsAsPercentage()}";
			
		}
	}
}
