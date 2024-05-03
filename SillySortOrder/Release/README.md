# Sort those bois

No longer are we prey to the arbitrary whimsy of mod makers' choice of whatever-at-the-time.  

Ping me (`TheTimesweeper#5727`) on the ror2 modding discord or the enforcer discord (https://discord.gg/csaEQDnMH8) if you have any issues/feedback.  
or if you just want to send (or even recieve) some nice words of encouragement
___
## Default: Mixes ror1 survivors with the rest of the cast

![The Gang's All Here](https://i.imgur.com/mQNrhfH.png)
Look at them bois

___
## Options:
![The Gang's All Here](https://i.imgur.com/IWK5SjX.png)

<ins>ForceModdedCharactersOut:</ins>
- Pulls out modded characters from the lineup and places them after ror1 and ror2 survivors
- Goku and Vegeta get outta here

<ins>MixRor1Survivors:</ins>
- On by default. Mixes in ror1 folks loosely based on their unlock condition
- When off, neatly separates and groups ror1 survivors right after Vanilla

<ins>NemesesSeparate:</ins>
- Place Nemesis survivors together at the end of the main block, instead of right next to their counterpart where they usually are

<ins>CustomOrder:</ins>
- Finally, place any character in any spot you want by specifying their desired sort position.
- See Print Sorting config for existing order to copy paste and change. 
- See the wiki tab on this thunderstore page for more info.
___
### Maybe Plans:
 - Option to order all survivors to ror1 order for fun.
___
### Changelog:
`1.0.4`
- improved whitespace trimming in custom sort config 

`1.0.3`
- put all my code in a big try catch so it doesn't break all survivors
  - i have accepted failure

`1.0.2`
- fixed survivors with spaces in their bodynames being un-custom-sort-able

`1.0.1`
- fixed a freeze on startup

`1.0.0`
- wrote not stupid code to properly detect Nemesis survivors in the nemeses sorting
  - this puts nemesis amp before amp in the "nemeses separate" config so uh lol

`0.2.1`
- updated custom sort order config flow
  - now prints the full sorted order in console and in config so you can simply copy paste and change what you want
  - thanks JavAngle for the idea
- print sorting config now does nothing but paste the sort order in its description when the game runs
  - its previous functionality is now always on
- fixed a harmless error showing when custom sort was empty

'0.2.0'
 - added custom order config
 - added support for new starstorms commandos
 - moved HAN-D to his default spot in his mod even though my spot was better, based on the unlock
   - config HANDOverclockedBody:8.1 to bring it back to how it was with this mod 

`0.1.2` 
  - fixed survivors being before dlc survivors
  - moved logoutput to a config

`0.1.1` 
  - Fixed Goku and Vegeta not being forced out. They were too powerful

`0.1.0` 
  - c:

___

may the day treat you well