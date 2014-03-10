# PetitMIDI #
PetitMIDI is a program to play MML music. MML [(Music Macro Language)](http://en.wikipedia.org/wiki/Music_Macro_Language)  
is a musical language that can be used to write songs of all types. The  
MML used in this app can be copied for use in programs that run on  
[Petit Computer](http://www.petitcomputer.com/).
## Quick Use ##
`CDEFGAB`  
These represent basic notes. The length of a note (quarter, eigth, etc.)  
can be adjusted for individual notes by including a number after the note.  
Notes can also be dotted by adding a period. For example, `C4.` represents  
a C, as a 3/8 note. Notes can be tied together by typing an `&` between successive  
notes. Note that th behavior of the tie is based on note length - In Petit Computer,  
the tie will change pitch if you tie together two notes of different pitches.  

`N`  
This plus a number specifies the exact note played, where `N60` represents  
middle C and each number up or down represents a single half step.  
The length of this note is determined by the channel default, and can't be dotted.  

`R`  
This represents a rest, where no music is played. This rest can have its  
length adjusted, and can be dotted like a regular note.  

`#+-`  
Placing a pound (sharp) or plus sign after a note raises it a half step.  
Similarly, placing a minus sign after a note lowers it by a half step.  

`<>`  
Placing a `<` increases the octave by 1, so notes play one octave higher  
than usual. Placing a `>` does the opposite, dropping the octave by 1.  

`T120`  
Typing a T at the beginning, followed by a number, adjusts the tempo of the  
entire song.  

`:0`  
Typing a colon, followed by a number from 0-7, lets you specify music for  
a specific channel in the player. Since the individual tracks are monotonic,  
this is one way to play chords, aside from arpeggio.  

`@0`  
Typing an `@` sign, followed by a number, specifies the instrument to use  
for the specified channel. The instruments provided follow the General MIDI  
specification, with the exception that percussion is done by setting the instrument  
number to 128/129, allowing any channel to act as a percussion channel. Instruments  
144-150 can be used to play square waves at different duty cycles, starting at  
12.5% duty on 144, up to 87.5% duty on 150. Instrument 151 is a noise generator.

## Unsupported Features ##
Aside from the limitations of the music player, this app doesn't support:  


* User made waveforms (instrument 255-256 on Petit Computer)  
* Tremolo, Vibrato  
* ADSR envelopes (the current implementation is lacking)  
* Gate Time (Q)  
* Detune
* Portamento
* Panning for PSG channels
