CCStudio
=======

A CCEmulator written in C#

##The code


###CCStudio.Core
This contains the core files:

 - APIs
 - Filesystem
 - Saving
 - Peripherals

This should be required create an emulator

###CCStudio.MonoGame
This is simply an experimental renderer for the Terminal written in MonoGame, it works but the font is ugly and some keys don't work.

###CCStudio.WPF (WIP)
This is very much a work in progress and is designed implement everything needed to emulate a computer:

 - Filesystem mount management
 - Rebooting
 - Peripheral management
 - Computer management
 - Console

###CCStudio.GTKSharp (Not yet started)
This will be a port of `CCStudio.WPF` to GTK#.
