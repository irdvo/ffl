\ ==============================================================================
\
\            scl - the single linked cell list in the ffl
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


[UNDEFINED] scl.version [IF]


include ffl/stc.fs
include ffl/scn.fs


1 constant scl.version


( Private structure )

struct: scl%
  cell: scl>first
  cell: scl>last
  cell: scl>length
;struct


( Private words )

: scl-add      ( w w:scl w:prev - -- Add new node after prev in list )
  swap >r                     
  swap scn-new               \ Create new scn
  
  swap dup nil= IF           \ if prev = nil then
    over
    r@ scl>first @
    swap scn>next !          \  new->next = first
    over
    r@ scl>first !           \  first = new
  ELSE                       \ else
    2dup
    scn>next @
    swap scn>next !          \  new->next = prev->next
    2dup
    scn>next !               \  prev->next = new
  THEN
  
  r@ scl>last
  tuck @ = IF                \ if prev = last then
    !                        \   last = prev
  ELSE
    2drop
  THEN
  
  r> scl>length 1+!          \   length++
;


: scl-del    ( w:scl w:prev - -- Delete the element after prev in the list )
  dup nil= IF                \ if prev = nil then
    drop
    dup scl>first
    dup @
    dup scn>next @           \   first = first->next
    rot !                    \   free first
  ELSE                       \ else
    scn>next
    dup @
    dup scn>next @
    rot !                    \   prev->next = prev->next->next
  THEN                       \   free prev->next
  scn-free
  
  dup scl>first @ nil= IF
    dup scl>last nil!
  THEN
  
  scl>length 1-!
  \ ToDo: update last..  
;


: scl-offset ( n w:scl - n -- Determine offset from index, incl. validation )
  scl>length @
  
  over 0< IF                 \ if index < 0 then
    tuck + swap              \   index = index + length
  THEN
  
  over <= over 0< OR IF      \ if index < 0 or index >= length
    exp-index-out-of-range throw 
  THEN
;


: scl-node     ( n w:scl - w:scn-prev w:scn-cur -- Get the nth element in the list )
  nil -rot                   \ prev = nil
  scl>first @                \ cur  = first
  
  BEGIN
    2dup nil<> swap 0> AND   \ while n>0 and cur<> nil do
  WHILE
    rot drop dup -rot        \  prev = cur
    scn>next @               \  cur  = cur->next
    swap 1- swap             \  n--
  REPEAT
  
  nip
;


: scl-search   ( w w:scl - w:scn-prev w:scn-cur -- Search for the first element )
  nil -rot                   \ prev = nil
  scl>first @                \ walk = first
  
  BEGIN
    dup nil<> IF             \ while walk <> nil and walk->cell <> w do
      2dup scn>cell @ <>
    ELSE
      false
    THEN
  WHILE
    rot drop dup -rot        \  prev = walk
    scn>next @               \  walk = walk->next
  REPEAT
  
  nip
;

    


( Public words )

: scl-init     ( w:scl - -- init list )
  dup scl>first  nil!
  dup scl>last   nil!
      scl>length   0!
;


: scl-create   ( "name" - -- Create list in dictionary )
  create   here   scl% allot   scl-init
;


: scl-new      ( - w:scl -- Create list on heap )
  scl% allocate  throw  dup scl-init
;


: scl-delete-all ( w:scl - -- Delete all cells in the list )
  BEGIN
    dup scl>first @ nil<>    \ while first<>nil 
  WHILE
    dup nil scl-del          \  delete first
  REPEAT
  drop
;


: scl-free     ( w:scl - -- Free list from heap )
  dup scl-delete-all
  
  free  throw
;


: scl-empty?   ( w:scl - f -- Check for empty list )
  scl>length @ 0=  
;


: scl-length@  ( w:scl - u -- Return number elements in list )
  scl>length @
;


: scl-append   ( w w:scl - -- Append cell in list )
  dup scl>last @  scl-add
;


: scl-prepend  ( w w:scl - -- Prepend cell in list )
  nil scl-add
;


: scl-count    ( w w:scl - u -- Count cells in list )
  0 >r                       \ count = 0
  scl>first @                \ walk = first
  BEGIN
    dup nil<> 
  WHILE                      \ while walk <> nil do
    2dup
    scn>cell @ = IF          \  if walk->cell = w then
      r> 1+ >r               \   count++
    THEN
    scn>next @               \  walk = walk->next
  REPEAT
  2drop
  r>
;


: scl-delete   ( n:index w:scl - w -- Delete indexth element from list )
  tuck scl-offset over       \ index > offset
  scl-node                   \ offet > prev + curr
  drop                       \ 
  \ ToDo: return cell
  scl-del                    \ delete curr
;


: scl-remove   ( w w:scl - f -- Remove first cell from list )
  dup scl-search             \ cell > prev + curr
  nil<> IF                   \ if curr <> nil then
    scl-del                  \  delete curr
    true
  ELSE                       \ else
    2drop                    \  not found
    false
  THEN
;


: scl-execute  ( xt w:scl - -- Execute xt for every element in list )
  scl>first @                \ walk = first
  BEGIN
    dup nil<>                \ while walk<>nil do
  WHILE
    dup scn>cell @           \   
    swap >r                  \ 
    over execute             \  execute xt with cell
    r>
    scn>next @               \  walk = walk->next
  REPEAT
  2drop
;


: scl-set      ( w n:index w:scl - -- Set indexth element in list )
  tuck scl-offset swap       \ index > offset
  scl-node                   \ offset > prev + curr
  nip                        \ 
  scn>cell !                 \ cell -> curr
;


: scl-get      ( n:index w:scl -- w -- Get indexth element from list )
  tuck scl-offset swap       \ index > offset
  scl-node                   \ offset > element
  scn>cell @                 \ element > cell
  nip
;


: scl-insert   ( w n:index w:scl - -- Insert cell at indexth element in list )
  tuck scl-offset over
  scl-node
  drop scl-add  
;


: scl-find     ( w w:scl - n:index -- Find first index for cell in list, -1 = not found )
  0 -rot                     \ index = 0
  scl>first @                \ walk = first
  
  BEGIN
    dup nil<> IF             \ while walk <> nil and walk->cell <> w do
      2dup scn>cell @ <>
    ELSE
      false
    THEN
  WHILE
    rot 1+ -rot              \  index++
    scn>next @               \  walk = walk->next
  REPEAT
  
  nip
  nil= IF                    \ if walk = nil then
    drop -1                  \   index = -1
  THEN
;


: scl-has?     ( w w:scl - f -- Check if cell is present in list )
  scl-find 0>=
;


: scl-insert-sorted   ( w w:scl - -- Insert in list sorted )
  tuck
  nil -rot                   \ prev = nil
  scl>first @                \ walk = first
  
  BEGIN
    dup nil<> IF             \ while walk <> nil and walk->cell <= w do
      2dup scn>cell @ >=
    ELSE
      false
    THEN
  WHILE
    rot drop dup -rot        \  prev = walk
    scn>next @               \  walk = walk->next
  REPEAT
  
  drop -rot scl-add          \ add the node
;


: scl-reverse  ( w:scl - -- Reverse the list )
  nil over
  scl>first @                \ walk = first
  
  BEGIN
    dup nil<>
  WHILE                      \ while walk<>nil do
    dup scn>next @
    >r
    tuck scn>next !          \  walk->next = prev
    r>
  REPEAT
  2drop
  
  dup  scl>first @
  over dup scl>last @       
  swap scl>first !           \ first = last
  swap scl>last  !           \ last  = first
;


: scl-dump     ( w:scl - -- Dump the list )
  ." scl:" dup . cr
  ."  first :" dup scl>first ?  cr
  ."  last  :" dup scl>last  ?  cr
  ."  length:" dup scl>length ? cr
  
  scl>first @
  BEGIN
    dup nil<> 
  WHILE
    dup scn-dump
    
    scn>next @
  REPEAT
  
  drop
;

[THEN]

\ ==============================================================================