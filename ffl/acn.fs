\ ==============================================================================
\
\           acn - the AVL binary tree cell node module in the ffl
\
\               Copyright (C) 2006  Dick van Oudheusden
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
\  $Date: 2008-04-30 06:15:13 $ $Revision: 1.1 $
\
\ ==============================================================================


include ffl/config.fs


[UNDEFINED] acn.version [IF]


include ffl/stc.fs
include ffl/bcn.fs


( acn = AVL binary tree cell node module )
( The acn module implements a node in an AVL binary tree. The acn structure  )
( extends the bcn structure.                                                 )


2 constant acn.version


( AVL Tree node structure )

begin-structure acn%   ( -- n = Get the required space for an acn node )
  bcn% 
  +field   acn>node            \ Extend the the bcn structure with ..
  cfield:  acn>balance         \ .. the balance factor (-1,0,1)
end-structure



( Node creation, initialisation and destruction )

: acn-init         (  x1 x2 acn1 acn2 -- = Initialise the node acn2 with the parent acn1, key x2 and data x1 )
  >r
  r@ bcn-init
  r> acn>balance 0!
;


: acn-new          ( x1 x2 acn1 -- acn2 = Create a new node acn2 on the heap with parent acn1, key x2 and data x1 )
  acn% allocate  throw   >r r@ acn-init r>
;


: acn-free         ( acn -- = Free the node from the heap )
  free throw 
;


( Private words )

: acn-balance@     ( acn -- n = Get the balance of the node )
  acn>balance @
;


( Inspection )

: acn-dump         ( acn -- = Dump the node )
  ." acn:" dup . cr
  dup acn>node bcn-dump
  ."   balance:" acn>balance ? cr
;

[THEN]

\ ==============================================================================
