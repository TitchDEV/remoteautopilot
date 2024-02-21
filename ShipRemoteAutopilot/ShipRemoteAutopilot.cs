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
        bool keysMode;

        AstroObject planetaryBody;


        //Enumerator Coroutines for the Upwards thrust and Alignment functions.
        IEnumerator ThrusterTiming()
        {
            WaitForSeconds wait = new WaitForSeconds(10);
            yield return wait;

            isTimerDone = true;
        }
        IEnumerator ShipAlignmentTiming()
        {
            WaitForSeconds wait = new WaitForSeconds(2);
            yield return wait;

            isShipAligned = true;
        }


        //Links the AlignShip() Function to be ran when OnArriveAtDestination() occurs.
        private void Start()
        {

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;

                var shipBody = FindObjectOfType<ShipBody>();
                shipBody.GetComponent<Autopilot>().OnArriveAtDestination += AlignShip;

                keysMode = ModHelper.Config.GetSettingsValue<bool>("Alternate Keys Mode");
            };
        }

        //Function to Align the ship to face the floor of the planet
        public void AlignShip(float arrivalError)
        {
            var notificationData = new NotificationData(NotificationTarget.Player, "Autopilot - Ship arrived at destination, entering orbit...", 5f, true);
            NotificationManager.SharedInstance.PostNotification(notificationData, false);

            //Re-enable the ships collision to correctly land on surface.
            Locator.GetShipBody().EnableCollisionDetection();

            //Get the current reference frame of currently selected planet.
            var AligningReferenceFrame = planetaryBody.GetOWRigidbody().GetReferenceFrame();

            //Align the ship with the planets surface.
            Locator.GetShipBody().GetComponent<AlignShipWithReferenceFrame>().OnEnterLandingMode(AligningReferenceFrame);
            
            //No idea WHAT this does but- its here.
            StartCoroutine(ShipAlignmentTiming());
        }
        
        //Actual function for activating the traditional Outer Wild's Autopilot.
        public void TravelToLocation()
        {
            //Sets the thruster timer to be complete again.
            isTimerDone = false;

            //Gets the reference frame for the selected planet to give to Autopilot component.
            var ShipReferenceFrame = planetaryBody.GetOWRigidbody().GetReferenceFrame();
            
            //Targets the reference frame and activates the Autopilot module.
            Locator.GetPlayerTransform().GetComponent<ReferenceFrameTracker>().TargetReferenceFrame(ShipReferenceFrame);
            Locator.GetShipBody().GetComponent<Autopilot>().FlyToDestination(ShipReferenceFrame);
        }


        private void Update()
        {
            //This is performed when "Enter" is pressed, Enter sets this to true and starts firing the Thrusters
            if (CounterActive)
            {
                Locator.GetShipBody().GetComponent<ThrusterModel>().AddTranslationalInput(Vector3.up);
            }

            //Catch to stop the ship going mental when the ship is aligned correctly.
            if (isShipAligned)
            {
                Locator.GetShipBody().GetComponent<AlignShipWithReferenceFrame>().OnExitLandingMode();
                isShipAligned = false;
            }

            //When the 10 second Thruster timer is complete, it sets this to True and, as show, disables the upwards thruster and itself, and begins travel to the destination.
            if (isTimerDone)
            {
                CounterActive = false;
                isTimerDone = false;
                
                var notificationData = new NotificationData(NotificationTarget.Player, "Thrusters Complete. Beginning travel to destination...", 5f, true);
                NotificationManager.SharedInstance.PostNotification(notificationData, false);

                TravelToLocation();
            }


            if (keysMode == false)
            {
                //Activation key for the Remote Autopilot function.
                if (Keyboard.current.numpadEnterKey.wasPressedThisFrame)
                {

                    var notificationData = new NotificationData(NotificationTarget.Player, "Autopilot Engaged - Firing Upward Thrusters...", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);

                    //Disables the ships collision to stop it colliding with the sun and- yknow- dying.
                    Locator.GetShipBody().DisableCollisionDetection();

                    //Starts the Upwards thrust and begins the timer so that it disables when neccesary.
                    CounterActive = true;
                    StartCoroutine(ThrusterTiming());
                }


                //Options to choose the location in which the ship should fly to. The player is notified on their Suits HUD which item is selected.
                if (Keyboard.current.numpad1Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.TimberHearth);
                    var notificationData = new NotificationData(NotificationTarget.Player, "AP - Destination Set - Timber Hearth", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.numpad2Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.GiantsDeep);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Giant's Deep", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.numpad3Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.BrittleHollow);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Brittle Hollow", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.numpad4Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.DarkBramble);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Dark Bramble", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.numpad5Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.TowerTwin);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Ash Twin", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.numpad6Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.CaveTwin);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Ember Twin", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.numpad7Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.Comet);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - The Interloper", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }
            }
            
            else if (keysMode == true)
            {
                //Activation key for the Remote Autopilot function.
                if (Keyboard.current.enterKey.wasPressedThisFrame)
                {
                   var notificationData = new NotificationData(NotificationTarget.Player, "Autopilot Engaged - Firing Upward Thrusters...", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);

                    //Disables the ships collision to stop it colliding with the sun and- yknow- dying.
                    Locator.GetShipBody().DisableCollisionDetection();

                    //Starts the Upwards thrust and begins the timer so that it disables when neccesary.
                    CounterActive = true;
                    StartCoroutine(ThrusterTiming());
                }


                //Options to choose the location in which the ship should fly to. The player is notified on their Suits HUD which item is selected.
                if (Keyboard.current.digit1Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.TimberHearth);
                    var notificationData = new NotificationData(NotificationTarget.Player, "AP - Destination Set - Timber Hearth", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.digit2Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.GiantsDeep);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Giant's Deep", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.digit3Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.BrittleHollow);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Brittle Hollow", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.digit4Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.DarkBramble);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Dark Bramble", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.digit5Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.TowerTwin);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Ash Twin", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.digit6Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.CaveTwin);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - Ember Twin", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }

                if (Keyboard.current.digit7Key.wasPressedThisFrame)
                {
                    planetaryBody = Locator.GetAstroObject(AstroObject.Name.Comet);
                    var notificationData = new NotificationData(NotificationTarget.All, "AP - Destination Set - The Interloper", 5f, true);
                    NotificationManager.SharedInstance.PostNotification(notificationData, false);
                }
            }
        }
    }
}
