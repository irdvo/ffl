\ ==============================================================================
\
\                  config - the config in the ffl
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
\  $Date: 2006-01-13 18:59:54 $ $Revision: 1.1 $
\
\ ==============================================================================
\
\ This file is for gforth.
\
\ ==============================================================================


s" ffl.version" forth-wordlist search-wordlist 0= [IF]


( config = Forth system specific words )
( The config module contains the extension and missing words for a forth system.)


000100 constant ffl.version


( Public Settings )


( Public words )

\ : [DEFINED]   
\   bl word find   nip 0<>   
\ ; immediate


\ : [UNDEFINED] 
\   bl word find   nip 0= 
\ ; immediate


\ : 2rdrop ( - ) 
\   2r> 2drop
\ ;


: 2+               ( n - n+2 = Add two to tos)
  1+ 1+
;


\ : cell           ( - n = Cell size)
\   1 cells
\ ;


\ : >=             ( n1 n2 - n1>=n1 = Greater equal)
\   < not
\ ;


\ : 0>=            ( n - f = Greater equal 0 )
\   0 >=
\ ;


\ : on             ( w - = Set boolean variable to true)
\   true swap !
\ ;


\ : off            ( w - = Set boolean variable to false)
\   false swap !
\ ;


\ 0 constant nil   ( - w = Nil address )


: 0!               ( w - = Set zero in address )
  0 swap !
;


: nil!             ( w - = Set nil in address )
  nil swap !
;


: nil=             ( w - f = Check for nil )
  nil =
;


: nil<>            ( w - f = Check for unequal to nil )
  nil <>
;


: 1+!              ( w - = Increase contents of address by 1 )
  1 swap +!
;


: 1-!              ( w - = Decrease contents of address by 1 )
  -1 swap +!
;


( Public Exceptions )

\ variable exp-next  -2050 exp-next !
\
\ : exception      ( w:addr u - n = add an exception )
\   2drop
\   exp-next @ 
\   -1 exp-next +!
\ ;


s" Index out of range" exception constant exp-index-out-of-range ( - n = Index out of range exception number )
s" Invalid state"      exception constant exp-invalid-state      ( - n = Invalid state exception number )

[ELSE]
  drop
[THEN]

\ ==============================================================================

