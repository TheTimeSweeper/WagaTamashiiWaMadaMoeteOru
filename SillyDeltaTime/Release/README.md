#### merc dash
reset timer no longer dependent on framerate
#### bandit reload
time after shooting to start reloading no longer dependent on framerate
#### steppedskilldef grace time
untested, very minor
#### character ai
I have no idea what's wrong here but I saw a fixedDeltaTime in a ManagedFixedUpdate which should cause problems so I patched it.  
Disable by default. you can try it and let me know if this changes anything.