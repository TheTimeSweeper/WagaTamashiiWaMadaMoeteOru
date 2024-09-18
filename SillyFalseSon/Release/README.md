# son
reach out to me (`thetimesweeper`) on the ror2 modding discord or the enforcer discord (https://discord.gg/r5XTMFd4W7) if ye want

![](https://raw.githubusercontent.com/TheTimeSweeper/WagaTamashiiWaMadaMoeteOru/master/SillyFalseSon/Release/_readme/FunnyCSS.png)  
lol

## TL;DR
- Made him 1.12x bigger (configurable)
    - bumped camera higher a smidge
- Made his lobby size reflect his actual size (configurable)
- m1 hibox size increased (configurable)
- reduced the size and distance of M1 swing effects (not configurable sorry)
- increased the size of m1 + m2 slam to match the hitbox (not configurable sorry)
- m1+m2 slam changes: jump arc tweaked and instantly slams when landing (toggleable)
- added additional m2 stocks to crosshair (configurable) 
- moved dash to body state machine so it doesn't interrupt other attacks (toggleable)

___

## Size
- Made him 1.12x bigger (configurable)
    - you can config this back to 1. he's actually plenty big
    - camera height and depth configurable to account for this
- Made his lobby size reflect his actual size (configurable)
    - the whole reason I made this mod is I saw in CSS he wasn't even bigger than loader. turns out he's over 2 commandos tall.
    - i've removed this discrepancy, and while I think he should be a bit taller in css, he doesn't need to be actual size. keeping it for the funny
- m1 hibox size increased (configurable)
    - Sorry, I'm the hitbox guy. His hitbox was ok, just could be bigger
    - The bigger problem was swing effects were so egregiously large that they made you think things were gonna get hit that were not even close to the hitbox
## Effects
- reduced the size and distance of M1 swing effects (not configurable sorry)
    - these now line up with the default hitbox
- increased the size of m1 + m2 slam to match the hitbox (not configurable sorry)  
<img width="500" src="https://raw.githubusercontent.com/TheTimeSweeper/WagaTamashiiWaMadaMoeteOru/master/SillyFalseSon/Release/_readme/funnyHitbox.png" />
    - did you know the hitbox was actually this big
    - also made it not disappear abruptly

## Actual Gameplay Changes
- hitbox point mentioned above
- m1+m2 slam changes (toggleable)
    - jump arc when used in air tightened to flow better
    - jump arc timing no longer affected by attack speed
    - instantly slams when landing on the ground
- added additional m2 stocks to crosshair (configurable)  
<img width="300" src="https://raw.githubusercontent.com/TheTimeSweeper/WagaTamashiiWaMadaMoeteOru/master/SillyFalseSon/Release/_readme/funnyCrosshair.png" />
    - i know it's not a satisfying radial pattern but I couldn't be arsed to do that
- moved dash to body state machine so it doesn't interrupt other attacks (toggleable)
    - can dash in and slam
    - can dash while charging R
        - can no longer cancel R lol
    - is it a little strong? yeah probably. is it fun? absolutely.
        - should it be like this by default? idk. that's why this mod is called SillyFalseSon and not FalseSonFixed or something cringe like that.
    - that said, there's a bit of a design issue in the fact that there's 0 reason not to use the m1+m2 slam over regular m1. 
        - it has a much larger area and basically double the dps if you just spam that. 
        - there should be a decision to make whether to use regular m1 or to use the slam, but as it stands you should just always use the slam, to the point what's the point of the difference.
        - If I was taking this seriously I would address that but I'm not so you get funny dash c:

___

## My False Son Laundry List
I'm not going to because I don't have time but hey if you're curious,  
What I would do if he was one of my boys:

- M1+M2
    - ~~Fix Minimum time on air slam~~
    - ~~Air slam better initial jump to slam down speed flow~~
    - Actually when holding m1+m2 not just when m1 was held first
    - More pronounced impact effects. atm enemies just kinda take damage
    - ~~Slam effect way fucking bigger for how big the hitbox is~~
        - ~~slam doesn't abruptly disappear~~
    - Intersection shockwave that shows you the area
    - Some view recoil to really feel it
- M4 laser moving up and down
    - fixed in StormTweaks mod. one of the only changes I agree with lol
- ~~move dash to body state machine~~
- Dash preserves initial XZ velocity
- Shards fly faster and probably bigger hitbox
- m2 interrupt alt r
- ~~hitbox~~

pie in the sky:
- m1 animations, little more windup to feel more satisfying
- little cleaner and clearer full charge effects
- Shards be actual physical models on his body that appear and disappear as they're gained and thrown respectively
    - Shooting them out wil actually communicate youâre getting lighter to be faster
    - Keeipng them on you in combat will communicate you have more armor
    - Having them reappear one by one while using special will be actual sex

You'll notice I'm not really reworking any major part of his design. Just polish and tweaks. I don't like changing characters' existing designs to my views as if I think I know better or anything like that.
___

## Changelog
`2.0.0`
- added everything that wasn't in 1.0.0