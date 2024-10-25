# BetterHudLite

Somewhat inspired by the BetterHud mod of old, by DekuDesu.

Ping me (`thetimesweeper`) on the ror2 modding discord or the enforcer discord (https://discord.gg/csaEQDnMH8) if you have any issues/feedback.  

___
## Health Bar and Skills Moved to Center:
Config to disable either  
(or both, which would make the mod literally do nothing, like why, but you can)
![The Gang's All He](https://i.imgur.com/lGdkHK4.png)
*Health bar height and width customizable too!*
___
### Credits:
DestroyedClone - fixed skills panel
___
### Known Issues:
 - I've accounted for the ExtendedLoadout mod a bit but it's still kinda janky
 - lot of cases untested. don't be shy letting me know
___
### Plans:
 - ~~risk of options~~
 - move buffs to health bar (config)
 - move sprint and inv buttons (config)
 - move level below health bar to give skills more room, 
   - and cause it's less important truthfully
   - this was unexpectedly annoying to do and make work with my health bar height config
 - Perhaps a little better compatibility with extendedloadout
   - or other mods with extra ui that I haven't tested at all like nemry or aatrox if that ever happens
 - This will never happen, hence naming this mod "lite" instead of "2": 
   - More of the crazy custom features that the old BetterHud mod had, namely moving/hiding the other ui elements
   - Would like the cool little skill cooldowns around the crosshair thingys, little prettier more readable less intrusive and such, 
     - would take a bit of work that I won't get around to but I'd love to see it
     - Played doom eternal and dang that shit's great  
___
  
Calling back to DekuDesu's mods' readmes:  
If you like this mod, consider donating to the Autism Research Institute https://www.autism.org/donate-autism-research-institute/  
___
### Changelog:

`0.3.0`
- per request, added option to hide the Keybind label (M1, Shift, etc) under the skills icons. off by default.
- per request, added option to hide all the transparent boxes around stuff. not really in the style of the mod but whatever. off by default.

`0.2.0`
- moved seeker's stratagem inputs up (when skills are centered)
- moved seeker's lotus flower to the side (when skills are centered)
- added riskofoptions

`0.1.4`
 - moving of the hud now happens a frame after `RoR2.UI.HUD.Awake`
   - allows other UI mods to do their thing then be moved as well
   - hopefully fixing a recent issue with TeammateRevival mod

`0.1.3`
 - fix for cum2
 - I think something else?


`0.1.2` 
  - bumped up spectating bar
  - moved notification panel (item pickups) to the side
  - more thanks to destroyedclone for setting me in the direction for these, too
 
`0.1.1` 
  - fixed being added to networked required mods
  - fixed skills being broke. thanks destroyedclone!
  - added ability to change configs while game is running

`0.1.0` 
  - c:

___

whatever you wanna do you can do it  
take it a little at a time, it adds up