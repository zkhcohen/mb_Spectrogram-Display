# The code in this is written _very poorly._ I wrote this _years_ ago. Since I don't have the time to refactor this repo, I'm archiving it.

MusicBee Spectrogram 1.7.5 README:

Tested on MusicBee 3.4.7755 P



Description:

This plugin displays the spectrogram for the currently playing song. It also works as a seekbar,
allowing you to select the location in the track by clicking on it. 

The program works by sending a command to ffmpeg (a free-ware program located in the 
Tooltips folder) which generates a spectrogram. Afterwards, the .png image 
it generated is displayed by the plugin.

After the image has been generated once, it will be used for any future plays, meaning
that it will load almost instantly, with no CPU usage.



Installation Instructions:


1. With MusicBee off, extract the file located in the "Plugins" folder to your MusicBee Plugins directory.

2. Start MusicBee.

3. A message will appear telling you where to place the plugin's "Dependencies" folder;  extract the whole folder there now.

4. In MusicBee navigate to Edit > Edit Preferences > Plugins. Ensure that "spectrogram-display" appears. Enable it, and hit Save.

5. Navigate to View > Arrange Panels...

6. Drag the "spectrogram-display" element from the "available elements" window to the "main panel" section to your desired position, for instance above the "now playing bar" element. Note: This is not the required location of the plugin, but it is HIGHLY RECOMMENDED that you position it here.  Hit Save.

7. Drag the top of the Spectrogram window where you have placed it to the desired height.

8. Try playing a song. After a second or two of processing, the spectrogram should appear.  You can seek through the song if desired within the spectrogram window.

9. See the first post of the plugin's thread on the MusicBee forum for instructions on using the plugin's built-in Configuration Panel and other important information.

NOTE:  If desired, the ffmpeg.exe supplied inside the Dependencies folder can be removed and a path given to your own copy of ffmpeg.exe elsewhere on the PC;  see step #9 for more details.  A 32-bit version of ffmpeg.exe (64-bit supplied) is necessary if you have a 32-bit OS.  The latest static release build at the Zeranoe site has been tested and works fine (https://ffmpeg.zeranoe.com/builds/).



Using the Configuration Panel:


1. To open the configuration panel, go to Edit > Edit Preferences > Plugins, then locate "Spectrogram-Display" and click "Configure", or click on the panel header drop-down.
2. The settings are rather intuitive, but for further information on what they do, please go to the following link:

	https://ffmpeg.org/ffmpeg-filters.html#showspectrumpic

NOTE: Adding a .png file called "placeholder.png" in the Dependencies folder will allow the plugin to display an image of your choice while streams are being played.
