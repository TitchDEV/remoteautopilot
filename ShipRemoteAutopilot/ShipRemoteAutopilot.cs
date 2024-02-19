using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShipRemoteAutopilot
{
    public class ShipRemoteAutopilot : ModBehaviour
    {

        bool CounterActive = false;
        bool isTimerDone = false;
        bool isShipAligned = false;

        AstroObject planetaryBody;


        //10 Seconds countdown for moving ship to air
        IEnumerator ThrusterTiming()
        {
            WaitForSeconds wait = new WaitForSeconds(10);
            yield return wait;

            isTimerDone = true;
        }
        //2 Second countdown for aligning the ship to the surface of the planet for landing
        IEnumerator ShipAlignmentTiming()
        {
            WaitForSeconds wait = new WaitForSeconds(2);
            yield return wait;

            isShipAligned = true;
        }


        //Links the Align and Land Ship functions to the Autopilot Arrival flag
        private void Start()
        {
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                var shipBody = FindObjectOfType<ShipBody>();
                shipBody.GetComponent<Autopilot>().OnArriveAtDestination += AlignShip;
            };
        }

        //Function to Align the ship to face the floor of the planet
        public void AlignShip(float arrivalError)
        {
            Locator.GetShipBody().EnableCollisionDetection();
            var AligningReferenceFrame = planetaryBody.GetOWRigidbody().GetReferenceFrame();
            Locator.GetShipBody().GetComponent<AlignShipWithReferenceFrame>().OnEnterLandingMode(AligningReferenceFrame);
            StartCoroutine(ShipAlignmentTiming());
        }
        
        //Function to activate autopilot to location
        public void TravelToLocation()
        {
            //Sets the timer to be done
            isTimerDone = false;

            var ShipReferenceFrame = planetaryBody.GetOWRigidbody().GetReferenceFrame();
            ModHelper.Console.WriteLine("Ship Reference Frame Name : " + ShipReferenceFrame);
            Locator.GetPlayerTransform().GetComponent<ReferenceFrameTracker>().TargetReferenceFrame(ShipReferenceFrame);
            Locator.GetShipBody().GetComponent<Autopilot>().FlyToDestination(ShipReferenceFrame);
        }


        private void Update()
        {
            //Controls upward thrust when counter is counting
            if (CounterActive)
            {
                Locator.GetShipBody().GetComponent<ThrusterModel>().AddTranslationalInput(Vector3.up);
            }

            //When the ship is aligned 
            if (isShipAligned)
            {
                Locator.GetShipBody().GetComponent<AlignShipWithReferenceFrame>().OnExitLandingMode();
                isShipAligned = false;
            }

            //When the timer is done, activate the travelling from orbit
            if (isTimerDone)
            {
                CounterActive = false;
                isTimerDone = false;
                TravelToLocation();
            }

            //Go to Location
            if (Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                Locator.GetShipBody().DisableCollisionDetection();
                Locator.GetShipBody().GetComponent<LandingPadManager>()._isLanded = true;

                if (Locator.GetShipBody().GetComponent<LandingPadManager>()._isLanded == true)
                {
                    CounterActive = true;
                    StartCoroutine(ThrusterTiming());
                }
            }


            //Select the Planet from the Planet Index
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.TimberHearth);
                var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Timber Hearth", 10f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);
            }

            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.GiantsDeep);
                var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Giant's Deep", 10f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);
            }

            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.BrittleHollow);
                var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Brittle Hollow", 10f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);
            }

            if (Keyboard.current.numpad4Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.DarkBramble);
                var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Dark Bramble", 10f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);
            }

            if (Keyboard.current.numpad5Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.TowerTwin);
                var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Ash Twin", 10f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);
            }

            if (Keyboard.current.numpad6Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.CaveTwin);
                var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Ember Twin", 10f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);
            }
            
            if (Keyboard.current.numpad7Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.Comet);
                var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - The Interloper", 10f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);
            }

            if (Keyboard.current.numpad8Key.wasPressedThisFrame)
            {
                planetaryBody = Locator.GetAstroObject(AstroObject.Name.RingWorld);
                var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - <REDACTED>", 10f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);
            }
        }
    }
}
