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
        //overcharge is contained in extra charges
        //start with 2/3 add 1/2 per stack

        //incredibly outdated and not not even working anyway. let it slowly rot into dust until I maybe revsit it in the future as a slightly more experienced monkey on a typewriter
        /*
        private float _pseudoInitialSpin;
        private Run.FixedTimeStamp _pseudoSnapshotTime;
        private float _pseudoInitialCharge;

        private CharacterBody _turbineOwnerBody;
        private GenericOwnership _turbineGenericOwnership;

        private float _baseSpinPerKill = 0.01f;

        private int _discStacks {
            get {
                int stacks = _turbineOwnerBody ? _turbineOwnerBody.inventory ? _turbineOwnerBody.inventory.GetItemCount(ItemIndex.LaserTurbine) : 1 : 1;

                Utils.Log($"_discStacks: {stacks}");

                return stacks;
            }
        }

        private float _stackedSpinPerKill {    
            get {
                if (_baseSpinPerKill == 0.01f) {
                    Utils.Log("_baseSpinPerKill = 0.01f. either it didn't get the real value or the real value has changed");
                }
                float stacked = _baseSpinPerKill * _discStacks;

                Utils.Log($"_stackedSpinPerKill: {stacked}");

                return stacked;
            }
        }

        private Run.FixedTimeStamp _runNow {
            get {
                return Run.FixedTimeStamp.now;
            }
        }

        public void Awake() {
            //nerf damage (remove stacking)
            On.EntityStates.LaserTurbine.LaserTurbineBaseState.OnEnter += LaserTurbineBaseState_OnEnter;

            On.EntityStates.LaserTurbine.LaserTurbineBaseState.GetDamage += LaserTurbineBaseState_GetDamage;

            //stack chargeRate (fuck how do I communicate this)
            On.RoR2.LaserTurbineController.Awake += LaserTurbineController_Awake;
            On.RoR2.LaserTurbineController.OnOwnerKilledOtherServer += LaserTurbineController_OnOwnerKilledOtherServer;
            On.RoR2.LaserTurbineController.ExpendCharge += LaserTurbineController_ExpendCharge;

            //all for a freakin read
            // k i may need to revisit this for the overcharge mechanic
            // my god should have just asked for what I now know is reflection
            //On.RoR2.LaserTurbineController.FixedUpdate += LaserTurbineController_FixedUpdate;
            On.RoR2.LaserTurbineController.OnStartServer += LaserTurbineController_OnStartServer;
        }

        private void LaserTurbineBaseState_OnEnter(On.EntityStates.LaserTurbine.LaserTurbineBaseState.orig_OnEnter orig, EntityStates.LaserTurbine.LaserTurbineBaseState self) {
            orig(self);
            getOwnershipAndBody(self);
        }

        private void getOwnershipAndBody(EntityStates.LaserTurbine.LaserTurbineBaseState self) {

            //see if all these gets were being chached for a reason
            _turbineGenericOwnership = self.outer.GetComponent<GenericOwnership>();
            //ownerbody in their property is being cached so they don't run getcomponent all the time.
            _turbineOwnerBody = (_turbineGenericOwnership != null) ? _turbineGenericOwnership.ownerObject.GetComponent<CharacterBody>() : null;
        }

        private float LaserTurbineBaseState_GetDamage(On.EntityStates.LaserTurbine.LaserTurbineBaseState.orig_GetDamage orig, EntityStates.LaserTurbine.LaserTurbineBaseState self) {

            float num = 1f;
            if (_turbineGenericOwnership) {
                if (_turbineOwnerBody.inventory) {
                    num = _turbineOwnerBody.damage;
                }
            }
            Utils.Log($"LaserTurbineBaseState_GetDamage: {num}");
            return num;
        }

        //

        private void LaserTurbineController_Awake(On.RoR2.LaserTurbineController.orig_Awake orig, LaserTurbineController self) {
            _baseSpinPerKill = self.spinPerKill;

            orig(self);
        }

        private void LaserTurbineController_FixedUpdate(On.RoR2.LaserTurbineController.orig_FixedUpdate orig, LaserTurbineController self) {

            Utils.Log($"{self.charge.ToString("0.00")} | {pseudoCalculateSpin(_runNow, self).ToString("0.00")} ({pseudoCalculateCharge(_runNow, self).ToString("0.00")})");

            orig(self);
        }

        //well this was a failure. I am very glad we have a public reference to charge
        //at least the spin calculation wasn't a similarly colossal failure

        // god fuckin dammit now i need this to work so I can calculate the overcharge
        private float pseudoCalculateCharge(Run.FixedTimeStamp now, LaserTurbineController self) {

            float num = now - _pseudoSnapshotTime;
            float num2 = self.minSpin * num;
            float num3 = _pseudoInitialSpin - self.minSpin;
            float t = Mathf.Min(Trajectory.CalculateFlightDuration(num3, -self.spinDecayRate) * 0.5f, num);
            float num4 = Trajectory.CalculatePositionYAtTime(0f, num3, t, -self.spinDecayRate);
            return Mathf.Min(_pseudoInitialCharge + num2 + num4, 1f);
        }

        private float pseudoCalculateSpin(Run.FixedTimeStamp now, LaserTurbineController self) {

            return Mathf.Max(_pseudoInitialSpin - self.spinDecayRate * (_runNow - _pseudoSnapshotTime), self.minSpin); ;
        }

        private void LaserTurbineController_OnStartServer(On.RoR2.LaserTurbineController.orig_OnStartServer orig, LaserTurbineController self) {

            _pseudoInitialSpin = self.minSpin;
            _pseudoSnapshotTime = _runNow;

            orig(self);
        }

        private void LaserTurbineController_OnOwnerKilledOtherServer(On.RoR2.LaserTurbineController.orig_OnOwnerKilledOtherServer orig, LaserTurbineController self) {

            Run.FixedTimeStamp now = _runNow;
            float spin = pseudoCalculateSpin(now, self);
            float charge = pseudoCalculateCharge(now, self);
            spin = Mathf.Min(spin + self.spinPerKill, self.maxSpin);

            _pseudoInitialSpin = spin;
            _pseudoInitialCharge = charge;
            _pseudoSnapshotTime = now;

            self.spinPerKill = _stackedSpinPerKill;

            orig(self);
        }

        private void LaserTurbineController_ExpendCharge(On.RoR2.LaserTurbineController.orig_ExpendCharge orig, LaserTurbineController self) {

            Run.FixedTimeStamp now = _runNow;
            float spin = pseudoCalculateSpin(now, self);
            spin += self.spinPerKill;

            _pseudoInitialSpin += spin;
            _pseudoInitialCharge = 0f;
            _pseudoSnapshotTime = now;

            self.spinPerKill = _stackedSpinPerKill;

            orig(self);
        }*/
    }
}

