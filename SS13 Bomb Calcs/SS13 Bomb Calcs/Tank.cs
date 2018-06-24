using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SS13_Bomb_Calcs {
	class Tank {
		public Gases gases = new Gases();

		public double pressure;
		public double volume;
		public double temperature;

		public Tank(double volume) {
			this.volume = volume;
		}

		public Tank(double volume , double pressure , double temperature , double plasmaPercentage , double oxygenPercentage , double nitrogenPercentage , double carbonDioxidePercentage , double waterPercentage , double nobliumPercentage , double nitrylPercentage, double nitrousPercentage , double tritiumPercentage, double stimulumPercentage, double pluoxiumPercentage , double BZPercentage) {
			this.volume = volume;
			fill(pressure,temperature, plasmaPercentage,oxygenPercentage,nitrogenPercentage,carbonDioxidePercentage,waterPercentage,nobliumPercentage,nitrylPercentage,nitrousPercentage,tritiumPercentage,stimulumPercentage,pluoxiumPercentage,BZPercentage);
		}

		public void fill(double pressure , double temperature , double plasmaPercentage , double oxygenPercentage , double nitrogenPercentage , double carbonDioxidePercentage , double waterPercentage , double nobliumPercentage , double nitrylPercentage, double nitrousPercentage , double tritiumPercentage, double stimulumPercentage, double pluoxiumPercentage , double BZPercentage) {
			double totalPercentage = plasmaPercentage + oxygenPercentage + nitrogenPercentage + carbonDioxidePercentage + waterPercentage + nobliumPercentage + nitrylPercentage + nitrousPercentage + tritiumPercentage + stimulumPercentage + pluoxiumPercentage + BZPercentage;
			if (totalPercentage > 1.0001 || totalPercentage < 0.9999) throw new ArithmeticException($"Total percentage added up to {totalPercentage}");

			this.pressure = pressure;
			this.temperature = temperature;

			double totalNewMoles = pressure * volume / (Constants.R_IDEAL_GAS_EQUATION * temperature);
			//Debug.WriteLine($"Total new moles: {totalNewMoles}, p {plasmaPercentage}, o {oxygenPercentage}, n {nitrogenPercentage}, co2 {carbonDioxidePercentage} nitryl {nitrylPercentage} trit {tritiumPercentage}");

			gases.plasma.moles = totalNewMoles * plasmaPercentage;
			gases.oxygen.moles = totalNewMoles * oxygenPercentage;
			gases.nitrogen.moles = totalNewMoles * nitrogenPercentage;
			gases.carbonDioxide.moles = totalNewMoles * carbonDioxidePercentage;
			gases.waterVapour.moles = totalNewMoles * waterPercentage;
			gases.noblium.moles = totalNewMoles * nobliumPercentage;
            gases.nitryl.moles = totalNewMoles * nitrylPercentage;
			gases.nitrousOxide.moles = totalNewMoles * nitrousPercentage;
            gases.tritium.moles = totalNewMoles * tritiumPercentage;
            gases.stimulum.moles = totalNewMoles * stimulumPercentage;
            gases.pluoxium.moles = totalNewMoles * pluoxiumPercentage;
			gases.BZ.moles = totalNewMoles * BZPercentage;
		}

		public double getTotalMoles() {
			double totalMoles = 0;
			foreach(Gas gas in gases) {
				//Debug.WriteLine($"Adding: {gas.moles} (shc: {gas.specificHeatCapacity}) => {totalMoles}");
				totalMoles += gas.moles;
			}
			return totalMoles;
		}

        public double getHeatCapacity() {
            //Debug.WriteLine($"{gases.tritium.moles}");
            double hc = 0;
			foreach(Gas gas in gases) {
				hc += gas.moles * gas.specificHeatCapacity;
                //Debug.WriteLine($"{gas.moles} has specific heat {gas.specificHeatCapacity}");
			}
            return hc;
        }

		public double calculatePressure() {
			pressure = getTotalMoles() * Constants.R_IDEAL_GAS_EQUATION * temperature / volume;
			return pressure;
		}

		public String contentsAsPercentage() {
			double totalMoles = getTotalMoles();
			return $"Total Moles: {totalMoles}, Plasma: {gases.plasma.moles}({gases.plasma.moles/totalMoles*100}%), O2: {gases.oxygen.moles}({gases.oxygen.moles/totalMoles*100}%), N2: {gases.nitrogen.moles}({gases.nitrogen.moles/totalMoles*100}%), CO2: {gases.carbonDioxide.moles}({gases.carbonDioxide.moles/totalMoles*100}%), N20: {gases.nitrousOxide.moles}({gases.nitrousOxide.moles/totalMoles*100}%), NO2: {gases.nitryl.moles}({gases.nitryl.moles/totalMoles*100}%), H20: {gases.waterVapour.moles}({gases.waterVapour.moles/totalMoles*100}%), Hyper-Noblium: {gases.noblium.moles}({gases.noblium.moles/totalMoles*100}%), Tritium: {gases.tritium.moles}({gases.tritium.moles/totalMoles*100}%, Stimulum: {gases.stimulum.moles}({gases.stimulum.moles/totalMoles*100}%), Pluoxium: {gases.pluoxium.moles}({gases.pluoxium.moles/totalMoles*100}), BZ: {gases.BZ.moles}({gases.BZ.moles/totalMoles*100}%)";
		}

		public override string ToString() {
			return $"{base.ToString()} Pressure: {pressure}kPa, Temperature: {temperature}K({temperature-Constants.T0C}C), Volume: {volume}, Contents: {contentsAsPercentage()}";
			
		}
	}
}
