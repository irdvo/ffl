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
\  $Date: 2008-03-11 18:33:47 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/hct.fs
include ffl/hci.fs

\ Example: store mountain heights in a hash table


\ Create the hash table in the dictionary with an initial size of 10

10 hct-create height-table


\ Add the mountains (in meters)

8300 s" mount everest" height-table hct-insert
4819 s" mont blanc"    height-table hct-insert
5642 s" mount elbrus"  height-table hct-insert

\ Get a mountain height

s" mount everest" height-table hct-get [IF]
  .( Mount everest: ) . cr
[ELSE]
  .( Unknown mountain) cr
[THEN]

s" vaalserberg" height-table hct-get [IF]
  .( Vaalserberg: ) . cr
[ELSE]
  .( Unknown mountain) cr
[THEN]


\ Word for printing the mountain height

: height-emit ( n c-addr u -- = height key )
  type ."  -> " . cr
;


\ Print all mountain heights

' height-emit height-table hct-execute            \ Execute the word height-emit for all entries in the hash table



\ Example hash table iterator

\ Create the hash table iterator in the dictionary

height-table hci-create height-iter               \ Create an iterator named height-iter on the height-table hash table

\ Moving the iterator

height-iter hci-first                         \ Move the iterator to the first record
[IF]                                          \ If record exists Then ..
  height-iter hci-key type                    \   Type the key ..
  .(  => )
  .                                           \   .. and the value
  cr
[THEN]

8300 height-iter hci-move                     \ Move the iterator to the mountain with a height of 8300
[IF]                                          \ If mountain exists Then ..
  height-iter hci-key type                    \   Type the name ..
  .(  => )
  height-iter hci-get drop .                  \   .. and the height
  cr
[ELSE]
  .( No mountain found with a height of 8300) cr  
[THEN]
