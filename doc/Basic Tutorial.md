
## 1. Sprites
Sprites can contain a single image or a sequence of images that form an animation. It’s recommended to import images from files rather than drawing them from scratch, because the built‑in image editor is not even 50% complete. You can use it for very simple shapes and drawings, though.

Do not leave a sprite without an image, because the engine currently cannot handle empty sprites. Also avoid including sprites in the project that are not assigned to any object, as this may cause bugs.

By default, the origin point for positioning and rotation is (0, 0) relative to the object’s position. You can change the origin by clicking the image preview in the sprite editor or by manually editing the values in the text fields. You can also set a custom collision mask for your sprite in the sprite editor.

## 2. Objects
You can assign a sprite to an object using the selection box on the right. Control the animation speed by setting the imageSpeed property in the Create event (imageSpeed = number of game frames before increasing imageIndex → higher value = slower animation).

To add events, click Add Event and choose Create, Step, or Draw. Do not select other events yet, as they are not implemented.

To add a task (script), select the event from the list, click Add Script, then double‑click the new task that appears. Start your scripts with /// description.

Make sure every object has an assigned sprite, because objects without sprites are not supported yet. If you want an object to be invisible, set its visible property to false.

If you add code to the Draw event, only what you draw manually will appear. The sprite will not render unless you call drawSelf().

## 3. Sounds
There are two types of sounds: background music and sound effects. Multiple sound effects can play simultaneously, but only one background music track can play at a time. Use .mp3 for background music and .wav for sound effects.

To play a sound:
`
playSound(Sounds.soundName, /* repeating: */ false)
`

Note: the “Play” button in the sound editor is currently non‑functional.

## 4. Paths
You can use the path editor to create paths that objects can follow or that can be drawn using built‑in functions:

```
startPath(Paths.pathName, /* speed: */ 4, /* endAction: */ PathEndAction.reverse, /* absolute: */ true)

drawPath(path, x, y, color, width)
```

## 5. Rooms
Use the room editor to design levels. Select an object and place it in the room by clicking on the map. You can set the room size and window caption in the Settings tab. You can also add views (cameras) and make them follow your player.

Critical note: When views are enabled, the game window size should automatically adjust to fit all viewports. This is not implemented yet, so avoid using views until it’s fixed (or fix it yourself in `ArcadeMaker\Engines\MonoGame\Core\ArcadeMakerMono.cs`).

## 6. Backgrounds
Backgrounds are not implemented in the engine yet.

## 7. Fonts
Font support is not fully implemented, but you can already create a font, choose its family, italic style, and size, and then use `drawText(x, y, text)` in the Draw event.

To align text with a view, use:
- getViewX(viewIndex)
- getViewY(viewIndex)
- getViewWidth(viewIndex)
- getViewHeight(viewIndex)

You can also use the currentViewIndex argument (passed to the Draw event) to draw text only for a specific view.

## 8. Collision Detection
The only collision method currently available is rotated rectangles. The engine automatically generates a minimal rectangle around the non‑transparent pixels of a sprite (the collision mask, editable in the sprite editor). It uses the object’s x, y, and imageAngle to check collisions.

Available functions:
- placeMeeting(x, y, typeOrInst)
- instanceMeeting(x, y, type)
- placeFree(x, y) — checks collision with any object marked as solid
- placeEmpty() — not implemented yet
