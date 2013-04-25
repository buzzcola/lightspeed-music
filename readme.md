Lightspeed
==========

You can learn to sight read with Lightspeed.  Catchy, right?

It's a game that can be played entirely on-screen or with a MIDI keyboard.  The program displays flashcards with single notes,
intervals, and triads (depending on your settings) and the player must score as many correct responses as possible in the time provided.

Lightspeed tracks your average response time and can get really addictive once you get going.  What's your high score and best
response time?  Sent me a message and let me know.

Technology
----------

* The source code is a Visual Studio 2010 Solution using C# 4 and WPF.
* MIDI functionality is provided by Leslie Sanford's excellent [C# MIDI Toolkit](http://www.codeproject.com/KB/audio-video/MIDIToolkit.aspx).
* Flashcard images are generated using [Lilypond](http://lilypond.org/).  (Lilypond is not required to run this software - the images are embedded resources.)

Current State
-------------

The game is ready to play on any windows computer although not extensively tested outside of my hardware/software stack.  The solution includes a
windows setup project, or you can download the installer here: (TODO, put installer up somewhere.)

Learning to Sight Read Music
----------------------------

If you're a music student or teacher, you're familiar learning to sight read and how tough it is.  I found that when I worked on new repertoire
I only spent a certain amount of time reading - then the material was memorized and I was working more on articulation and performance.
There are sight reading exercise books out there but I wanted something fun that would give me feedback for incorrect notes.

Lightspeed will _not_ help develop your ability to read ahead of your hands, since only one note or note combination is displayed at
a time.  But it _is_ a hardcore drill on note recognition, shape recognition and keyboard geography, and by incorporating a few rounds every morning into my
practice routine it's made a big difference in my sight reading.  I hope it helps you too.

Future Goals
------------
* A more beautiful interface design would be nice.  Since I'm not much of a designer I've erred on the side of simplicity.
* Real-time audio input with pitch recognition: would allow support for non-MIDI instruments to be added.