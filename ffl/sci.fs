\ ==============================================================================
\
\          sci - the single linked cell list iterator in the ffl
\
\               Copyright (C) 2005  Dick van Oudheusden
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
\  $Date: 2005-12-14 19:27:44 $ $Revision: 1.1.1.1 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] sci.version [IF]


include ffl/stc.fs
include ffl/scl.fs
include ffl/scn.fs


1 constant sci.version


( Private structure )

struct: sci%
  cell: sci>list
  cell: sci>walk
;struct 



( Private words )

: sci-init     ( w:scl w:sci - -- Init iterator with a list )
  tuck sci>list     !
       sci>walk  nil!
;


( Public words )

: sci-create   ( w:scl "name" - w:sci -- Create an iterator in the dictionary )
  create 
    here  sci% allot  sci-init
;


: sci-new      ( w:scl - w:sci -- Create an iterator on the heap )
  sci% allocate  throw  tuck sci-init
;


: sci-free     ( w:sci - -- Free list from heap )
  free throw
;


: sci-get      ( w:sci - w true | false -- Get the cell data from the current record )
  sci>walk @
  dup nil<> IF               \ if current <> nil then
    scn>cell @               \   fetch cell
    true
  ELSE
    drop
    false
  THEN
;


: sci-set      ( w w:sci - -- Set the cell data for the current record )
  sci>walk @
  dup nil<> IF
    scn>cell !
  ELSE
    exp-invalid-state throw
  THEN
;


: sci-first    ( w:sci - w true | false -- Move the iterator to the first record )
  dup sci>list @             
  scl>first @
  over sci>walk !            \ walk = list->first
  sci-get
;


: sci-next     ( w:sci - w true | false -- Move the iterator to the next record )
  dup sci>walk 
  dup @
  dup nil<> IF               \ if walk <> nil then
    scn>next @               \   walk = walk->next
    swap !
  ELSE                       \ else
    exp-invalid-state throw  \   exception
  THEN
  sci-get
;


: sci-move     ( w w:sci - f -- Move the iterator to the <next?> record with the cell data )
  swap
  BEGIN
    over sci-next IF
      over = 
    ELSE
      true
    THEN
  UNTIL
  drop
  sci>walk @ nil<>
;


: sci-first?   ( w:sci - f -- Check if the iterator is on the first record )
  dup sci>list @
  scl>first @
  dup nil= IF
    2drop
    false
  ELSE
    swap sci>walk @ =
  THEN
;


: sci-last?    ( w:sci - f -- Check if the iterator is on the last record )
  dup sci>list @
  scl>last @
  dup nil= IF
    2drop
    false
  ELSE
    swap sci>walk @ =
  THEN
;


: sci-insert-after ( w w:sci - -- Insert the cell data after the current record )
  dup sci>list @
  swap sci>walk @
  dup nil<> IF
    scl-add
  ELSE
    exp-invalid-state throw
  THEN
;


: sci-dump     ( w:sci - -- Dump the iterator )
  ." sci:" dup . cr
  ."  list :" dup sci>list  ?  cr
  ."  walk :"     sci>walk  ?  cr
;

[THEN]

\ ==============================================================================
