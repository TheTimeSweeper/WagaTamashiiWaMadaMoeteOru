using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace HyperTurbine {

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.TheTimeSweeper.HyperTurbine", "Hyper Turbine", "0.0.3")]
    public class HyperTurbineMod : BaseUnityPlugin {
        //heavy nerf to damage
        //disc always pierces
        //
        private float _pseudoInitialSpin;
        private Run.FixedTimeStamp _pseudoSnapshotTime;
        private float _pseudoInitialCharge;

        private float _baseSpinPerKill;
        private int _discStacks;

        private float _stackedSpinPerKill {
            get {
                return _baseSpinPerKill * _discStacks;
            }
        }

        

        Run.FixedTimeStamp runNow {
            get {
                return Run.FixedTimeStamp.now;
            }
        }

        public void Awake() {
            //nerf damage (remove stacking)
            On.EntityStates.LaserTurbine.LaserTurbineBaseState.GetDamage += LaserTurbineBaseState_GetDamage;

            //stack chargeRate (fuck how do I communicate this)



            //all for a freakin read
            On.RoR2.LaserTurbineController.Awake += LaserTurbineController_Awake;
            On.RoR2.LaserTurbineController.FixedUpdate += LaserTurbineController_FixedUpdate;

            On.RoR2.LaserTurbineController.OnStartServer += LaserTurbineController_OnStartServer;
            On.RoR2.LaserTurbineController.OnOwnerKilledOtherServer += LaserTurbineController_OnOwnerKilledOtherServer;
            On.RoR2.LaserTurbineController.ExpendCharge += LaserTurbineController_ExpendCharge;
        }

        private float LaserTurbineBaseState_GetDamage(On.EntityStates.LaserTurbine.LaserTurbineBaseState.orig_GetDamage orig, EntityStates.LaserTurbine.LaserTurbineBaseState self) {

            //there's gotta be another way
            GenericOwnership genericOwnership = self.outer.GetComponent<GenericOwnership>();
            CharacterBody charBod = self.outer.GetComponent<CharacterBody>();

            return charBod.damage;
        }

        //

        private void LaserTurbineController_Awake(On.RoR2.LaserTurbineController.orig_Awake orig, LaserTurbineController self) {
            _baseSpinPerKill = self.spinPerKill;
            orig(self);
        }

        private void LaserTurbineController_FixedUpdate(On.RoR2.LaserTurbineController.orig_FixedUpdate orig, LaserTurbineController self) {
            orig(self);

            Run.FixedTimeStamp now = runNow;

            Debug.Log($"{self.charge.ToString("0.00")} | {pseudoCalculateSpin(now, self).ToString("0.00")})");
        }

        //well this was a failure. I am very glad we have a public reference to charge
        //and also the spin calculation wasn't a similarly colossal failure
        private float pseudoCalculateCharge(Run.FixedTimeStamp now, LaserTurbineController self) {

            float num = now - _pseudoSnapshotTime;
            float num2 = self.minSpin * num;
            float num3 = _pseudoInitialSpin - self.minSpin;
            float t = Mathf.Min(Trajectory.CalculateFlightDuration(num3, -self.spinDecayRate) * 0.5f, num);
            float num4 = Trajectory.CalculatePositionYAtTime(0f, num3, t, -self.spinDecayRate);
            return Mathf.Min(_pseudoInitialCharge + num2 + num4, 1f);
        }

        private float pseudoCalculateSpin(Run.FixedTimeStamp now, LaserTurbineController self) {

            return Mathf.Max(_pseudoInitialSpin - self.spinDecayRate * (runNow - _pseudoSnapshotTime), self.minSpin); ;
        }

        private void LaserTurbineController_OnStartServer(On.RoR2.LaserTurbineController.orig_OnStartServer orig, LaserTurbineController self) {

            _pseudoInitialSpin = self.minSpin;
            _pseudoSnapshotTime = runNow;

            orig(self);
        }

        private void LaserTurbineController_OnOwnerKilledOtherServer(On.RoR2.LaserTurbineController.orig_OnOwnerKilledOtherServer orig, LaserTurbineController self) {

            Run.FixedTimeStamp now = runNow;
            float spin = pseudoCalculateSpin(now, self);
            float charge = pseudoCalculateCharge(now, self);
            spin = Mathf.Min(spin + self.spinPerKill, self.maxSpin);

            _pseudoInitialSpin = spin;
            _pseudoInitialCharge = charge;
            _pseudoSnapshotTime = now;

            orig(self);
        }

        private void LaserTurbineController_ExpendCharge(On.RoR2.LaserTurbineController.orig_ExpendCharge orig, LaserTurbineController self) {

            Run.FixedTimeStamp now = runNow;
            float spin = pseudoCalculateSpin(now, self);
            spin += self.spinPerKill;

            _pseudoInitialSpin += spin;
            _pseudoInitialCharge = 0f;
            _pseudoSnapshotTime = now;

            orig(self);
        }
    }
}

