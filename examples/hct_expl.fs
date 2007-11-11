\ ==============================================================================
\
\          hnct_expl - the cell hash table example in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2007-11-11 07:41:31 $ $Revision: 1.1 $
\
\ ==============================================================================

include ffl/hct.fs


\ Example: store mountain heights in a hash table


\ Create the hash table in the dictionary with an initial size of 10

10 hct-create height-table


\ Add the mountains (in meters)

8300 s" mount everest" height-table hct-insert


\ Get a mountain height

s" mount everest" height-table hct-get [IF]
  ." Mount everest: " . cr
[ELSE]
  ." Unknown mountain" cr
[THEN]

s" mont blanc" height-table hct-get [IF]
  ." Height: " . cr
[ELSE]
  ." Unknown mountain" cr
[THEN]


\ Word for printing the mountain height

: height-emit ( w c-addr u - = height key )
  type ."  -> " . cr
;


\ Print all mountain heights

' height-emit height-table hct-execute            \ Execute the word height-emit for all entries in the hash table


