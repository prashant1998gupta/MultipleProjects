Easy Fade Obstructions - Julian Ryan

--Instructions--

1. Attach the EasyFadeObstructions.cs script to the MainCamera.
2. In the inspector, drag your player GameObject into the Player Object field on the MainCamera's attached EasyFadeObstriction script.
3. Set the layer of objects your wish to fade.  If nothing is selected, nothing will fade.

	NOTE:  The material on the objects in the fade layer must have an alpha property.  The standard shaders set to the modes: Fade and Transparent work.
	

--Script Variable Descriptions--

Player Object:  The reference the the object being tracked which will determine the location of things to be faded.

Fade Radius:  Image a sphere being fired toward, and stopping, at the player object.  This determines the radius of that sphere.
			  Anything in the sphere radius will be faded.
			  
Fade Distance Offset:  (See fade radius first) A positive number will cause the fired sphere to go past the player whereas a
						negative will cause it to stop before the player.

Fade Mask Layer:  The objects on any of the selected layers will be faded provided they have a material with an alpha property.
					Multiple layers may be checked.
					
Fade In Speed:  If an object is outside of the Fade Radius, it will fade back in.  This determines how fast it will fade back in.

Fade Out Speed:  If an object is inside of the Fade Radius, it will fade out.  This determines how fast it will fade out.

Fade Transparency:  This changes how transparent the fade will be.  0 Makes it invisible and 1 makes it completely solid.